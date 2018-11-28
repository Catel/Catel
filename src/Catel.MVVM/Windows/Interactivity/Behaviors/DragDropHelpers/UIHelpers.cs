// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIHelpers.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Interactivity.DragDropHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class UIHelpers
    {

        public enum RelativeVerticalMousePosition
        {
            Middle,
            Top,
            Bottom
        }

        public static RelativeVerticalMousePosition GetRelativeVerticalMousePosition(FrameworkElement elm, Point pt)
        {
            if (pt.Y > 0.0 && pt.Y < 25)
            {
                return RelativeVerticalMousePosition.Top;
            }
            if (pt.Y > elm.ActualHeight - 25 && pt.Y < elm.ActualHeight)
            {
                return RelativeVerticalMousePosition.Top;
            }
            return RelativeVerticalMousePosition.Middle;
        }

        public static object GetItemFromPointInItemsControl(ItemsControl parent, Point p)
        {
            var element = parent.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == parent)
                    return null;

                var data = parent.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return data;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static UIElement GetItemContainerFromPointInItemsControl(ItemsControl parent, Point p)
        {
            var element = parent.InputHitTest(p) as UIElement;
            while (element != null)
            {
                if (element == parent)
                    return null;

                object data = parent.ItemContainerGenerator.ItemFromContainer(element);
                if (data != DependencyProperty.UnsetValue)
                {
                    return element;
                }
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        public static T GetVisualDescendent<T>(DependencyObject d) 
            where T : DependencyObject
        {
            return GetVisualDescendents<T>(d).FirstOrDefault();
        }

        private static IEnumerable<T> GetVisualDescendents<T>(DependencyObject d) 
            where T : DependencyObject
        {
            for (var n = 0; n < VisualTreeHelper.GetChildrenCount(d); n++)
            {
                var child = VisualTreeHelper.GetChild(d, n);

                if (child is T)
                {
                    yield return (T)child;
                }

                foreach (var match in GetVisualDescendents<T>(child))
                {
                    yield return match;
                }
            }
        }

    }
}

#endif
