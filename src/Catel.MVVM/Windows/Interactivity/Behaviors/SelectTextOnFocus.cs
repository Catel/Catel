namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using UIEventArgs = System.EventArgs;

    /// <summary>
    /// Selects all the text when the <see cref="TextBox"/> or <see cref="PasswordBox"/> is focused.
    /// </summary>
    public class SelectTextOnFocus : BehaviorBase<Control>
    {
        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.GotMouseCapture += OnGotMouseCapture;
            AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.GotMouseCapture -= OnGotMouseCapture;
            AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
        }

        /// <summary>
        /// Called when the <c>UIElement.OnGotFocus</c> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            SelectAllText();
        }

        /// <summary>
        /// Called when the <see cref="UIElement.PreviewMouseLeftButtonDown"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnPreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!AssociatedObject.IsKeyboardFocusWithin)
            {
                e.Handled = true;

                AssociatedObject.Focus();
            }
        }

        /// <summary>
        /// Called when the <see cref="UIElement.GotMouseCapture"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnGotMouseCapture(object sender, RoutedEventArgs e)
        {
            SelectAllText();
        }

        /// <summary>
        /// Called when the <see cref="UIElement.GotKeyboardFocus"/> event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            SelectAllText();
        }

        /// <summary>
        /// Selects all the text in the associated object.
        /// </summary>
        private void SelectAllText()
        {
            if (!IsEnabled)
            {
                return;
            }

            var textBox = AssociatedObject as TextBox;
            if (textBox is not null)
            {
                textBox.SelectAll();
            }

            var passwordBox = AssociatedObject as PasswordBox;
            if (passwordBox is not null)
            {
                passwordBox.SelectAll();
            }
        } 
    }
}
