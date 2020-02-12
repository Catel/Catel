// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowLogic.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.MVVM.Providers
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Views;
    using Logging;
    using MVVM;
    using Reflection;
    using Catel.Data;

    /// <summary>
    /// MVVM Provider behavior implementation for a window.
    /// </summary>
    public class WindowLogic : LogicBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        
        private bool? _closeInitiatedByViewModel;
        private bool? _closeInitiatedByViewModelResult;

        private readonly string _targetWindowClosedEventName;
        private readonly Catel.IWeakEventListener _targetWindowClosedWeakEventListener;
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

            var closedEvent = targetWindowType.GetEventEx("Closed");
            var eventName = closedEvent != null ? "Closed" : "Unloaded";

            _targetWindowClosedWeakEventListener = this.SubscribeToWeakGenericEvent<EventArgs>(targetWindow, eventName, OnTargetWindowClosed);

            _targetWindowClosedEventName = eventName;

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
        public override async Task OnTargetViewUnloadedAsync(object sender, EventArgs e)
        {
            await base.OnTargetViewUnloadedAsync(sender, e);

            // This should only happen when the window only exposes an Unloaded event
            var vm = ViewModel;
            if (vm != null && !vm.IsClosed)
            {
                await CloseViewModelAsync(null, true);
            }

            ViewModel = null;
        }

        /// <summary>
        /// Called when the <see cref="LogicBase.ViewModel"/> is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.MVVM.ViewModelClosedEventArgs"/> instance containing the event data.</param>
        public override async Task OnViewModelClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            if (_closeInitiatedByViewModel is null)
            {
                _closeInitiatedByViewModel = true;
                _closeInitiatedByViewModelResult = e.Result;
            }

            await base.OnViewModelClosedAsync(sender, e);

            if (_closeInitiatedByViewModelResult != null)
            {
                bool result;
                try
                {
                    result = PropertyHelper.TrySetPropertyValue(TargetWindow, "DialogResult", BoxingCache.GetBoxedValue(_closeInitiatedByViewModelResult), true);
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
#pragma warning disable AvoidAsyncVoid // Avoid async void
        // ReSharper disable UnusedMember.Local
        public async void OnTargetWindowClosed(object sender, EventArgs e)
        // ReSharper restore UnusedMember.Local
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            if (_closeInitiatedByViewModel is null)
            {
                _closeInitiatedByViewModel = false;

                bool? dialogResult = null;
                if (!PropertyHelper.TryGetPropertyValue(TargetWindow, "DialogResult", out dialogResult))
                {
                    Log.Warning("Failed to get the 'DialogResult' property of window type '{0}', using 'null' as dialog result", TargetWindow.GetType().Name);
                }

                if (dialogResult is null)
                {
                    // See https://github.com/Catel/Catel/issues/1503, even though there is no real DialogResult,
                    // we will get the result from the VM instead
                    var vm = ViewModel;
                    if (vm != null)
                    {
                        dialogResult = vm.GetResult();
                    }
                }

                await CloseViewModelAsync(dialogResult, true);
            }

            _targetWindowClosedWeakEventListener.Detach();
        }

        /// <summary>
        /// Invokes the close method on the window dynamically.
        /// </summary>
        private void InvokeCloseDynamically()
        {
            var closeMethod = TargetWindow.GetType().GetMethodEx("Close");
            if (closeMethod is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Cannot close any window without a public 'Close()' method, implement the 'Close()' method on '{0}'", TargetWindow.GetType().Name);
            }

            closeMethod.Invoke(TargetWindow, null);
        }
        #endregion
    }
}

#endif
