namespace Catel.Windows
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

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
            ArgumentNullException.ThrowIfNull(element);

            element.SnapsToDevicePixels = true;
            element.UseLayoutRounding = true;
        }

        /// <summary>
        /// Determines whether the specified element is visible.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if the specified element is visible; otherwise, <c>false</c>.</returns>
        public static bool IsVisible(this FrameworkElement element)
        {
            ArgumentNullException.ThrowIfNull(element);

            return element.IsVisible;
        }

        /// <summary>
        /// Determines whether the framework element is currently visible to the user.
        /// </summary>
        /// <returns><c>true</c> if the framework element is currently visible to the user; otherwise, <c>false</c>.</returns>
        public static bool IsVisibleToUser(this FrameworkElement element)
        {
            ArgumentNullException.ThrowIfNull(element);

            var container = GetRelevantParent<FrameworkElement>(element);
            if (container is not null)
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
            ArgumentNullException.ThrowIfNull(element);
            Argument.IsNotNull("container", container);

            if (!container.IsVisible() || !element.IsVisible())
            {
                return false;
            }

            var transform = element.TransformToAncestor(container);
            var bounds = transform.TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            var topLeft = bounds.TopLeft;
            var bottomRight = bounds.BottomRight;

            return rect.Contains(topLeft) || rect.Contains(bottomRight);
        }

        /// <summary>
        /// Gets the relevant parent which is either a content presenter or panel.
        /// </summary>
        /// <typeparam name="T">Type of the relevant parent</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The relevant parent.</returns>
        private static FrameworkElement GetRelevantParent<T>(FrameworkElement obj)
            where T : FrameworkElement
        {
            var container = VisualTreeHelper.GetParent(obj) as FrameworkElement;

            var contentPresenter = container as ContentPresenter;
            if (contentPresenter is not null)
            {
                container = GetRelevantParent<T>(contentPresenter);
            }

            var panel = container as Panel;
            if (panel is not null)
            {
                container = GetRelevantParent<ScrollViewer>(panel);
            }

            if (!(container is T) && (container is not null))
            {
                container = GetRelevantParent<T>(container);
            }

            return container;
        }
    }
}
