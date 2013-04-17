// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyPressToCommand.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Behavior that converts a key press on a specific UI element to a command.
    /// </summary>
    public class KeyPressToCommand : CommandBehaviorBase<FrameworkElement>
    {
        #region Properties
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
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(KeyPressToCommand), new PropertyMetadata(Key.Enter));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, System.EventArgs e)
        {
            base.OnAssociatedObjectLoaded(sender, e);

            AssociatedObject.KeyUp += OnKeyUp;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, System.EventArgs e)
        {
            AssociatedObject.KeyUp -= OnKeyUp;

            base.OnAssociatedObjectUnloaded(sender, e);
        }

        /// <summary>
        /// Called when the specified key is pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key)
            {
                if (CanExecuteCommand())
                {
                    ExecuteCommand();

#if NET
                    e.Handled = true;
#endif
                }
            }
        }
        #endregion
    }
}
