// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventToCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Interactivity
{
    using System;
    using MVVM.Converters;
#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#elif NETCORE
    using System.Windows;
    using Microsoft.Xaml.Behaviors;
    using UIEventArgs = System.EventArgs;
#else
    using System.Windows;
    using System.Windows.Interactivity;
    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Class to convert a routed event to a command.
    /// </summary>
    public class EventToCommand : CommandTriggerActionBase<FrameworkElement>
    {
        #region Fields
        private bool? _disableAssociatedObjectOnCannotExecute;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EventToCommand"/> class.
        /// </summary>
        public EventToCommand()
        {
            _disableAssociatedObjectOnCannotExecute = true;

            PreventInvocationIfAssociatedObjectIsDisabled = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EventArgs"/> passed to the event handler
        /// should be passed to the command as well.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="EventArgs"/> passed to the event handler should be passed to the command; otherwise, <c>false</c>.
        /// </value>
        public bool PassEventArgsToCommand { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="IEventArgsConverter"/> that will convert an <see cref="EventArgs"/> object to a neutral object for consumption.
        /// </summary>
        /// <value>
        /// Converter that implements <see cref="IEventArgsConverter"/> that will be passed should <see cref="PassEventArgsToCommand"/> be <c>true</c>; otherwise <c>false</c>.
        /// </value>
        public IEventArgsConverter EventArgsConverter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the invocation of the command should be prevented when the
        /// <see cref="TriggerAction{T}.AssociatedObject"/> is disabled.
        /// <para />
        /// By default, this value is <c>true.</c>
        /// <para />
        /// This property is introduced to disable the default behavior. For example, when showing a window which will disable
        /// the underlying object, the command will no longer be invoked. While this is the recommended behavior in most cases,
        /// sometimes you just need to bypass the default functionality.
        /// </summary>
        /// <value><c>true</c> if the invocation of the command should be prevented when the <see cref="TriggerAction{T}.AssociatedObject"/> is disabled; otherwise, <c>false</c>.</value>
        public bool PreventInvocationIfAssociatedObjectIsDisabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the associated object should be disabled when the command
        /// cannot be executed.
        /// </summary>
        /// <remarks>
        /// Wrapper for the DisableAssociatedObjectOnCannotExecute dependency property.
        /// </remarks>
        public bool DisableAssociatedObjectOnCannotExecute
        {
            get { return (bool)GetValue(DisableAssociatedObjectOnCannotExecuteProperty); }
            set { SetValue(DisableAssociatedObjectOnCannotExecuteProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for DisableAssociatedObjectOnCannotExecute.
        /// </summary>
        public static readonly DependencyProperty DisableAssociatedObjectOnCannotExecuteProperty =
            DependencyProperty.Register("DisableAssociatedObjectOnCannotExecute", typeof(bool), typeof(EventToCommand), new PropertyMetadata(true,
                (sender, e) => ((EventToCommand)sender).OnDisableAssociatedObjectOnCannotExecuteChanged((bool)e.NewValue)));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the <see cref="DisableAssociatedObjectOnCannotExecute"/> property has changed.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnDisableAssociatedObjectOnCannotExecuteChanged(bool newValue)
        {
            _disableAssociatedObjectOnCannotExecute = newValue;

            UpdateElementState();
        }

        /// <summary>
        /// Called when the <c>CanExecute</c> state of a command has changed.
        /// </summary>
        protected override void OnCommandCanExecuteChanged()
        {
            UpdateElementState();
        }

        /// <summary>
        /// Invokes the action without any parameter.
        /// </summary>
        public void Invoke()
        {
            Invoke(null);
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the Action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            if (PreventInvocationIfAssociatedObjectIsDisabled && IsAssociatedObjectDisabled())
            {
                return;
            }

            var commandParameter = CommandParameter;
            if ((commandParameter is null) && PassEventArgsToCommand)
            {
                commandParameter = EventArgsConverter != null ? EventArgsConverter.Convert(AssociatedObject, parameter) : parameter;
            }

            ExecuteCommand(commandParameter);
        }

        /// <summary>
        /// Checks whether the associated object is disabled or not.
        /// </summary>
        /// <returns><c>true</c> if the associated object is disabled; otherwise <c>false</c>.</returns>
        private bool IsAssociatedObjectDisabled()
        {
            return ((AssociatedObject != null) && !AssociatedObject.IsEnabled);
        }

        /// <summary>
        /// Updates the state of the associated element.
        /// </summary>
        private void UpdateElementState()
        {
            if ((AssociatedObject is null) || (Command is null))
            {
                return;
            }

            if (!_disableAssociatedObjectOnCannotExecute.HasValue || !_disableAssociatedObjectOnCannotExecute.Value)
            {
                return;
            }

            var isEnabled = CanExecuteCommand();

            AssociatedObject.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            UpdateElementState();
        }
        #endregion
    }
}

#endif
