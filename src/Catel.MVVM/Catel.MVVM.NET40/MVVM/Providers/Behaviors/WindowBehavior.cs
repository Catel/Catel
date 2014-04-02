// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Providers
{
    using System;
    using System.Reflection;
    using System.Windows.Interactivity;
    using System.Windows;
    using Catel.Logging;
    using Catel.MVVM.Views;
    using MVVM;
    using Reflection;

    /// <summary>
    /// A <see cref="Behavior{T}"/> implementation for a window. 
    /// </summary>
    public class WindowBehavior : MVVMBehaviorBase<FrameworkElement, WindowLogic>
    {
        #region Classes
        /// <summary>
        /// Class that parses an event target in the form of [controlname].[event].
        /// </summary>
        public class EventTarget
        {
            #region Fields
            private readonly FrameworkElement _associatedObject;
            private readonly object _target;
            private readonly EventInfo _eventInfo;
            private readonly Delegate _delegate;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="EventTarget"/> class.
            /// </summary>
            /// <param name="associatedObject">The associated object.</param>
            /// <param name="eventTarget">The event target as string representation.</param>
            /// <remarks>
            /// If the parsing fails, no exception will be thrown but the <see cref="ControlName"/>
            /// and <see cref="EventName"/> will remain empty.
            /// </remarks>
            /// <exception cref="ArgumentNullException">The <paramref name="associatedObject"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentException">The <paramref name="eventTarget"/> is <c>null</c> or whitespace.</exception>
            /// <exception cref="InvalidOperationException">The <paramref name="associatedObject"/> is not yet loaded.</exception>
            public EventTarget(FrameworkElement associatedObject, string eventTarget)
            {
                Argument.IsNotNull("associatedObject", associatedObject);
                Argument.IsNotNullOrWhitespace("eventTarget", eventTarget);

#if NET
                if (!associatedObject.IsLoaded)
                {
                    throw new InvalidOperationException("The associated object is not yet loaded, which is required");
                }
#endif

                _associatedObject = associatedObject;
                ControlName = string.Empty;
                EventName = string.Empty;

                int dotIndex = eventTarget.IndexOf('.');
                if (dotIndex == -1)
                {
                    ControlName = eventTarget;
                    EventName = "Click";
                }
                else
                {
                    ControlName = eventTarget.Substring(0, dotIndex);
                    EventName = eventTarget.Substring(dotIndex + 1, eventTarget.Length - dotIndex - 1);
                }

                _target = _associatedObject.FindName(ControlName);
                if (_target == null)
                {
                    throw new InvalidOperationException(string.Format("'{0}' resulted in control name '{1}', which cannot be found on the associated object",
                        eventTarget, ControlName));
                }

                _eventInfo = _target.GetType().GetEventEx(EventName);
                if (_eventInfo == null)
                {
                    throw new InvalidOperationException(string.Format("'{0}' resulted in event name '{1}', which cannot be found on target '{2}'",
                        eventTarget, EventName, ControlName));
                }

                var methodInfo = GetType().GetMethodEx("OnEvent", BindingFlagsHelper.GetFinalBindingFlags(true, false));
                var boundEventHandler = methodInfo.MakeGenericMethod(new[] { _eventInfo.EventHandlerType.GetMethodEx("Invoke").GetParameters()[1].ParameterType });

                _delegate = DelegateHelper.CreateDelegate(_eventInfo.EventHandlerType, this, boundEventHandler);
                _eventInfo.AddEventHandler(_target, _delegate);
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the name of the control.
            /// </summary>
            /// <value>The name of the control.</value>
            private string ControlName { get; set; }

            /// <summary>
            /// Gets or sets the event name.
            /// </summary>
            /// <value>The name of the event.</value>
            private string EventName { get; set; }
            #endregion

            #region Events
#pragma warning disable 67
            /// <summary>
            /// Occurs when the specified event occurs.
            /// </summary>
            public event EventHandler Event;
#pragma warning restore 67
            #endregion

            #region Methods
            /// <summary>
            /// Called when the event on the target occurs.
            /// </summary>
            /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
// ReSharper disable UnusedMember.Local
            public void OnEvent<TEventArgs>(object sender, TEventArgs e)
// ReSharper restore UnusedMember.Local
            {
                if (Event != null)
                {
                    Event(this, EventArgs.Empty);
                }
            }

            /// <summary>
            /// Cleans up.
            /// </summary>
            public void CleanUp()
            {
                if (_eventInfo != null)
                {
                    _eventInfo.RemoveEventHandler(_target, _delegate);
                }
            }
            #endregion
        }
        #endregion

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private EventTarget _saveTargetInfo;
        private EventTarget _saveAndCloseTargetInfo;
        private EventTarget _cancelTargetInfo;
        private EventTarget _cancelAndCloseTargetInfo;
        private EventTarget _closeTargetInfo;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowBehavior"/> class.
        /// </summary>
        /// <remarks></remarks>
        public WindowBehavior()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowBehavior"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public WindowBehavior(IViewModel viewModel)
            : base(viewModel)
        {   
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the save action.
        /// <para />
        /// The format is [controlname].[event]. By default, the <c>Click</c> event is used, thus for
        /// a button [controlname] is sufficient.
        /// <para />
        /// This property is not required, but if it is used, it must be valid otherwise an exception will be thrown.
        /// </summary>
        /// <value>The save action.</value>
        public string Save { get; set; }

        /// <summary>
        /// Gets or sets the save and close action.
        /// <para />
        /// The format is [controlname].[event]. By default, the <c>Click</c> event is used, thus for
        /// a button [controlname] is sufficient.
        /// <para />
        /// This property is not required, but if it is used, it must be valid otherwise an exception will be thrown.
        /// </summary>
        /// <value>The save action.</value>
        public string SaveAndClose { get; set; }

        /// <summary>
        /// Gets or sets the cancel action.
        /// <para />
        /// The format is [controlname].[event]. By default, the <c>Click</c> event is used, thus for
        /// a button [controlname] is sufficient.
        /// <para />
        /// This property is not required, but if it is used, it must be valid otherwise an exception will be thrown.
        /// </summary>
        /// <value>The cancel action.</value>
        public string Cancel { get; set; }

        /// <summary>
        /// Gets or sets the cancel and close action.
        /// <para />
        /// The format is [controlname].[event]. By default, the <c>Click</c> event is used, thus for
        /// a button [controlname] is sufficient.
        /// <para />
        /// This property is not required, but if it is used, it must be valid otherwise an exception will be thrown.
        /// </summary>
        /// <value>The cancel action.</value>
        public string CancelAndClose { get; set; }

        /// <summary>
        /// Gets or sets the close action.
        /// <para />
        /// The format is [controlname].[event]. By default, the <c>Click</c> event is used, thus for
        /// a button [controlname] is sufficient.
        /// <para />
        /// This property is not required, but if it is used, it must be valid otherwise an exception will be thrown.
        /// </summary>
        /// <value>The cancel action.</value>
        public string Close { get; set; }

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
        /// 	<c>true</c> if <c>Close</c> should be called directly after setting the <c>DialogResult</c>; otherwise, <c>false</c>.
        /// </value>
        public bool ForceCloseAfterSettingDialogResult
        {
            get { return Logic.ForceCloseAfterSettingDialogResult; }
            set { Logic.ForceCloseAfterSettingDialogResult = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the logic required for MVVM.
        /// </summary>
        /// <returns>The <see cref="LogicBase"/> implementation uses by this behavior.</returns>
        protected override WindowLogic CreateLogic()
        {
            var associatedObjectType = AssociatedObject.GetType();
            if (!associatedObjectType.ImplementsInterfaceEx<IView>())
            {
                string error = string.Format("Type '{0}' does not implement IView, make sure to implement the interface correctly", associatedObjectType);
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            return new WindowLogic((IView)AssociatedObject, ViewModelType, InjectedViewModel);
        }

        /// <summary>
        /// Called when the <see cref="MVVMBehaviorBase{TAttachedType,TLogicType}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// The behavior is initialized when the associated object is loaded because the <see cref="EventTarget"/>
        /// class requires loaded associated objects.
        /// </remarks>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Save))
            {
                _saveTargetInfo = new EventTarget(AssociatedObject, Save);
                _saveTargetInfo.Event += OnSaved;
            }

            if (!string.IsNullOrEmpty(SaveAndClose))
            {
                _saveAndCloseTargetInfo = new EventTarget(AssociatedObject, SaveAndClose);
                _saveAndCloseTargetInfo.Event += OnSavedAndClosed;
            }

            if (!string.IsNullOrEmpty(Cancel))
            {
                _cancelTargetInfo = new EventTarget(AssociatedObject, Cancel);
                _cancelTargetInfo.Event += OnCanceled;
            }

            if (!string.IsNullOrEmpty(CancelAndClose))
            {
                _cancelAndCloseTargetInfo = new EventTarget(AssociatedObject, CancelAndClose);
                _cancelAndCloseTargetInfo.Event += OnCanceledAndClosed;
            }

            if (!string.IsNullOrEmpty(Close))
            {
                _closeTargetInfo = new EventTarget(AssociatedObject, Close);
                _closeTargetInfo.Event += OnClosed;
            }
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        protected override void Uninitialize()
        {
            base.Uninitialize();

            if (_saveTargetInfo != null)
            {
                _saveTargetInfo.Event -= OnSaved;
                _saveTargetInfo.CleanUp();
                _saveTargetInfo = null;
            }

            if (_saveAndCloseTargetInfo != null)
            {
                _saveAndCloseTargetInfo.Event -= OnSavedAndClosed;
                _saveAndCloseTargetInfo.CleanUp();
                _saveAndCloseTargetInfo = null;
            }

            if (_cancelTargetInfo != null)
            {
                _cancelTargetInfo.Event -= OnCanceled;
                _cancelTargetInfo.CleanUp();
                _cancelTargetInfo = null;
            }

            if (_cancelAndCloseTargetInfo != null)
            {
                _cancelAndCloseTargetInfo.Event -= OnCanceledAndClosed;
                _cancelAndCloseTargetInfo.CleanUp();
                _cancelAndCloseTargetInfo = null;
            }

            if (_closeTargetInfo != null)
            {
                _closeTargetInfo.Event -= OnClosed;
                _closeTargetInfo.CleanUp();
                _closeTargetInfo = null;
            }
        }

        /// <summary>
        /// Called when the save event is invoked by a subscribed control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSaved(object sender, EventArgs e)
        {
            Logic.SaveViewModel();
        }

        /// <summary>
        /// Called when the save and close event is invoked by a subscribed control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSavedAndClosed(object sender, EventArgs e)
        {
            Logic.SaveAndCloseViewModel();
        }

        /// <summary>
        /// Called when the cancel event is invoked by a subscribed control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCanceled(object sender, EventArgs e)
        {
            Logic.CancelViewModel();
        }

        /// <summary>
        /// Called when the cancel and close event is invoked by a subscribed control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCanceledAndClosed(object sender, EventArgs e)
        {
            Logic.CancelAndCloseViewModel();
        }

        /// <summary>
        /// Called when the close event is invoked by a subscribed control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnClosed(object sender, EventArgs e)
        {
            Logic.CloseViewModel(null);
        }
        #endregion
    }
}
