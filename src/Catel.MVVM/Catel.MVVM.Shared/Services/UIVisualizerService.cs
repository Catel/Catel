// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using MVVM;

    using Logging;
    using MVVM.Properties;
    using Reflection;
    using Catel.Windows.Threading;

#if NET
    using Windows;
#endif

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Service to show modal or non-modal popup windows.
    /// <para/>
    /// All windows will have to be registered manually or are be resolved via the <see cref="Catel.MVVM.IViewLocator"/>.
    /// </summary>
    public class UIVisualizerService : ViewModelServiceBase, IUIVisualizerService
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary of registered windows.
        /// </summary>
        protected readonly Dictionary<string, Type> RegisteredWindows = new Dictionary<string, Type>();

        /// <summary>
        /// The view locator.
        /// </summary>
        private readonly IViewLocator _viewLocator;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UIVisualizerService"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLocator"/> is <c>null</c>.</exception>
        public UIVisualizerService(IViewLocator viewLocator)
        {
            Argument.IsNotNull(() => viewLocator);

            _viewLocator = viewLocator;
        }

        #region Properties
        /// <summary>
        /// Gets the type of the window that this implementation of the <see cref="IUIVisualizerService"/> interface
        /// supports.
        /// <para />
        /// The default value is <c>Window</c> in WPF and <c>ChildWindow</c> in Silverlight.
        /// </summary>
        /// <value>
        /// The type of the window.
        /// </value>
        protected virtual Type WindowType
        {
            get
            {
                return typeof(ContentControl);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public virtual bool IsRegistered(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (RegisteredWindows)
            {
                return RegisteredWindows.ContainsKey(name);
            }
        }

        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="windowType" /> is not of type <see cref="WindowType" />.</exception>
        public virtual void Register(string name, Type windowType, bool throwExceptionIfExists = true)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("windowType", windowType);
            Argument.IsOfType("windowType", windowType, WindowType);

            lock (RegisteredWindows)
            {
                if (RegisteredWindows.ContainsKey(name))
                {
                    if (throwExceptionIfExists)
                    {
                        throw new InvalidOperationException(Exceptions.ViewModelAlreadyRegistered);
                    }
                }

                RegisteredWindows.Add(name, windowType);

                Log.Debug("Registered view model '{0}' in combination with '{1}' in the UIVisualizerService", name, windowType.FullName);
            }
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <returns>
        /// <c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Unregister(string name)
        {
            lock (RegisteredWindows)
            {
                bool result = RegisteredWindows.Remove(name);
                if (result)
                {
                    Log.Debug("Unregistered view model '{0}' in UIVisualizerService", name);
                }

                return result;
            }
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="ViewModelNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public virtual async Task<bool?> Show(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull("viewModel", viewModel);

            string viewModelTypeName = viewModel.GetType().FullName;

            if (!RegisteredWindows.ContainsKey(viewModelTypeName))
            {
                var viewType = _viewLocator.ResolveView(viewModel.GetType());
                if (viewType != null)
                {
                    this.Register(viewModel.GetType(), viewType);
                }
            }

            return await Show(viewModelTypeName, viewModel, completedProc);
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type, bool)"/> method first.</exception>
        public virtual async Task<bool?> Show(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.ContainsKey(name))
                {
                    throw new WindowNotRegisteredException(name);
                }
            }

            var window = CreateWindow(name, data, completedProc, false);
            if (window != null)
            {
                return await ShowWindow(window, false);
            }

            return false;
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public virtual async Task<bool?> ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull("viewModel", viewModel);

            string viewModelTypeName = viewModel.GetType().FullName;

            if (!RegisteredWindows.ContainsKey(viewModelTypeName))
            {
                var viewType = _viewLocator.ResolveView(viewModel.GetType());
                if (viewType != null)
                {
                    this.Register(viewModel.GetType(), viewType);
                }
            }

            return await ShowDialog(viewModelTypeName, viewModel, completedProc);
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        public virtual async Task<bool?> ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.ContainsKey(name))
                {
                    throw new WindowNotRegisteredException(name);
                }
            }

            var window = CreateWindow(name, data, completedProc, true);
            if (window != null)
            {
                return await ShowWindow(window, true);
            }

            return false;
        }

#if NET
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
#endif

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
        /// <returns>The created window.</returns>
        protected virtual FrameworkElement CreateWindow(Type windowType, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            var window = ViewHelper.ConstructViewWithViewModel(windowType, data);

#if NET
            var activeWindow = GetActiveWindow();
            if (window != activeWindow)
            {
                PropertyHelper.TrySetPropertyValue(window, "Owner", activeWindow);
            }
#endif

            if ((window != null) && (completedProc != null))
            {
                HandleCloseSubscription(window, data, completedProc, isModal);
            }

            return window;
        }

        /// <summary>
        /// Handles the close subscription.
        /// <para />
        /// The default implementation uses the <see cref="DynamicEventListener"/>.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        protected virtual void HandleCloseSubscription(object window, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            var dynamicEventListener = new DynamicEventListener(window, "Closed");

            EventHandler closed = null;
            closed = (sender, e) =>
            {
                bool? dialogResult = null;
                PropertyHelper.TryGetPropertyValue(window, "DialogResult", out dialogResult);

                completedProc(this, new UICompletedEventArgs(data, isModal ? dialogResult : null));
#if SILVERLIGHT     
                if (window is ChildWindow)
                {
                    // Due to a bug in the latest version of the Silverlight toolkit, set parent to enabled again
                    // TODO: After every toolkit release, check if this code can be removed
                    Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
                }
#endif
                dynamicEventListener.EventOccurred -= closed;
                dynamicEventListener.UnsubscribeFromEvent();
            };

            dynamicEventListener.EventOccurred += closed;
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="showModal">If <c>true</c>, the window should be shown as modal.</param>
        /// <returns><c>true</c> if the window is closed with success; otherwise <c>false</c> or <c>null</c>.</returns>
        protected virtual async Task<bool?> ShowWindow(FrameworkElement window, bool showModal)
        {
            return await Task<bool?>.Factory.StartNew(() =>
            {
                if (showModal)
                {
                    var showDialogMethodInfo = window.GetType().GetMethodEx("ShowDialog");
                    if (showDialogMethodInfo != null)
                    {
                        // Child window does not have a ShowDialog, so not null is allowed
                        bool? result = null;

                        window.Dispatcher.Invoke(() =>
                        {
                            // Safety net to prevent crashes when this is the main window
                            try
                            {
                                result = showDialogMethodInfo.Invoke(window, null) as bool?;
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(ex, "An error occurred, returning null since we don't know the result");
                            }
                        });

                        return result;
                    }

                    Log.Warning("Method 'ShowDialog' not found on '{0}', falling back to 'Show'", window.GetType().Name);
                }

                var showMethodInfo = window.GetType().GetMethodEx("Show");
                if (showMethodInfo == null)
                {
                    var error = string.Format("Method 'Show' not found on '{0}', cannot show the window", window.GetType().Name);
                    Log.Error(error);

                    throw new NotSupportedException(error);
                }

                window.Dispatcher.Invoke(() => showMethodInfo.Invoke(window, null));
                return null;
            });
        }
        #endregion
    }
}

#endif