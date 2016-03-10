// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIElementExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



#if !XAMARIN

namespace Catel.Windows
{
#if NETFX_CORE
    using global::Windows.Foundation;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Extensions for <see cref="FrameworkElement"/>.
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
        /// <summary>
        /// Fixes the blurriness in WPF by setting both <c>SnapsToDevicePixels</c> and
        /// <c>UseLayoutRounding</c> to <c>true</c>.
        /// </summary>
        /// <param name="element">The UI element.</param>
        public static void FixBlurriness(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

#if NET
            element.SnapsToDevicePixels = true;
#endif

            element.UseLayoutRounding = true;
        }

        /// <summary>
        /// Determines whether the specified element is visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the specified element is visible; otherwise, <c>false</c>.</returns>
        public static bool IsVisible(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

#if NETFX_CORE || SILVERLIGHT
            return element.Visibility == Visibility.Visible;
#else
            return element.IsVisible;
#endif
        }

        /// <summary>
        /// Determines whether the framework element is currently visible to the user.
        /// </summary>
        /// <returns><c>true</c> if the framework element is currently visible to the user; otherwise, <c>false</c>.</returns>
        public static bool IsVisibleToUser(this FrameworkElement element)
        {
            Argument.IsNotNull("element", element);

            var container = element.GetRelevantParent<FrameworkElement>();
            if (container != null)
            {
                var visible = element.IsVisibleToUser(container);
                if (!visible)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified element is currently visible to the user.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="container">The container.</param>
        /// <returns><c>true</c> if if the specified element is currently visible to the user; otherwise, <c>false</c>.</returns>
        public static bool IsVisibleToUser(this FrameworkElement element, FrameworkElement container)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNull("container", container);

            if (!container.IsVisible() || !element.IsVisible())
            {
                return false;
            }

#if NETFX_CORE || SILVERLIGHT
            var transform = element.TransformToVisual(container);
#else
            var transform = element.TransformToAncestor(container);
#endif

            var bounds = transform.TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);

#if NETFX_CORE || SILVERLIGHT
            var topLeft = new Point(bounds.Left, bounds.Top);
            var bottomRight = new Point(bounds.Right, bounds.Bottom);
#else
            var topLeft = bounds.TopLeft;
            var bottomRight = bounds.BottomRight;
#endif

            return rect.Contains(topLeft) || rect.Contains(bottomRight);
        }

        /// <summary>
        /// Gets the relevant parent.
        /// </summary>
        /// <typeparam name="T">Type of the relevant parent</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The relevant parent.</returns>
        private static FrameworkElement GetRelevantParent<T>(this FrameworkElement obj)
            where T : FrameworkElement
        {
            var container = VisualTreeHelper.GetParent(obj) as FrameworkElement;

            var contentPresenter = container as ContentPresenter;
            if (contentPresenter != null)
            {
                container = GetRelevantParent<T>(contentPresenter);
            }

            var panel = container as Panel;
            if (panel != null)
            {
                container = GetRelevantParent<ScrollViewer>(panel);
            }

            if (!(container is T) && (container != null))
            {
                container = GetRelevantParent<T>(container);
            }

            return container;
        }
    }
}

#endif