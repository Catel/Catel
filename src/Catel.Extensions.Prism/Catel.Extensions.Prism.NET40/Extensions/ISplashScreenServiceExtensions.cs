// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISplashScreenServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.MVVM.Services;

    /// <summary>
    /// The splash screen service extensions.
    /// </summary>
    public static class ISplashScreenServiceExtensions
    {
        #region Methods
        /// <summary>
        /// The commit asyc.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="this">The splash screen service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <param name="completedCallback">The completed callback.</param>
        public static void CommitAsync<TViewModel>(this ISplashScreenService @this, TViewModel viewModel, string regionName, Action completedCallback = null) where TViewModel : IProgressNotifyableViewModel
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var uiVisualizerService = dependencyResolver.Resolve<IUIVisualizerService>();

            uiVisualizerService.Activate(viewModel, regionName);

            @this.CommitAsync(viewModel: viewModel, show: false, completedCallback: completedCallback);
        }

        /// <summary>
        /// The commit asyc.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="this">The splash screen service.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <param name="completedCallback">The completed callback.</param>
        public static void CommitAsync<TViewModel>(this ISplashScreenService @this, TViewModel viewModel, IViewModel parentViewModel, string regionName, Action completedCallback = null)
            where TViewModel : IProgressNotifyableViewModel
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var uiVisualizerService = dependencyResolver.Resolve<IUIVisualizerService>();

            uiVisualizerService.Activate(viewModel, parentViewModel, regionName);

            @this.CommitAsync(viewModel: viewModel, show: false, completedCallback: completedCallback);
        }
        #endregion
    }
}