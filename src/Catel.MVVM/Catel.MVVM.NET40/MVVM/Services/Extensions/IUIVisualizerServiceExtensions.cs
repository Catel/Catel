// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIVisualizerServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Services
{
    using System;
    using Catel.IoC;

    /// <summary>
    /// Extension methods for the <see cref="IUIVisualizerService"/>.
    /// </summary>
    public static class IUIVisualizerServiceExtensions
    {
        /// <summary>
        /// Shows the window in non-modal state and creates the view model automatically using the specified model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="uiVisualizerService">The UI visualizer service.</param>
        /// <param name="model">The model to be injected into the view model, can be <c>null</c>.</param>
        /// <param name="completedProc">The completed proc.</param>
        /// <returns><c>true</c> if shown successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="uiVisualizerService" /> is <c>null</c>.</exception>
        public static bool Show<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
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
        public static bool? ShowDialog<TViewModel>(this IUIVisualizerService uiVisualizerService, object model = null, EventHandler<UICompletedEventArgs> completedProc = null)
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