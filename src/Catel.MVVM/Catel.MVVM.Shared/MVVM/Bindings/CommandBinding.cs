// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandBinding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using System.Reflection;
    using Logging;
    using System;
    using Reflection;

    /// <summary>
    /// Binding to bind events to commands.
    /// </summary>
    public class CommandBinding : BindingBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private object _element;
        private EventInfo _eventInfo;
        private PropertyInfo _enabledPropertyInfo;
        private ICatelCommand _command;
        private Binding _commandParameterBinding;

        private EventHandler _eventHandler;
        private EventHandler _canExecuteChangedHandler;
        private EventHandler<EventArgs> _commandBindingParameterValueChangedHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBinding"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="command">The command.</param>
        /// <param name="commandParameterBinding">The command parameter binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="eventName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public CommandBinding(object element, string eventName, ICatelCommand command, Binding commandParameterBinding = null)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNullOrWhitespace("eventName", eventName);
            Argument.IsNotNull("command", command);

            _element = element;
            _command = command;
            _commandParameterBinding = commandParameterBinding;

            var elementType = _element.GetType();
            _eventInfo = elementType.GetEventEx(eventName);
            if (_eventInfo == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Event '{0}.{1}' not found, cannot create command binding", elementType.Name, eventName);
            }

            _enabledPropertyInfo = elementType.GetPropertyEx("Enabled");

            _eventHandler = delegate
            {
                var commandParameter = _commandParameterBinding.GetBindingValue();
                if (_command.CanExecute(commandParameter))
                {
                    _command.Execute(commandParameter);
                }
            };
            _eventInfo.AddEventHandler(element, _eventHandler);

            _canExecuteChangedHandler = (sender, e) => UpdateEnabledState();
            command.CanExecuteChanged += _canExecuteChangedHandler;

            if (commandParameterBinding != null)
            {
                _commandBindingParameterValueChangedHandler = (sender, e) => UpdateEnabledState();
                commandParameterBinding.ValueChanged += _commandBindingParameterValueChangedHandler;
            }

            UpdateEnabledState();
        }

        #region Methods
        /// <summary>
        /// Determines the value to use in the <see cref="BindingBase.ToString"/> method.
        /// </summary>
        /// <returns>The string to use.</returns>
        protected override string DetermineToString()
        {
            var elementType = _element.GetType();

            return string.Format("{0}.{1}", elementType.Name, _eventInfo.Name);
        }

        /// <summary>
        /// Uninitializes this binding.
        /// </summary>
        protected override void Uninitialize()
        {
            if (_eventHandler != null)
            {
                _eventInfo.RemoveEventHandler(_element, _eventHandler);
                _eventHandler = null;
            }

            if (_canExecuteChangedHandler != null)
            {
                _command.CanExecuteChanged -= _canExecuteChangedHandler;
                _canExecuteChangedHandler = null;
            }

            if (_commandBindingParameterValueChangedHandler != null)
            {
                _commandParameterBinding.ValueChanged -= _commandBindingParameterValueChangedHandler;
                _commandBindingParameterValueChangedHandler = null;
            }

            _element = null;
            _eventInfo = null;
            _enabledPropertyInfo = null;
            _command = null;
            _commandParameterBinding = null;

            // TODO: call commandParameterBinding.ClearBinding();?
        }

        private void UpdateEnabledState()
        {
            if (_enabledPropertyInfo == null)
            {
                return;
            }

            var commandParameter = _commandParameterBinding.GetBindingValue();
            _enabledPropertyInfo.SetValue(_element, _command.CanExecute(commandParameter));
        }
        #endregion
    }
}

#endif