// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Tests
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Window helper class.
    /// </summary>
    internal static class WindowHelper
    {
        #region Methods
        /// <summary>
        /// Shows a control in a window.
        /// </summary>
        /// <param name="control">Control to show in a window.</param>
        public static void ShowControlInWindow(UIElement control)
        {
            // Create window
            var window = new Window();

            // Create a stack panel
            var stackPanel = new StackPanel();
            stackPanel.Children.Add(control);

            // Set as window content
            window.Content = stackPanel;

            // Set title
            window.Title = "Test window";

            // Show window
            window.Show();
        }
        #endregion
    }
}

#endif
