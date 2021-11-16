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
        /// <summary>
        /// Gets the active window to use as parent window of new modal windows.
        /// <para />
        /// The default implementation returns the active window of the application.
        /// </summary>
        /// <returns>The active window.</returns>
        protected virtual FrameworkElement GetActiveWindow()
        {
            return Application.Current.GetActiveWindow();
        }

        /// <summary>
        /// Gets the main window to use as parent window for new non-modal windows.
        /// </summary>
        /// <returns>The main window.</returns>
        protected virtual FrameworkElement GetMainWindow()
        {
            var mainWindow = Application.Current.MainWindow;

            if (PropertyHelper.TryGetPropertyValue(mainWindow, "NeverMeasured", out bool neverMeasured))
            {
                if (!neverMeasured)
                {
                    // Window should be valid
                    return mainWindow;
                }
            }

            // In case the window is not initialized, return active window as main window
            return GetActiveWindow();
        }

        protected virtual void SetOwnerWindow(FrameworkElement window, System.Windows.Window ownerWindow)
        {
            if (!ReferenceEquals(window, ownerWindow))
            {
                PropertyHelper.TrySetPropertyValue(window, "Owner", ownerWindow);
            }
        }

        /// <summary>
        /// This creates the window from a key.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>The created window.</returns>    
        [ObsoleteEx(ReplacementTypeOrMember = "CreateWindowAsync", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
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
        [ObsoleteEx(ReplacementTypeOrMember = "CreateWindowAsync", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
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

            if (window != null && completedProc != null)
            {
                HandleCloseSubscription(window, data, completedProc, isModal);
            }

            return window;
        }

        /// <summary>
        /// This creates the window from a key.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>The created window.</returns>
        [ObsoleteEx(ReplacementTypeOrMember = "CreateWindowAsync(UIVisualizerContext)", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual async Task<FrameworkElement> CreateWindowAsync(string name, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            Type windowType;

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.TryGetValue(name, out windowType))
                {
                    return null;
                }
            }

            return await CreateWindowAsync(windowType, data, completedProc, isModal);
        }

        /// <summary>
        /// This creates the window from a key.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The created window.</returns>
        protected virtual async Task<FrameworkElement> CreateWindowAsync(UIVisualizerContext context)
        {
            Type windowType;

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.TryGetValue(context.Name, out windowType))
                {
                    return null;
                }
            }

            var result = await CreateWindowAsync(windowType, context);
            return result;
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
        [ObsoleteEx(ReplacementTypeOrMember = "CreateWindowAsync(UIVisualizerContext)", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual Task<FrameworkElement> CreateWindowAsync(Type windowType, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            return CreateWindowAsync(windowType, new UIVisualizerContext
            {
                Data = data,
                CompletedCallback = completedProc,
                IsModal = isModal
            });
        }

        /// <summary>
        /// This creates the window of the specified type.
        /// </summary>
        /// <param name="windowType">The type of the window.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The created window.
        /// </returns>
        protected virtual async Task<FrameworkElement> CreateWindowAsync(Type windowType, UIVisualizerContext context)
        {
            var tcs = new TaskCompletionSource<FrameworkElement>();

            _dispatcherService.BeginInvoke(() =>
            {
                try
                {
                    var window = ViewHelper.ConstructViewWithViewModel(windowType, context.Data);

                    // Important: don't set owner window here. Whenever this owner gets closed between this moment and the actual
                    // showing, this window will be diposed automatically too. For more information, see https://github.com/Catel/Catel/issues/1794
                    //
                    // Keeping this code so it's easier to understand why things are done this way
                    //
                    //if (isModal)
                    //{
                    //    SetOwnerWindow(window);
                    //}

                    // Explicitly clear since creating a data window automatically sets the owner window
                    SetOwnerWindow(window, null);

                    var completedCallback = context.CompletedCallback;

                    if (window is not null &&
                        completedCallback is not null)
                    {
                        HandleCloseSubscription(window, context);
                    }

                    tcs.TrySetResult(window);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            return await tcs.Task;
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
        [ObsoleteEx(ReplacementTypeOrMember = "HandleCloseSubscription(UIVisualizerContext, bool)", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual void HandleCloseSubscription(object window, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            HandleCloseSubscription(window, new UIVisualizerContext
            {
                Data = data,
                CompletedCallback = completedProc,
                IsModal = isModal
            });
        }

        /// <summary>
        /// Handles the close subscription.
        /// <para />
        /// The default implementation uses the <see cref="WeakEventListener"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleCloseSubscription(object window, UIVisualizerContext context)
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
                        var vm = context.Data as IViewModel;
                        if (vm != null)
                        {
                            dialogResult = vm.GetResult();
                        }
                    }

                    var completedProc = context.CompletedCallback;
                    if (completedProc is not null)
                    {
                        try
                        {
                            completedProc(this, new UICompletedEventArgs(context, dialogResult));
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
        [ObsoleteEx(ReplacementTypeOrMember = "ShowWindowAsync(UIVisualizerContext, bool)", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        protected virtual Task<bool?> ShowWindowAsync(FrameworkElement window, object data, bool showModal)
        {
            return ShowWindowAsync(window, new UIVisualizerContext
            {
                Data = data,
                IsModal = showModal
            });
        }

        /// <summary>
        /// Shows the window, respecting the specified context.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if the window is closed with success; otherwise <c>false</c> or <c>null</c>.</returns>
        public virtual Task<bool?> ShowWindowAsync(FrameworkElement window, UIVisualizerContext context)
        {
            // Note: no async/await because we use a TaskCompletionSource
            var tcs = new TaskCompletionSource<bool?>();

            HandleCloseSubscription(window, context, (sender, args) => tcs.TrySetResult(args.Result), context.IsModal);

            var showMethodInfo = context.IsModal ? window.GetType().GetMethodEx("ShowDialog") : window.GetType().GetMethodEx("Show");
            if (context.IsModal && showMethodInfo is null)
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
                window.Dispatcher.BeginInvoke(async () =>
                {
                    if (context.SetParentWindow)
                    {
                        var parentWindowCallback = context.SetParentWindowCallback;
                        if (parentWindowCallback is not null)
                        {
                            await parentWindowCallback(context, window);
                        }
                        else
                        {
                            var parentWindow = (context.IsModal ? GetActiveWindow() : GetMainWindow()) as System.Windows.Window;
                            SetOwnerWindow(window, parentWindow);
                        }
                    }

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
                }, DispatcherPriority.Input);
            }

            return tcs.Task;
        }
    }
}

#endif
