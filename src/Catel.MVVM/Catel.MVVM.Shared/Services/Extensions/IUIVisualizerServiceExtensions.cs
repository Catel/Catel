// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIVisualizerServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using IoC;
    using MVVM;
    using MVVM.Properties;

    /// <summary>
    /// Extension methods for the <see cref="IUIVisualizerService"/>.
    /// </summary>
    public static class IUIVisualizerServiceExtensions
    {
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

            if (viewModelType.GetInterface(typeof(IViewModel).FullName, false) == null)
            {
                throw new ArgumentException(Exceptions.ArgumentMustImplementIViewModelInterface, "viewModelType");
            }

            uiVisualizerService.Register(viewModelType.FullName, windowType);
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
        public static Task<bool?> Show<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
            where TViewModel : IViewModel
        {
            Argument.IsNotNull("uiVisualizerService", uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateViewModel(typeof (TViewModel), model);
            return uiVisualizerService.Show(vm, completedProc);
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
        public static Task<bool?> ShowDialog<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
            where TViewModel : IViewModel
        {
            Argument.IsNotNull("uiVisualizerService", uiVisualizerService);

            var viewModelFactory = GetViewModelFactory(uiVisualizerService);
            var vm = viewModelFactory.CreateViewModel(typeof(TViewModel), model);
            return uiVisualizerService.ShowDialog(vm, completedProc);
        }

        private static IViewModelFactory GetViewModelFactory(IUIVisualizerService uiVisualizerService)
        {
            var dependencyResolver = uiVisualizerService.GetDependencyResolver();
            var viewModelFactory = dependencyResolver.Resolve<IViewModelFactory>();
            return viewModelFactory;
        }
    }
}

#endif