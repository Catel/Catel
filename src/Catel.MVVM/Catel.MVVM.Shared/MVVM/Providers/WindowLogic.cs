// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.MVVM.Providers
{
    using System;
    using System.Windows;
    using Views;
    using Logging;
    using MVVM;
    using Reflection;

#if SILVERLIGHT
    using System.Windows.Controls;
#endif

    /// <summary>
    /// MVVM Provider behavior implementation for a window.
    /// </summary>
    /// <remarks>
    /// Some parts in this class (with the instances and increments), but this is required to dynamically subscribe to
    /// an even that we do not know the handler of on forehand. Normally, you would do this via an anynomous delegate, 
    /// but that doesn't work so the event delegate is created via ILGenerator at runtime.
    /// <para />
    /// http://stackoverflow.com/questions/8122085/calling-an-instance-method-when-event-occurs/8122242#8122242.
    /// </remarks>
    public class WindowLogic : LogicBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        
        private bool? _closeInitiatedByViewModel;
        private bool? _closeInitiatedByViewModelResult;

        private readonly DynamicEventListener _dynamicEventListener;

#if SILVERLIGHT
        private bool _isClosed;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowLogic"/> class.
        /// </summary>
        /// <param name="targetWindow">The window this provider should take care of.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="viewModel">The view model to inject.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetWindow"/> is <c>null</c>.</exception>
        public WindowLogic(IView targetWindow, Type viewModelType = null, IViewModel viewModel = null)
            : base(targetWindow, viewModelType, viewModel)
        {
            var targetWindowType = targetWindow.GetType();

            string eventName;

            var closedEvent = targetWindowType.GetEventEx("Closed");
            if (closedEvent != null)
            {
                eventName = "Closed";

                _dynamicEventListener = new DynamicEventListener(targetWindow, "Closed", this, "OnTargetWindowClosed");
            }
            else
            {
                eventName = "Unloaded";

                _dynamicEventListener = new DynamicEventListener(targetWindow, "Unloaded", this, "OnTargetWindowClosed");
            }

            Log.Debug("Using '{0}.{1}' event to determine window closing", targetWindowType.FullName, eventName);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the logic should call <c>Close</c> immediately when
        /// the <c>DialogResult</c> is set.
        /// <para />
        /// By default, the <c>Window</c> class correctly closes the window when the <c>DialogResult</c> is 
        /// set, but not all implementations work like this.
        /// <para />
        /// The default value is false.
        /// </summary>
        /// <value>
        /// <c>true</c> if <c>Close</c> should be called directly after setting the <c>DialogResult</c>; otherwise, <c>false</c>.
        /// </value>
        public bool ForceCloseAfterSettingDialogResult { get; set; }

        /// <summary>
        /// Gets the target control as window object.
        /// </summary>
        /// <value>The target window.</value>
        private FrameworkElement TargetWindow
        {
            get { return (FrameworkElement)TargetView; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the data context of the target control.
        /// <para />
        /// This method is abstract because the real logic implementation knows how to set the data context (for example,
        /// by using an additional data context grid).
        /// </summary>
        /// <param name="newDataContext">The new data context.</param>
        protected override void SetDataContext(object newDataContext)
        {
            TargetView.DataContext = newDataContext;
        }

        /// <summary>
        /// Called when <see cref="LogicBase.TargetView"/> has just been unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void OnTargetViewUnloaded(object sender, EventArgs e)
        {
            base.OnTargetViewUnloaded(sender, e);

            ViewModel = null;
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel"/> is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.MVVM.ViewModelClosedEventArgs"/> instance containing the event data.</param>
        public override void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
        {
            if (_closeInitiatedByViewModel == null)
            {
                _closeInitiatedByViewModel = true;
                _closeInitiatedByViewModelResult = e.Result;
            }

            base.OnViewModelClosed(sender, e);

#if SILVERLIGHT
            if (TargetWindow is ChildWindow)
            {
                // This code is implemented due to a bug in the ChildWindow of silverlight, see:
                // http://silverlight.codeplex.com/workitem/7935

                // Only handle this once
                if (_isClosed)
                {
                    return;
                }
            }
#endif

            if (_closeInitiatedByViewModelResult != null)
            {
                bool result;
                try
                {
                    result = PropertyHelper.TrySetPropertyValue(TargetWindow, "DialogResult", _closeInitiatedByViewModelResult);
                }
                catch (Exception ex)
                {
                    Log.Warning("Failed to set the 'DialogResult' exception: {0}", ex);
                    result = false;
                }

                // Support all windows (even those that do not derive from ChildWindow)
                if (!result)
                {
                    Log.Warning("Failed to set the 'DialogResult' property of window type '{0}', closing window via method", TargetWindow.GetType().Name);

                    InvokeCloseDynamically();
                }
                else if (ForceCloseAfterSettingDialogResult)
                {
                    InvokeCloseDynamically();
                }
            }
            else
            {
                InvokeCloseDynamically();
            }
        }

        /// <summary>
        /// Called when the <see cref="TargetWindow"/> has been closed.
        /// </summary>
        /// <remarks>
        /// Public to allow the generated ILGenerator to access this method.
        /// </remarks>
        // ReSharper disable UnusedMember.Local
        public async void OnTargetWindowClosed()
        // ReSharper restore UnusedMember.Local
        {
#if SILVERLIGHT
            // This code is implemented due to a bug in the ChildWindow of silverlight, see:
            // http://silverlight.codeplex.com/workitem/7935

            // Only handle this once
            if (_isClosed)
            {
                return;
            }

            _isClosed = true;
#endif

            if (_closeInitiatedByViewModel == null)
            {
                _closeInitiatedByViewModel = false;

                bool? dialogResult = null;
                if (!PropertyHelper.TryGetPropertyValue(TargetWindow, "DialogResult", out dialogResult))
                {
                    Log.Warning("Failed to get the 'DialogResult' property of window type '{0}', using 'null' as dialog result", TargetWindow.GetType().Name);
                }

                await CloseViewModelAsync(dialogResult);
            }

            _dynamicEventListener.UnsubscribeFromEvent();
        }

        /// <summary>
        /// Invokes the close method on the window dynamically.
        /// </summary>
        private void InvokeCloseDynamically()
        {
            var closeMethod = TargetWindow.GetType().GetMethodEx("Close");
            if (closeMethod == null)
            {
                string error = string.Format("Cannot close any window without a public 'Close()' method, implement the 'Close()' method on '{0}'", TargetWindow.GetType().Name);
                Log.Error(error);

                throw new NotSupportedException(error);
            }

            closeMethod.Invoke(TargetWindow, null);
        }
        #endregion
    }
}

#endif