// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyPressToCommand.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    
#if NETFX_CORE
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Input;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using UIEventArgs = System.EventArgs;
#endif

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
        protected override void OnAssociatedObjectLoaded(object sender, UIEventArgs e)
        {
            base.OnAssociatedObjectLoaded(sender, e);

            AssociatedObject.KeyUp += OnKeyUp;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, UIEventArgs e)
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
