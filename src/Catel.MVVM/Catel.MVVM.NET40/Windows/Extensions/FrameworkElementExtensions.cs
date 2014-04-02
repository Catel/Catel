// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Logging;

#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Media;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using VisualStateGroup = global::Windows.UI.Xaml.VisualStateGroup;
#else
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    using VisualStateGroup = System.Object;
#endif

#if NET
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;
    using System.Windows.Documents;
#endif

#if SILVERLIGHT
    using Reflection;
    using System.Threading;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Extensions for <see cref="System.Windows.FrameworkElement"/>.
    /// </summary>
    public static class FrameworkElementExtensions
    {
#if NET
        #region Win32
        [DllImport("user32.dll", SetLastError = true)]
		static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool BringWindowToTop(HandleRef hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		/// <summary>
		/// The GetForegroundWindow function returns a handle to the foreground window.
		/// </summary>
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();

		[DllImport("kernel32.dll")]
		static extern IntPtr GetCurrentThreadId();

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

		[DllImport("user32.dll")]
		static extern IntPtr GetLastActivePopup(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern IntPtr SetActiveWindow(IntPtr hWnd);
        #endregion
#endif

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        /// <summary>
        /// Transfers the styles and transitions from two framework elements.
        /// </summary>
        /// <param name="oldElement">The old element.</param>
        /// <param name="newElement">The new element.</param>
        public static void TransferStylesAndTransitions(this FrameworkElement oldElement, FrameworkElement newElement)
        {
            Argument.IsNotNull("oldElement", oldElement);
            Argument.IsNotNull("newElement", newElement);

            Log.Debug("Transferring styles and transitions");

            var name = oldElement.Name;
            var renderTransform = oldElement.RenderTransform;
            var renderTransformOrigin = oldElement.RenderTransformOrigin;

            oldElement.RenderTransform = null;
            oldElement.Name = "__dynamicReplacement";

            newElement.Name = name;
            newElement.RenderTransform = renderTransform;
            newElement.RenderTransformOrigin = renderTransformOrigin;

            var customVisualStateManager = VisualStateManager.GetCustomVisualStateManager(oldElement);
            if (customVisualStateManager != null)
            {
                VisualStateManager.SetCustomVisualStateManager(oldElement, null);
                VisualStateManager.SetCustomVisualStateManager(newElement, customVisualStateManager);
            }

            var oldContentVisualStateGroups = VisualStateManager.GetVisualStateGroups(oldElement);
            if (oldContentVisualStateGroups.Count > 0)
            {
                // Copy to temp list, then clear, then add them to new parent
                var tempList = new List<VisualStateGroup>(oldContentVisualStateGroups.Cast<VisualStateGroup>());

                oldContentVisualStateGroups.Clear();

                var newContentVisualStateGroups = VisualStateManager.GetVisualStateGroups(newElement);
                foreach (var visualStateGroup in tempList)
                {
                    newContentVisualStateGroups.Add(visualStateGroup);
                }
            }

            Log.Debug("Transferred styles and transitions");
        }


        /// <summary>
        /// Hides the validation adorner.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        public static void HideValidationAdorner(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                return;
            }

#if NET
            frameworkElement.ApplyTemplate();

            var adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
            if (adornerLayer != null)
            {
                adornerLayer.Visibility = Visibility.Collapsed;
            }

            Validation.SetValidationAdornerSite(frameworkElement, null);
            Validation.SetErrorTemplate(frameworkElement, null);
#endif
        }

#if NET
		/// <summary>
		/// Activates the window this framework element contains to.
		/// </summary>
		/// <param name="frameworkElement">Reference to the current <see cref="FrameworkElement"/>.</param>
		public static void BringWindowToTop(this FrameworkElement frameworkElement)
		{
			if (frameworkElement == null)
			{
			    return;
			}

			// Check if the framework element has an owner
			Window ownerWindow = null;
			FrameworkElement parentFrameworkElement = frameworkElement;
			while (parentFrameworkElement != null)
			{
			    ownerWindow = parentFrameworkElement as Window;
                if (ownerWindow != null)
                {
                    break;
                }

				parentFrameworkElement = parentFrameworkElement.Parent as FrameworkElement;
			}

			try
			{
				// Get the handle (of the window or process)
				IntPtr windowHandle = (ownerWindow != null) ? new WindowInteropHelper(ownerWindow).Handle : Process.GetCurrentProcess().MainWindowHandle;
				if (windowHandle != IntPtr.Zero)
				{
					SetForegroundWindowEx(windowHandle);
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Sets the foreground window (some "dirty" win32 stuff).
		/// </summary>
		/// <param name="hWnd">Handle of the window to set to the front.</param>
		/// <remarks>
		/// This method takes over the input thread for the window. This means that you are unable
		/// to debug the code between "Attach" and "Detach" since the input thread of Visual Studio
		/// will be attached to the thread of the application.
		/// </remarks>
		private static void SetForegroundWindowEx(IntPtr hWnd)
		{
			IntPtr foregroundWindowThreadID = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
			IntPtr currentThreadID = GetCurrentThreadId();

			if (!AttachThreadInput(foregroundWindowThreadID, currentThreadID, true))
			{
                Log.Warning("Failed to attach to input thread (Win32 code '{0}')", Marshal.GetLastWin32Error());
				return;
			}

			IntPtr lastActivePopupWindow = GetLastActivePopup(hWnd);
			SetActiveWindow(lastActivePopupWindow);

			if (!AttachThreadInput(foregroundWindowThreadID, currentThreadID, false))
			{
                Log.Warning("Failed to detach from input thread");
				return;
			}

			BringWindowToTop(hWnd);
		}

        /// <summary>
        /// Get first parent binding group from specified element.
        /// </summary>
        /// <param name="frameworkElement">Reference to the current <see cref="FrameworkElement"/>.</param>
        /// <returns>The first parent BindingGroup or null when not exists.</returns>
        public static System.Windows.Data.BindingGroup GetParentBindingGroup(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null)
            {
                return null;
            }

            if (frameworkElement.BindingGroup != null)
            {
                return frameworkElement.BindingGroup;
            }

            return GetParentBindingGroup(LogicalTreeHelper.GetParent(frameworkElement) as FrameworkElement);
        }
#endif

        /// <summary>
        /// Gets the parent. This method first tries to use the <see cref="FrameworkElement.Parent"/> property. If that is <c>null</c>,
        /// it will use the <c>FrameworkElement.TemplatedParent</c>. If that is <c>null</c>, this method assumes there is no
        /// parent and will return <c>null</c>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>The parent or <c>null</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        public static DependencyObject GetParent(this FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            var parent = VisualTreeHelper.GetParent(frameworkElement);
            if (parent != null)
            {
                return parent;
            }

            if (frameworkElement.Parent != null)
            {
                return frameworkElement.Parent;
            }

#if NET
            if (frameworkElement.TemplatedParent != null)
            {
                return frameworkElement.TemplatedParent;
            }
#endif

            return null;
        }

#if SILVERLIGHT
        /// <summary>
        /// Fixes the UI language bug in Silverlight.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        public static void FixUILanguageBug(this FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            frameworkElement.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        /// <summary>
        /// Tries to find the resource. WPF (of course) already has an implementation for this, Silverlight doesn't.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The resource or <c>null</c> if the resource is not found.
        /// </returns>
        public static bool TryFindResource<T>(this FrameworkElement frameworkElement, object resourceKey, out T value)
        {
            while (frameworkElement != null)
            {
                if (frameworkElement.Resources.TryFindResource(resourceKey, out value))
                {
                    return true;
                }

                frameworkElement = frameworkElement.Parent as FrameworkElement;
            }

            return Application.Current.Resources.TryFindResource(resourceKey, out value);
        }

        /// <summary>
        /// Tries to find the resource in the resource dictionary. WPF (of course) already has an implementation for this, Silverlight doesn't.
        /// </summary>
        /// <param name="dictionary">The resource dictionary.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The resource or <c>null</c> if the resource is not found.
        /// </returns>
        public static bool TryFindResource<T>(this ResourceDictionary dictionary, object resourceKey, out T value)
        {
            if (dictionary != null)
            {
                if (dictionary.Contains(resourceKey))
                {
                    var val = dictionary[resourceKey];
                    if ((val != null) && (val.GetType().IsAssignableFrom(typeof(T))))
                    {
                        value = (T)val;
                        return true;
                    }
                }

                foreach (var mergedDictionary in dictionary.MergedDictionaries)
                {
                    if (mergedDictionary.TryFindResource(resourceKey, out value))
                    {
                        return true;
                    }
                }
            }

            value = default(T);
            return false;
        }
#endif
        #endregion
    }
}
