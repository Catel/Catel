namespace Catel.Windows
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using Reflection;
    using Threading;
    using Logging;
    using SystemWindow = System.Windows.Window;
    using Catel.Win32;

    /// <summary>
    /// Extensions for <see cref="System.Windows.Window"/>.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Current process main window
        /// <summary>
        /// Sets the owner window to the main window of the current process.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        /// <param name="focusFirstControl">If true, the first control will automatically be focused.</param>
        public static void SetOwnerWindow(this SystemWindow window, bool forceNewOwner = false, bool focusFirstControl = false)
        {
            // Check active window first
            var activeWindow = Application.Current.GetActiveWindow();
            if (activeWindow is not null)
            {
                SetOwnerWindowByWindow(window, activeWindow, forceNewOwner, focusFirstControl);
                return;
            }

            var mainWindow = CatelEnvironment.MainWindow;
            if (mainWindow is not null)
            {
                SetOwnerWindowByWindow(window, mainWindow, forceNewOwner, focusFirstControl);
                return;
            }

            // Set by process main window handle
            SetOwnerWindowByHandle(window, GetProcessMainWindowHandle(), forceNewOwner, focusFirstControl);
        }

        /// <summary>
        /// Sets the owner window to the main window of the current process, but
        /// also sets the focus on the first control.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        /// <param name="focusFirstControl"></param>
        public static void SetOwnerWindowAndFocus(this SystemWindow window, bool forceNewOwner = false, bool focusFirstControl = true)
        {
            SetOwnerWindow(window, forceNewOwner, focusFirstControl);
        }
        #endregion

        #region Specific window - Window
        /// <summary>
        /// Sets the owner window of a specific window via the Window class.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        public static void SetOwnerWindow(this SystemWindow window, SystemWindow owner, bool forceNewOwner = false)
        {
            SetOwnerWindowByWindow(window, owner, forceNewOwner, false);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the Window class, but
        /// also sets the focus on the first control.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        public static void SetOwnerWindowAndFocus(this SystemWindow window, SystemWindow owner, bool forceNewOwner = false)
        {
            SetOwnerWindowByWindow(window, owner, forceNewOwner, true);
        }

        /// <summary>
        /// Sets the owner window of a specific window.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        /// <param name="focusFirstControl">If true, the first control will automatically be focused.</param>
        private static void SetOwnerWindowByWindow(SystemWindow window, SystemWindow owner, bool forceNewOwner = false, bool focusFirstControl = true)
        {
            SetOwnerWindow(window, owner, IntPtr.Zero, forceNewOwner, focusFirstControl);
        }
        #endregion

        #region Specific window - IntPtr
        /// <summary>
        /// Sets the owner window of a specific window via the window handle.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        public static void SetOwnerWindow(this SystemWindow window, IntPtr owner, bool forceNewOwner = false)
        {
            SetOwnerWindowByHandle(window, owner, forceNewOwner, false);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the window handle, but
        /// also sets the focus on the first control.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        public static void SetOwnerWindowAndFocus(this SystemWindow window, IntPtr owner, bool forceNewOwner = false)
        {
            SetOwnerWindowByHandle(window, owner, forceNewOwner, true);
        }

        /// <summary>
        /// Sets the owner window of a specific window via the window handle.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="owner">New owner window.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        /// <param name="focusFirstControl">If true, the first control will automatically be focused.</param>
        private static void SetOwnerWindowByHandle(SystemWindow window, IntPtr owner, bool forceNewOwner = false, bool focusFirstControl = true)
        {
            SetOwnerWindow(window, null, owner, forceNewOwner, focusFirstControl);
        }
        #endregion

        /// <summary>
        /// Determines whether the <see cref="SystemWindow.DialogResult"/> property can be set.
        /// </summary>
        /// <param name="window">The window to check.</param>
        /// <returns><c>true</c> if the dialog result can be set; otherwise, <c>false</c>.</returns>
        public static bool CanSetDialogResult(this SystemWindow window)
        {
            ArgumentNullException.ThrowIfNull(window);

            // See #1616, not fully reliable, so we can only exit if this returns *false*
            if (!ComponentDispatcher.IsThreadModal)
            {
                return false;
            }

            // Double check that we can use this one for reflection
            if (window is SystemWindow)
            {
                var dialogFieldInfo = typeof(SystemWindow).GetFieldEx("_showingAsDialog");
                if (dialogFieldInfo is not null)
                {
                    var dialogFieldValue = dialogFieldInfo.GetValue(window) as bool?;
                    if (dialogFieldValue is null || !dialogFieldValue.Value)
                    {
                        return false;
                    }
                }
            }

            // Assume true, at this stage we are willing to take the first chance exception if we need to
            return true;
        }

        public static bool IsValidAsOwnerWindow(this SystemWindow window)
        {
            if (window is null)
            {
                return false;
            }

            if (!window.IsLoaded)
            {
                return false;
            }

            if (window.ActualWidth > 0d && window.ActualHeight > 0d)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the window handle of the specified window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>IntPtr.</returns>
        public static IntPtr GetWindowHandle(this SystemWindow window)
        {
            ArgumentNullException.ThrowIfNull(window);

            var interopHelper = new WindowInteropHelper(window);
            return interopHelper.Handle;
        }

        /// <summary>
        /// Brings to specified window to top.
        /// </summary>
        /// <param name="window">The window to bring to top.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="window"/> is <c>null</c>.</exception>
        public static void BringWindowToTop(this SystemWindow window)
        {
            ArgumentNullException.ThrowIfNull(window);

            var windowHandle = window.GetWindowHandle();

            User32.BringWindowToTop(windowHandle);
        }

        /// <summary>
        /// Sets the owner window of a specific window. It will first try to set the owner via
        /// the <paramref name="ownerWindow"/>. If the <paramref name="ownerWindow"/> is not available,
        /// this method will use the <paramref name="ownerHandle"/> to set the parent.
        /// </summary>
        /// <param name="window">Reference to the current window.</param>
        /// <param name="ownerWindow">New owner window.</param>
        /// <param name="ownerHandle">The owner handle.</param>
        /// <param name="forceNewOwner">If true, the new owner will be forced. Otherwise, if the
        /// window currently has an owner, that owner will be respected (and thus not changed).</param>
        /// <param name="focusFirstControl">If true, the first control will automatically be focused.</param>
        private static void SetOwnerWindow(SystemWindow window, SystemWindow? ownerWindow, IntPtr ownerHandle, bool forceNewOwner, bool focusFirstControl)
        {
            ArgumentNullException.ThrowIfNull(window);

            if (focusFirstControl)
            {
                window.FocusFirstControl();
            }

            if (!forceNewOwner && HasOwner(window))
            {
                return;
            }

            try
            {
                if (ownerWindow is not null)
                {
                    if (ReferenceEquals(ownerWindow, window))
                    {
                        Log.Warning("Cannot set owner window to itself, no owner window set");
                        return;
                    }

                    if (window.Dispatcher.GetThreadId() != ownerWindow.Dispatcher.GetThreadId())
                    {
                        Log.Warning("The owner window '{0}' is not created on the same thread as the current window '{1}', cannot set owner window",
                            ownerWindow.GetType().GetSafeFullName(false), window.GetType().GetSafeFullName(false));
                        return;
                    }

                    window.Owner = ownerWindow;
                }
                else
                {
                    // Set owner via interop helper
                    var interopHelper = new WindowInteropHelper(window);
                    interopHelper.Owner = ownerHandle;

                    // Get handler (so we can nicely unsubscribe)
                    RoutedEventHandler? onWindowLoaded = null;
                    onWindowLoaded = delegate(object sender, RoutedEventArgs e)
                    {
                        // Since this owner type doesn't support WindowStartupLocation.CenterOwner, do
                        // it manually
                        if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
                        {
                            // Get the parent window rect
                            if (User32.GetWindowRect(ownerHandle, out var ownerRect))
                            {
                                // Get some additional information
                                int ownerWidth = ownerRect.Right - ownerRect.Left;
                                int ownerHeight = ownerRect.Bottom - ownerRect.Top;
                                int ownerHorizontalCenter = (ownerWidth / 2) + ownerRect.Left;
                                int ownerVerticalCenter = (ownerHeight / 2) + ownerRect.Top;

                                // Set the location to manual
                                window.WindowStartupLocation = WindowStartupLocation.Manual;

                                // Now we know the location of the parent, center the window
                                window.Left = ownerHorizontalCenter - (window.ActualWidth / 2);
                                window.Top = ownerVerticalCenter - (window.ActualHeight / 2);
                            }
                        }

                        ((SystemWindow)sender).Loaded -= onWindowLoaded;
                    };

                    window.Loaded += onWindowLoaded;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to set the owner window");
            }
        }

        /// <summary>
        /// Returns the main window handle of the current process.
        /// </summary>
        /// <returns>Handle of the main window of the current process.</returns>
        private static IntPtr GetProcessMainWindowHandle()
        {
            var process = Process.GetCurrentProcess();
            var mainWindowHandle = process.MainWindowHandle;
            process.Dispose();

            return mainWindowHandle;
        }

        /// <summary>
        /// Returns whether the window currently has an owner.
        /// </summary>
        /// <param name="window">Window to check.</param>
        /// <returns>
        /// True if the window has an owner, otherwise false.
        /// </returns>
        private static bool HasOwner(SystemWindow window)
        {
            return ((window.Owner is not null) || (new WindowInteropHelper(window).Owner != IntPtr.Zero));
        }

        /// <summary>
        /// Applies the icon from the entry assembly (the application) to the window.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void ApplyIconFromApplication(this SystemWindow window)
        {
            ArgumentNullException.ThrowIfNull(window);

            try
            {
                if (window.Icon is not null)
                {
                    return;
                }

                var currentApplication = Application.Current;
                if (currentApplication is not null)
                {
                    var entryAssembly = AssemblyHelper.GetEntryAssembly();
                    if (entryAssembly is not null)
                    {
                        var icon = Icon.ExtractAssociatedIcon(entryAssembly.Location);
                        if (icon is not null)
                        {
                            window.Icon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                                new Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to set the application icon to the window");
            }
        }
    }
}
