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
    using Threading;

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
            Argument.IsNotNull("viewModelType", viewModelType);

            return uiVisualizerService.IsRegistered(viewModelType.FullName);
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
            Argument.IsNotNull("viewModelType", viewModelType);

            if (viewModelType.GetInterfaceEx(typeof(IViewModel).FullName, false) is null)
            {
                throw new ArgumentException("The argument must implement IViewModel interface", "viewModelType");
            }

            uiVisualizerService.Register(viewModelType.FullName, windowType, throwExceptionIfExists);
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
            Argument.IsNotNull("viewModelType", viewModelType);

            return uiVisualizerService.Unregister(viewModelType.FullName);
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
        public static Task<UIVisualizerResult> ShowAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
            where TViewModel : IViewModel
        {
            Argument.IsNotNull("uiVisualizerService", uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateViewModel(typeof(TViewModel), model);
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
        public static Task<UIVisualizerResult> ShowDialogAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
            where TViewModel : IViewModel
        {
            Argument.IsNotNull("uiVisualizerService", uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateViewModel(typeof(TViewModel), model);
            return uiVisualizerService.ShowDialogAsync(vm, completedProc);
        }

        private static IViewModelFactory GetViewModelFactory(IUIVisualizerService uiVisualizerService)
        {
            var dependencyResolver = uiVisualizerService.GetDependencyResolver();
            var viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();
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
        public static async Task<bool?> ShowOrActivateAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, object scope = null, EventHandler<UICompletedEventArgs> completedProc = null)
            where TViewModel : IViewModel
        {
            Argument.IsNotNull("uiVisualizerService", uiVisualizerService);

            var dependencyResolver = uiVisualizerService.GetDependencyResolver();

            var viewModelManager = dependencyResolver.Resolve<IViewModelManager>();
            var viewModel = viewModelManager.GetFirstOrDefaultInstance(typeof(TViewModel));
            if (viewModel is null)
            {
                var viewModelFactory = GetViewModelFactory(uiVisualizerService);
                var vm = viewModelFactory.CreateViewModel(typeof(TViewModel), model, scope);
                var result = await uiVisualizerService.ShowAsync(vm, completedProc);
                return result.DialogResult;
            }

            var viewLocator = dependencyResolver.Resolve<IViewLocator>();
            var viewType = viewLocator.ResolveView(viewModel.GetType());
            var viewManager = dependencyResolver.Resolve<IViewManager>();
            var view = viewManager.GetFirstOrDefaultInstance(viewType);
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
            var activateMethodInfo = window.GetType().GetMethodEx("Activate");
            if (activateMethodInfo is null)
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Method 'Activate' not found on '{0}', cannot activate the window", window.GetType().Name);
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
        public static async Task ShowOrActivateAsync<TViewModel>(this IUIVisualizerService uiVisualizerService, object dataContext = null, object scope = null)
            where TViewModel : IViewModel
        {
            var dependencyResolver = uiVisualizerService.GetDependencyResolver();
            var viewModelManager = dependencyResolver.Resolve<IViewModelManager>();
            var viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();

            var existingViewModel = viewModelManager.GetFirstOrDefaultInstance<TViewModel>();
            if (existingViewModel is not null)
            {
                await uiVisualizerService.ShowOrActivateAsync<TViewModel>(dataContext, scope);
            }
            else
            {
                var vm = viewModelFactory.CreateViewModel(typeof(TViewModel), dataContext, scope);
                await uiVisualizerService.ShowAsync(vm);
            }
        }
    }
}
