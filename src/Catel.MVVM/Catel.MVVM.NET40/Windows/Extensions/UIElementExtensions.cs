// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIElementExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;

    /// <summary>
    /// Extensions for <see cref="UIElement"/>.
    /// </summary>
    public static class UIElementExtensions
    {
        #region Win32 imports
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
        #endregion

        #region Methods
        /// <summary>
        /// Gets the focused control.
        /// </summary>
        /// <param name="element">The element to check and all childs.</param>
        /// <returns>The focused <see cref="UIElement"/> or <c>null</c> if none if the children has the focus.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static UIElement GetFocusedControl(this UIElement element)
        {
            Argument.IsNotNull("element", element);

            return element.FindVisualDescendant(obj =>
            {
                var objAsUIElement = obj as UIElement;
                if (objAsUIElement != null)
                {
                    return objAsUIElement.IsFocused;
                }

                return false;
            }) as UIElement;
        }

        /// <summary>
        /// Focuses the first control on the ContentElement.
        /// </summary>
        /// <param name="element">Reference to the current <see cref="ContentElement"/>.</param>
        /// <param name="focusParentsFirst">if set to <c>true</c>, the parents are focused first.</param>
        public static void FocusFirstControl(this ContentElement element, bool focusParentsFirst = true)
        {
            FocusFirstControl((object)element, focusParentsFirst);
        }

        /// <summary>
        /// Focuses the first control on the UI Element.
        /// </summary>
        /// <param name="element">Reference to the current <see cref="UIElement"/>.</param>
        /// <param name="focusParentsFirst">if set to <c>true</c>, the parents are focused first.</param>
        public static void FocusFirstControl(this UIElement element, bool focusParentsFirst = true)
        {
            FocusFirstControl((object)element, focusParentsFirst);
        }

        /// <summary>
        /// Focuses the first control on the UI Element.
        /// </summary>
        /// <param name="element">Reference to the current element.</param>
        /// <param name="focusParentsFirst">if set to <c>true</c>, the parents are focused first.</param>
        private static void FocusFirstControl(object element, bool focusParentsFirst)
        {
            var elementAsFrameworkElement = element as FrameworkElement;
            if (elementAsFrameworkElement != null)
            {
                if (elementAsFrameworkElement.IsLoaded)
                {
                    FocusNextControl(elementAsFrameworkElement, focusParentsFirst);
                }
                else
                {
                    // Get handler (so we can nicely unsubscribe)
                    RoutedEventHandler onFrameworkElementLoaded = null;
                    onFrameworkElementLoaded = delegate
                    {
                        FocusNextControl(elementAsFrameworkElement, focusParentsFirst);
                        elementAsFrameworkElement.Loaded -= onFrameworkElementLoaded;
                    };

                    elementAsFrameworkElement.Loaded += onFrameworkElementLoaded;
                }
            }
            else
            {
                FocusFirstControl(element, focusParentsFirst);
            }
        }

        /// <summary>
        /// Focuses the next control on the UI Element.
        /// </summary>
        /// <param name="element">Element to focus the next control of.</param>
        /// <param name="focusParentsFirst">if set to <c>true</c>, the parents are focused first.</param>
        private static void FocusNextControl(object element, bool focusParentsFirst)
        {
            var elementAsFrameworkElement = element as FrameworkElement;
            if (elementAsFrameworkElement != null)
            {
                if (focusParentsFirst)
                {
                    var parentsToFocus = new Stack<FrameworkElement>();
                    var parent = elementAsFrameworkElement.Parent as FrameworkElement;
                    while (parent != null)
                    {
                        if (parent.Focusable)
                        {
                            parentsToFocus.Push(parent);
                        }

                        parent = parent.Parent as FrameworkElement;
                    }

                    while (parentsToFocus.Count > 0)
                    {
                        FrameworkElement parentToFocus = parentsToFocus.Pop();
                        parentToFocus.Focus();
                    }
                }
            }

            var uiElement = element as UIElement;
            var contentElement = element as ContentElement;

            if (uiElement != null)
            {
                // Focus element itself
                if (uiElement.Focusable)
                {
                    uiElement.Focus();
                }
            }
            else if (contentElement != null)
            {
                // Focus content element
                if (contentElement.Focusable)
                {
                    contentElement.Focus();
                }
            }

            MoveFocus(element, FocusNavigationDirection.Next, 1);
        }

        /// <summary>
        /// Moves the focus in a specific direction.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="hops">The hops.</param>
        public static void MoveFocus(this IInputElement element, FocusNavigationDirection direction, int hops)
        {
            MoveFocus((object)element, direction, hops);
        }

        /// <summary>
        /// Moves the focus in a specific direction.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="hops">The hops.</param>
        public static void MoveFocus(this UIElement element, FocusNavigationDirection direction, int hops)
        {
            MoveFocus((object)element, direction, hops);
        }

        /// <summary>
        /// Moves the focus in a specific direction.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="hops">The hops.</param>
        public static void MoveFocus(this ContentElement element, FocusNavigationDirection direction, int hops)
        {
            MoveFocus((object)element, direction, hops);
        }

        /// <summary>
        /// Moves the focus in a specific direction.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="hops">The hops.</param>
        private static void MoveFocus(object element, FocusNavigationDirection direction, int hops)
        {
            if (hops <= 0)
            {
                return;
            }

            var frameworkElement = element as FrameworkElement;
            bool delayMove = ((frameworkElement != null) && !frameworkElement.IsLoaded);

            if (delayMove)
            {
                RoutedEventHandler onFrameworkElementLoaded = null;
                onFrameworkElementLoaded = delegate
                {
                    MoveFocus((object)frameworkElement, direction, hops);
                    frameworkElement.Loaded -= onFrameworkElementLoaded;
                };

                frameworkElement.Loaded += onFrameworkElementLoaded;
            }
            else
            {
                var uiElement = element as UIElement;
                var contentElement = element as ContentElement;

                if (uiElement != null)
                {
                    // Focus next
                    uiElement.MoveFocus(new TraversalRequest(direction));
                }
                else if (contentElement != null)
                {
                    // Focus next
                    contentElement.MoveFocus(new TraversalRequest(direction));
                }

                if (hops > 1)
                {
                    MoveFocus(Keyboard.FocusedElement, direction, hops - 1);
                }
            }
        }

        /// <summary>
        /// Invalidates the rect as it is possible in win32.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        public static void InvalidateRect(this UIElement element)
        {
            Argument.IsNotNull("element", element);

            var hwndSource = PresentationSource.FromVisual(element) as HwndSource;
            if (hwndSource == null)
            {
                return;
            }

            InvalidateRect(hwndSource.Handle, IntPtr.Zero, true);
        }
        #endregion
    }
}
