// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
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
#if SILVERLIGHT
    // Create window
            ChildWindow window = new ChildWindow();

            // Set as window content
            window.Content = control;
#else
            // Create window
            Window window = new Window();

            // Create a stack panel
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(control);

            // Set as window content
            window.Content = stackPanel;
#endif

            // Set title
            window.Title = "Test window";

            // Show window
            window.Show();
        }
        #endregion
    }
}