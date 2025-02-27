﻿namespace Catel.Windows.Interactivity
{
    using System.Windows.Input;
    using Catel.Windows.Input;
    using System.Windows;

    /// <summary>
    /// Trigger base class that handles a safe unsubscribe and clean up because the default
    /// Trigger class does not always call <see cref="TriggerBase{T}.OnDetaching"/>.
    /// <para />
    /// This class extends the <see cref="TriggerBase{T}"/> class by adding supports for commands.
    /// </summary>
    /// <typeparam name="T">The <see cref="FrameworkElement"/> this behavior should attach to.</typeparam>
    public abstract class CommandTriggerBase<T> : TriggerBase<T>
        where T : FrameworkElement
    {
        private ICommand? _command;
        private object? _commandParameter;
        private bool _isSubscribed;

        /// <summary>
        /// Gets or sets the modifiers to check for.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Modifiers.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register(nameof(Modifiers), typeof(ModifierKeys), typeof(CommandTriggerBase<T>),
            new PropertyMetadata(ModifierKeys.None));

        /// <summary>
        /// Gets or sets the command to execute when the key is pressed.
        /// </summary>
        /// <value>The command.</value>
        public ICommand? Command
        {
            get { return (ICommand?)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CommandTriggerBase<T>),
            new PropertyMetadata(null, (sender, e) => ((CommandTriggerBase<T>)sender).OnCommandChangedInternal(e.NewValue as ICommand)));

        /// <summary>
        /// Gets or sets the command parameter, which will override the parameter defined in the direct command binding.
        /// </summary>
        /// <value>The command parameter.</value>
        public object? CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// The property definition for the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(CommandTriggerBase<T>),
            new PropertyMetadata(null, (sender, e) => ((CommandTriggerBase<T>)sender).OnCommandParameterChangedInternal(e.NewValue)));

        /// <summary>
        /// Called when the <see cref="ICommand.CanExecute"/> state has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCommandCanExecuteChangedInternal(object? sender, System.EventArgs e)
        {
            OnCommandCanExecuteChanged();
        }

        /// <summary>
        /// Called when the <see cref="ICommand.CanExecute"/> state has changed.
        /// </summary>
        protected virtual void OnCommandCanExecuteChanged()
        {
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            UpdateCommandSubscriptions();
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            UnsubscribeFromCommand();

            base.OnAssociatedObjectUnloaded();
        }

        /// <summary>
        /// Subscribes to the command.
        /// </summary>
        private void SubscribeToCommand()
        {
            if (_isSubscribed)
            {
                return;
            }

            var command = _command;
            if (command is null)
            {
                return;
            }

            command.CanExecuteChanged += OnCommandCanExecuteChangedInternal;

            _isSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes from the command.
        /// </summary>
        private void UnsubscribeFromCommand()
        {
            if (!_isSubscribed)
            {
                return;
            }

            var command = _command;
            if (command is not null)
            {
                command.CanExecuteChanged -= OnCommandCanExecuteChangedInternal;
            }

            _isSubscribed = false;
        }

        /// <summary>
        /// Called when the <see cref="Command"/> property has changed.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnCommandChangedInternal(ICommand? newValue)
        {
            UpdateCommandSubscriptions();
        }

        private void UpdateCommandSubscriptions()
        {
            var oldCommand = _command;
            var newCommand = Command;

            if (ReferenceEquals(oldCommand, newCommand))
            {
                return;
            }

            UnsubscribeFromCommand();

            _command = newCommand;

            SubscribeToCommand();

            OnCommandChanged();
            OnCommandCanExecuteChanged();
        }

        /// <summary>
        /// Invoked when the <see cref="Command"/> property has changed.
        /// </summary>
        protected virtual void OnCommandChanged()
        {
        }

        /// <summary>
        /// Called when the <see cref="CommandParameter"/> property has changed.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        private void OnCommandParameterChangedInternal(object? newValue)
        {
            _commandParameter = newValue;

            OnCommandParameterChanged();
        }

        /// <summary>
        /// Invoked when the <see cref="CommandParameter"/> property has changed.
        /// </summary>
        protected virtual void OnCommandParameterChanged()
        {
        }

        /// <summary>
        /// Determines whether the command can be invoked. It does this by checking both the <see cref="Modifiers"/> and
        /// the command itself.
        /// </summary>
        /// <returns><c>true</c> if the command can be invoked; otherwise, <c>false</c>.</returns>
        protected virtual bool CanExecuteCommand()
        {
            return CanExecuteCommand(CommandParameter);
        }

        /// <summary>
        /// Determines whether the command can be invoked. It does this by checking both the <see cref="Modifiers"/> and
        /// the command itself.
        /// <para />
        /// If the <see cref="CommandParameter"/> should be used, use the <see cref="CanExecuteCommand()"/> instead.
        /// </summary>
        /// <returns><c>true</c> if the command can be invoked; otherwise, <c>false</c>.</returns>
        protected virtual bool CanExecuteCommand(object? parameter)
        {
            var command = _command;
            if (command is null)
            {
                return false;
            }

            if (!IsEnabled)
            {
                return false;
            }

            return command.CanExecute(parameter);
        }

        /// <summary>
        /// Invokes the command with the specified parameter.
        /// </summary>
        protected virtual void ExecuteCommand()
        {
            ExecuteCommand(_commandParameter);
        }

        /// <summary>
        /// Invokes the command with the overridden parameter.
        /// <para />
        /// If the <see cref="CommandParameter"/> should be used, use the <see cref="ExecuteCommand()"/> instead.
        /// </summary>
        /// <param name="parameter">The parameter that will override the <see cref="CommandParameter"/>.</param>
        protected virtual void ExecuteCommand(object? parameter)
        {
            // CTL-638
            if (Modifiers != ModifierKeys.None)
            {
                if (!KeyboardHelper.AreKeyboardModifiersPressed(Modifiers))
                {
                    return;
                }
            }

            if (CanExecuteCommand(parameter))
            {
                _command?.Execute(parameter);
            }
        }
    }
}
