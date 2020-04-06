#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;
    using MVVM;

    using IoC;
    using Logging;
    using Reflection;
    using Catel.Windows.Threading;
    using Threading;

#if UWP
    using global::Windows.UI.Xaml;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Service to show modal or non-modal popup windows.
    /// <para/>
    /// All windows will have to be registered manually or are be resolved via the <see cref="Catel.MVVM.IViewLocator"/>.
    /// </summary>
    public partial class UIVisualizerService : ViewModelServiceBase, IUIVisualizerService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, Type> RegisteredWindows = new Dictionary<string, Type>();

        private readonly IViewLocator _viewLocator;
        private readonly IDispatcherService _dispatcherService;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UIVisualizerService"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLocator"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService"/> is <c>null</c>.</exception>
        public UIVisualizerService(IViewLocator viewLocator, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull("viewLocator", viewLocator);
            Argument.IsNotNull("dispatcherService", dispatcherService);

            _viewLocator = viewLocator;
            _dispatcherService = dispatcherService;
        }

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
        public virtual void Register(string name, Type windowType, bool throwExceptionIfExists = true)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("windowType", windowType);

            lock (RegisteredWindows)
            {
                if (RegisteredWindows.TryGetValue(name, out var existingRegistration))
                {
                    if (existingRegistration != windowType && throwExceptionIfExists)
                    {
                        throw new InvalidOperationException($"View model '{name}' already registered");
                    }
                }

                RegisteredWindows[name] = windowType;

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
                var result = RegisteredWindows.Remove(name);
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
        public virtual async Task<bool?> ShowAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelType = viewModel.GetType();
            var viewModelTypeName = viewModelType.FullName;

            RegisterViewForViewModelIfRequired(viewModelType);

            return await ShowAsync(viewModelTypeName, viewModel, completedProc);
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
        public virtual async Task<bool?> ShowAsync(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            EnsureViewIsRegistered(name);

            var window = await CreateWindowAsync(name, data, completedProc, false);
            if (window != null)
            {
                return await ShowWindowAsync(window, data, false);
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
        public virtual async Task<bool?> ShowDialogAsync(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNull("viewModel", viewModel);

            var viewModelType = viewModel.GetType();
            var viewModelTypeName = viewModelType.FullName;

            RegisterViewForViewModelIfRequired(viewModelType);

            return await ShowDialogAsync(viewModelTypeName, viewModel, completedProc);
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
        public virtual async Task<bool?> ShowDialogAsync(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            EnsureViewIsRegistered(name);

            var window = await CreateWindowAsync(name, data, completedProc, true);
            if (window != null)
            {
                return await ShowWindowAsync(window, data, true);
            }

            return false;
        }

        /// <summary>
        /// Ensures that the specified view is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="WindowNotRegisteredException"></exception>
        protected virtual void EnsureViewIsRegistered(string name)
        {
            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.ContainsKey(name))
                {
                    throw new WindowNotRegisteredException(name);
                }
            }
        }

        /// <summary>
        /// Registers the view for the specified view model if required.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        protected virtual void RegisterViewForViewModelIfRequired(Type viewModelType)
        {
            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.ContainsKey(viewModelType.FullName))
                {
                    var viewType = _viewLocator.ResolveView(viewModelType);
                    if (viewType != null)
                    {
                        Register(viewModelType.FullName, viewType);
                    }
                }
            }
        }
        #endregion
    }
}

#endif
