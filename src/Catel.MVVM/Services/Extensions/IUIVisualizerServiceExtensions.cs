namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Windows.Threading;
    using IoC;
    using Logging;
    using MVVM;
    using MVVM.Views;
    using Reflection;

    /// <summary>
    /// Extension methods for the <see cref="IUIVisualizerService" />.
    /// </summary>
    public static partial class IUIVisualizerServiceExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified view model type is registered.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <returns><c>true</c> if the specified view model type is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered<TViewModel>(this IUIVisualizerService uiVisualizerService)
        {
            return IsRegistered(uiVisualizerService, typeof(TViewModel));
        }

        /// <summary>
        /// Determines whether the specified view model type is registered.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns><c>true</c> if the specified view model type is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        public static bool IsRegistered(this IUIVisualizerService uiVisualizerService, Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return uiVisualizerService.IsRegistered(viewModelType.GetSafeFullName());
        }

        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="System.ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">viewModelType</exception>
        public static void Register<TViewModel, TView>(this IUIVisualizerService uiVisualizerService, bool throwExceptionIfExists = true)
        {
            Register(uiVisualizerService, typeof(TViewModel), typeof(TView), throwExceptionIfExists);
        }

        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="System.ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">viewModelType</exception>
        /// <exception cref="ArgumentException">The <paramref name="viewModelType" /> does not implement <see cref="IViewModel" />.</exception>
        public static void Register(this IUIVisualizerService uiVisualizerService, Type viewModelType, Type windowType, bool throwExceptionIfExists = true)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);
            Argument.ImplementsInterface<IViewModel>(nameof(viewModelType), viewModelType);

            uiVisualizerService.Register(viewModelType.GetSafeFullName(), windowType, throwExceptionIfExists);
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <returns><c>true</c> if the view model is unregistered; otherwise <c>false</c>.</returns>
        public static bool Unregister<TViewModel>(this IUIVisualizerService uiVisualizerService)
        {
            return Unregister(uiVisualizerService, typeof(TViewModel));
        }

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="viewModelType">Type of the view model to unregister.</param>
        /// <returns><c>true</c> if the view model is unregistered; otherwise <c>false</c>.</returns>
        public static bool Unregister(this IUIVisualizerService uiVisualizerService, Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return uiVisualizerService.Unregister(viewModelType.GetSafeFullName());
        }

        /// <summary>
        /// Shows the window in non-modal state and creates the view model automatically using the specified model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="model">The model to be injected into the view model, can be <c>null</c>.</param>
        /// <param name="completedProc">The completed proc.</param>
        /// <returns><c>true</c> if shown successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        public static Task<UIVisualizerResult> ShowAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object? model = null, EventHandler<UICompletedEventArgs>? completedProc = null)
            where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateRequiredViewModel<TViewModel>(model);
            return uiVisualizerService.ShowAsync(vm, completedProc);
        }

        /// <summary>
        /// Shows the window in modal state and creates the view model automatically using the specified model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="model">The model to be injected into the view model, can be <c>null</c>.</param>
        /// <param name="completedProc">The completed proc.</param>
        /// <returns>The dialog result.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        public static Task<UIVisualizerResult> ShowDialogAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object? model = null, EventHandler<UICompletedEventArgs>? completedProc = null)
            where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateRequiredViewModel<TViewModel>(model);
            return uiVisualizerService.ShowDialogAsync(vm, completedProc);
        }

        private static IViewModelFactory GetViewModelFactory(IUIVisualizerService uiVisualizerService)
        {
            var dependencyResolver = uiVisualizerService.GetDependencyResolver();
            var viewModelFactory = dependencyResolver.ResolveRequired<IViewModelFactory>();
            return viewModelFactory;
        }

        /// <summary>
        /// Creates a window in non-modal state. If a window with the specified viewModelType exists, the window is activated instead of being created.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="model">The model to be injected into the view model, can be <c>null</c>.</param>
        /// <param name="scope">The service locator scope.</param>
        /// <param name="completedProc">The completed proc. Not applicable if window already exists.</param>
        /// <returns>
        ///   <c>true</c> if shown or activated successfully, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        public static async Task<bool?> ShowOrActivateAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object? model = null, object? scope = null, EventHandler<UICompletedEventArgs>? completedProc = null)
            where TViewModel : IViewModel
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);

            var dependencyResolver = uiVisualizerService.GetDependencyResolver();

#pragma warning disable IDISP001
            var viewModelManager = dependencyResolver.ResolveRequired<IViewModelManager>();
#pragma warning restore IDISP001
            var viewModel = viewModelManager.GetFirstOrDefaultInstance(typeof(TViewModel));
            if (viewModel is null)
            {
                var viewModelFactory = GetViewModelFactory(uiVisualizerService);
                var vm = viewModelFactory.CreateRequiredViewModel<TViewModel>(model, scope);
                var result = await uiVisualizerService.ShowAsync(vm, completedProc);
                return result.DialogResult;
            }

            var viewLocator = dependencyResolver.ResolveRequired<IViewLocator>();
            var viewType = viewLocator.ResolveView(viewModel.GetType());
            if (viewType is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Found active instance of '{viewModel.GetType().GetSafeFullName()}', but could not find a related view type");
            }

            var viewManager = dependencyResolver.ResolveRequired<IViewManager>();
            var view = viewManager.GetFirstOrDefaultInstance(viewType);
            if (viewType is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Found active instance of '{viewModel.GetType().GetSafeFullName()}', but could not find a related view");
            }

            var window = view as System.Windows.Window;
            if (view is null || window is null)
            {
                var result = await uiVisualizerService.ShowAsync(viewModel, completedProc);
                return result.DialogResult;
            }

            var activated = ActivateWindow(window);
            return activated;
        }

        /// <summary>
        /// Activates the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns><c>true</c> if the window is activated with success; otherwise <c>false</c> or <c>null</c>.</returns>
        public static bool? ActivateWindow(Window window)
        {
            ArgumentNullException.ThrowIfNull(window);

            var activateMethodInfo = window.GetType().GetMethodEx("Activate");
            if (activateMethodInfo is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>($"Method 'Activate' not found on '{window.GetType().Name}', cannot activate the window");
            }

            bool? result = false;
            window.Dispatcher.InvokeIfRequired(() => result = (bool?)activateMethodInfo.Invoke(window, null));
            return result;
        }

        /// <summary>
        /// Creates a window in non-modal state. If a window with the specified viewModelType exists, the window is activated instead of being created.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="uiVisualizerService">The uiVisualizerService</param>
        /// <param name="dataContext">The data context.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>
        /// A task.
        /// </returns>
        public static async Task ShowOrActivateAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object? dataContext = null, object? scope = null)
            where TViewModel : IViewModel
        {
            var dependencyResolver = uiVisualizerService.GetDependencyResolver();
            var viewModelManager = dependencyResolver.ResolveRequired<IViewModelManager>();
            var viewModelFactory = dependencyResolver.ResolveRequired<IViewModelFactory>();

            var existingViewModel = viewModelManager.GetFirstOrDefaultInstance<TViewModel>();
            if (existingViewModel is not null)
            {
                await uiVisualizerService.ShowOrActivateAsync<TViewModel>(dataContext, scope);
            }
            else
            {
                var vm = viewModelFactory.CreateRequiredViewModel(typeof(TViewModel), dataContext, scope);
                await uiVisualizerService.ShowAsync(vm);
            }
        }
    }
}
