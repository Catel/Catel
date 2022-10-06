namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MVVM;
    using Logging;
    using Catel.Reflection;

    /// <summary>
    /// Service to show modal or non-modal popup windows.
    /// <para/>
    /// All windows will have to be registered manually or are be resolved via the <see cref="Catel.MVVM.IViewLocator"/>.
    /// </summary>
    public partial class UIVisualizerService : ViewModelServiceBase, IUIVisualizerService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected readonly Dictionary<string, Type> RegisteredWindows = new Dictionary<string, Type>();

        private readonly IViewLocator _viewLocator;
        private readonly IDispatcherService _dispatcherService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIVisualizerService"/> class.
        /// </summary>
        /// <param name="viewLocator">The view locator.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLocator"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcherService"/> is <c>null</c>.</exception>
        public UIVisualizerService(IViewLocator viewLocator, IDispatcherService dispatcherService)
        {
            ArgumentNullException.ThrowIfNull(viewLocator);
            ArgumentNullException.ThrowIfNull(dispatcherService);

            _viewLocator = viewLocator;
            _dispatcherService = dispatcherService;
        }

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
            ArgumentNullException.ThrowIfNull(windowType);

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
        /// Shows a window with the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The dialog result.</returns>
        public virtual async Task<UIVisualizerResult> ShowContextAsync(UIVisualizerContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var viewModel = context.Data as IViewModel;
            if (viewModel is not null)
            {
                var viewModelType = viewModel.GetType();

                RegisterViewForViewModelIfRequired(viewModelType);

                if (string.IsNullOrWhiteSpace(context.Name))
                {
                    context.Name = viewModelType.GetSafeFullName();
                }
            }

            if (!string.IsNullOrWhiteSpace(context.Name))
            {
                EnsureViewIsRegistered(context.Name);
            }

            var window = await CreateWindowAsync(context);
            if (window is not null)
            {
                var result = await ShowWindowAsync(window, context);
                return result;
            }

            return new UIVisualizerResult(null, context, null);
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
                    throw Log.ErrorAndCreateException<WindowNotRegisteredException>(name);
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
                var fullName = viewModelType.GetSafeFullName();

                if (!RegisteredWindows.ContainsKey(fullName))
                {
                    var viewType = _viewLocator.ResolveView(viewModelType);
                    if (viewType is not null)
                    {
                        Register(fullName, viewType);
                    }
                }
            }
        }
    }
}
