// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollViewerProperties.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Container class for custom Attached properties for ScrollViewer.
    /// </summary>
    /// <remarks>
    /// Source : http://serialseb.blogspot.com/2007/09/wpf-tips-6-preventing-scrollviewer-from.html
    /// </remarks>
    public static class ScrollViewerProperties
    {
        #region Mousewheel fix
        /// <summary>
        /// Gets the fix scrolling.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetFixScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(FixScrollingProperty);
        }

        /// <summary>
        /// Sets the fix scrolling.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetFixScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(FixScrollingProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FixScrollingProperty =
            DependencyProperty.RegisterAttached("FixScrolling", typeof(bool), typeof(ScrollViewerProperties), new PropertyMetadata(false, ScrollViewerProperties.OnFixScrollingPropertyChanged));

        /// <summary>
        /// Called when [fix scrolling property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnFixScrollingPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Argument.IsOfType("sender", sender, typeof(ScrollViewer));

            var viewer = (ScrollViewer)sender;

            if ((bool)args.NewValue)
            {
                viewer.PreviewMouseWheel += HandlePreviewMouseWheel;
            }
            else if ((bool)args.NewValue == false)
            {
                viewer.PreviewMouseWheel -= HandlePreviewMouseWheel;
            }
        }

        private static readonly List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();

        /// <summary>
        /// Handles the preview mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseWheelEventArgs"/> instance containing the event data.</param>
        private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            var scrollControl = sender as ScrollViewer;
            if (scrollControl == null || args.Handled || _reentrantList.Contains(args))
            {
                return;
            }

            var originalSource = args.OriginalSource as UIElement;
            if (originalSource == null)
            {
                // For example, Paragraph is not a UI element
                return;
            }

            var previewEventArg = new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta)
            {
                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                Source = sender
            };

            _reentrantList.Add(previewEventArg);
            originalSource.RaiseEvent(previewEventArg);
            _reentrantList.Remove(previewEventArg);
            // at this point if no one else handled the event in our children, we do our job

            if (!previewEventArg.Handled && ((args.Delta > 0 && scrollControl.VerticalOffset == 0) || (args.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight)))
            {
                var parent = (UIElement)((FrameworkElement)sender).Parent;

                if (parent != null)
                {
                    var eventArg = new MouseWheelEventArgs(args.MouseDevice, args.Timestamp, args.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    parent.RaiseEvent(eventArg);
                    args.Handled = true;
                }
            }
        }
        #endregion
    }
}

#endif