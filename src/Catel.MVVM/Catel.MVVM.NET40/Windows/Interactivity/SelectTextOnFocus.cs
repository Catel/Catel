// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectTextOnFocus.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    
#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Selects all the text when the <see cref="TextBox"/> is focused.
    /// </summary>
    public class SelectTextOnFocus : BehaviorBase<TextBox>
    {
        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, UIEventArgs e)
        {
            AssociatedObject.GotFocus += OnGotFocus;

#if NET
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.GotMouseCapture += OnGotMouseCapture;
            AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
#endif
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, UIEventArgs e)
        {
            AssociatedObject.GotFocus -= OnGotFocus;

#if NET
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.GotMouseCapture -= OnGotMouseCapture;
            AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
#endif
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

#if NET
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
#endif

        /// <summary>
        /// Selects all the text in the associated object.
        /// </summary>
        private void SelectAllText()
        {
            AssociatedObject.SelectAll();
        } 
    }
}
