// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowEventToCommandBehaviorBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Behavior class that catches an event from the root window element.
    /// <para />
    /// The event is forwarded to the DataContext of the <see cref="FrameworkElement"/> it is attached to.
    /// </summary>
    public class WindowEventToCommand : CommandBehaviorBase<FrameworkElement>
    {
        #region Fields
        /// <summary>
        /// Will be executed instead of the command if set.
        /// </summary>
        private readonly Action<Window> _action;

        /// <summary>
        /// Stores a reference to the window the event handler is registered on.
        /// <para />
        /// Will be used to deregister the event handler.
        /// </summary>
        private Window _currentWindow = null;

        private DynamicEventListener _dynamicEventListener;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowEventToCommand"/> class.
        /// </summary>
        protected WindowEventToCommand()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowEventToCommand"/> class.
        /// </summary>
        /// <param name="action">The action to execute on double click. This is very useful when the behavior is added
        /// via code and an action must be invoked instead of a command.</param>
        protected WindowEventToCommand(Action<Window> action)
        {
            _action = action;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the event to subscribe to.
        /// </summary>
        /// <value>The name of the event.</value>
        public string EventName { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the associated object is loaded.<br />
        /// <para />
        /// Registers the event subscription.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            UnsubscribeFromWindowEvent();
            SubscribeToWindowEvent();
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// Deregisters the event subscription.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
            UnsubscribeFromWindowEvent();
        }

        /// <summary>
        /// Registers the handler to the event of the current window.
        /// </summary>
        private void SubscribeToWindowEvent()
        {
            var window = Window.GetWindow(AssociatedObject);
            if (window == null)
            {
                return;
            }

            RegisterEventHandler(window);

            _currentWindow = window;
        }

        /// <summary>
        /// Removes the handler registration from the previous window.
        /// </summary>
        private void UnsubscribeFromWindowEvent()
        {
            if (_currentWindow == null)
            {
                return;
            }

            UnregisterEventHandler(_currentWindow);

            _currentWindow = null;
        }

        /// <summary>
        /// Registers the handler to the event of the given window.
        /// <code>
        /// protected override void RegisterEventHandler(Window window)
        /// {
        ///     window.Closing += WindowOnClosing;
        /// }
        /// 
        /// private void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        /// {
        ///     ExecuteCommand(sender);
        /// }
        /// </code>
        /// </summary>
        /// <param name="window">The window instance the eventhandler has to be registered to.</param>
        protected void RegisterEventHandler(Window window)
        {
            var dynamicEventListener = new DynamicEventListener(window, EventName);
            dynamicEventListener.EventOccurred += OnEventOccurred;
        }

        /// <summary>
        /// Unregisters the handler from the event of the given window.
        /// <code>
        /// protected override void RegisterEventHandler(Window window)
        /// {
        ///     window.Closed += WindowOnClosed;
        /// }
        /// 
        /// private void WindowOnClosed(object sender, EventArgs eventArgs)
        /// {
        ///     ExecuteCommand(sender);
        /// }
        /// </code>
        /// </summary>
        /// <param name="window">The window instance the eventhandler has to be unregistered from.</param>
        protected void UnregisterEventHandler(Window window)
        {
            if (_dynamicEventListener != null)
            {
                _dynamicEventListener.EventOccurred -= OnEventOccurred;
                _dynamicEventListener.UnsubscribeFromEvent();
                _dynamicEventListener = null;
            }
        }

        /// <summary>
        /// Invokes the Action or executes the Command.
        /// <para />
        /// The current window instance is used as parameter.
        /// </summary>
        protected override void ExecuteCommand()
        {
            ExecuteCommand(_currentWindow);
        }

        /// <summary>
        /// Invokes the Action or executes the Command.
        /// The given window instance is used as parameter.
        /// </summary>
        protected override void ExecuteCommand(object parameter)
        {
            var window = parameter as Window;
            if (_action != null)
            {
                _action(window);
            }
            else
            {
                base.ExecuteCommand(window);
            }
        }

        /// <summary>
        /// Called when the event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <remarks>
        /// This method is public to allow the WeakEventListener to support this in Silverlight.
        /// </remarks>
        public void OnEventOccurred(object sender, EventArgs e)
        {
            ExecuteCommand();
        }
        #endregion
    }
}

#endif