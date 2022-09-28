namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using UIEventArgs = System.EventArgs;

    /// <summary>
    /// Behavior that converts a key press on a specific UI element to a command.
    /// </summary>
    public class KeyPressToCommand : CommandBehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the key to which the behavior should respond.
        /// </summary>
        /// <value>The key.</value>
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(nameof(Key), typeof(Key), 
            typeof(KeyPressToCommand), new PropertyMetadata(Key.None));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            AssociatedObject.KeyDown += OnKeyDown;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.KeyDown -= OnKeyDown;

            base.OnAssociatedObjectUnloaded();
        }

        /// <summary>
        /// Called when the specified key is pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The key event args instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (e.Handled)
            {
                return;
            }

            if (e.Key == Key)
            {
                if (CanExecuteCommand())
                {
                    ExecuteCommand();

                    e.Handled = true;
                }
            }
        }
    }
}
