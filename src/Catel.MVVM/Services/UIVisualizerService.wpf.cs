// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Catel.Windows.Threading;
    using Logging;
    using MVVM;
    using Reflection;
    using Windows;

    /// <summary>
    /// Service to show modal or non-modal popup windows.
    /// <para/>
    /// All windows will have to be registered manually or are be resolved via the <see cref="Catel.MVVM.IViewLocator"/>.
    /// </summary>
    public partial class UIVisualizerService
    {
        #region Methods
        /// <summary>
        /// Gets the active window to use as parent window of new windows.
        /// <para />
        /// The default implementation returns the active window of the application.
        /// </summary>
        /// <returns>The active window.</returns>
        protected virtual FrameworkElement GetActiveWindow()
        {
            return Application.Current.GetActiveWindow();
        }

        /// <summary>
        /// This creates the window from a key.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>The created window.</returns>    
        protected virtual FrameworkElement CreateWindow(string name, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            Type windowType;

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.TryGetValue(name, out windowType))
                {
                    return null;
                }
            }

            return CreateWindow(windowType, data, completedProc, isModal);
        }

        /// <summary>
        /// This creates the window of the specified type.
        /// </summary>
        /// <param name="windowType">The type of the window.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>
        /// The created window.
        /// </returns>
        protected virtual FrameworkElement CreateWindow(Type windowType, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            var window = ViewHelper.ConstructViewWithViewModel(windowType, data);

            if (isModal)
            {
                var activeWindow = GetActiveWindow();
                if (!ReferenceEquals(window, activeWindow))
                {
                    PropertyHelper.TrySetPropertyValue(window, "Owner", activeWindow);
                }
            }

            if ((window != null) && (completedProc != null))
            {
                HandleCloseSubscription(window, data, completedProc, isModal);
            }

            return window;
        }

        /// <summary>
        /// Handles the close subscription.
        /// <para />
        /// The default implementation uses the <see cref="WeakEventListener"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        protected virtual void HandleCloseSubscription(object window, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            var eventInfo = window.GetType().GetEvent("Closed");
            var addMethod = eventInfo?.AddMethod;
            if (addMethod != null)
            {
                EventHandler eventHandler = null;
                void Closed(object s, EventArgs e)
                {
                    if (!ReferenceEquals(window, s))
                    {
                        // Fix for https://github.com/Catel/Catel/issues/1074
                        return;
                    }

                    PropertyHelper.TryGetPropertyValue(window, "DialogResult", out bool? dialogResult);

                    if (dialogResult is null)
                    {
                        // See https://github.com/Catel/Catel/issues/1503, even though there is no real DialogResult,
                        // we will get the result from the VM instead
                        var vm = data as IViewModel;
                        if (vm != null)
                        {
                            dialogResult = vm.GetResult();
                        }
                    }

                    try
                    {
                        completedProc(this, new UICompletedEventArgs(data, dialogResult));
                    }
                    finally
                    {
                        var removeMethod = eventInfo.RemoveMethod;
                        if (removeMethod != null)
                        {
                            removeMethod.Invoke(window, new object[] { eventHandler });
                        }
                    }
                }

                eventHandler = Closed;
                addMethod.Invoke(window, new object[] { eventHandler });
            }
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="data">The data.</param>
        /// <param name="showModal">If <c>true</c>, the window should be shown as modal.</param>
        /// <returns><c>true</c> if the window is closed with success; otherwise <c>false</c> or <c>null</c>.</returns>
        protected virtual Task<bool?> ShowWindowAsync(FrameworkElement window, object data, bool showModal)
        {
            // Note: no async/await because we use a TaskCompletionSource
            var tcs = new TaskCompletionSource<bool?>();

            HandleCloseSubscription(window, data, (sender, args) => tcs.TrySetResult(args.Result), showModal);

            var showMethodInfo = showModal ? window.GetType().GetMethodEx("ShowDialog") : window.GetType().GetMethodEx("Show");
            if (showModal && showMethodInfo is null)
            {
                Log.Warning("Method 'ShowDialog' not found on '{0}', falling back to 'Show'", window.GetType().Name);

                showMethodInfo = window.GetType().GetMethodEx("Show");
            }

            if (showMethodInfo is null)
            {
                var exception = Log.ErrorAndCreateException<NotSupportedException>($"Methods 'Show' or 'ShowDialog' not found on '{window.GetType().Name}', cannot show the window");
                tcs.SetException(exception);
            }
            else
            {
                // ORCOMP-337: Always invoke with priority Input.
                window.Dispatcher.BeginInvoke(() =>
                {
                    // Safety net to prevent crashes when this is the main window
                    try
                    {
                        showMethodInfo.Invoke(window, null);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"An error occurred while showing window '{window.GetType().GetSafeFullName(true)}'");
                        tcs.TrySetResult(null);
                    }
                }, DispatcherPriority.Input, false);
            }

            return tcs.Task;
        }
        #endregion
    }
}

#endif
