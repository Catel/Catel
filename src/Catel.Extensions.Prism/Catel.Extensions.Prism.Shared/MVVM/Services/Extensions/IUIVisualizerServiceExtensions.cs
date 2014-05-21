// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIVisualizerServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Threading;

    using IoC;

    using Microsoft.Practices.Prism.Regions;

    using MVVM;
    using MVVM.Views;

    /// <summary>
    /// Extension methods for the <see cref="IUIVisualizerService" />.
    /// </summary>
    public static class IUIVisualizerServiceExtensions
    {
        #region Methods
        private static T ResolveTypeFromContainer<T>()
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            return dependencyResolver.Resolve<T>();
        }

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> via <see cref="IRegionManager" /> from a given view model.
        /// </summary>
        /// <param name="this">The <see cref="IUIVisualizerService" /> service self instance.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="NotSupportedException">If the implementation of IRegionManager is not registered in the IoC container</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        [ObsoleteEx(RemoveInVersion = "5.0", TreatAsErrorFromVersion = "4.2", Replacement = "IUICompositionService.Activate(viewModel, regionName)")]
        public static void Activate(this IUIVisualizerService @this, IViewModel viewModel, string regionName = null)
        {
            Argument.IsNotNull("@this", @this);

            var uiCompositionService = ResolveTypeFromContainer<IUICompositionService>();
            uiCompositionService.Activate(viewModel, regionName);
        }

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> via <see cref="IRegionManager" /> from a given view model.
        /// </summary>
        /// <param name="this">The <see cref="IUIVisualizerService" /> service self instance.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="parentViewModel">The parent view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="System.InvalidOperationException">The <paramref name="viewModel" /> and <paramref name="parentViewModel" /> are equals.</exception>
        /// <exception cref="NotSupportedException">If the implementation of IRegionManager is not registered in the IoC container</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="regionName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        [ObsoleteEx(RemoveInVersion = "5.0", TreatAsErrorFromVersion = "4.2", Replacement = "IUICompositionService.Activate(viewModel, parentViewModel, regionName)")]
        public static void Activate(this IUIVisualizerService @this, IViewModel viewModel, IViewModel parentViewModel, string regionName)
        {
            Argument.IsNotNull("@this", @this);

            var uiCompositionService = ResolveTypeFromContainer<IUICompositionService>();
            uiCompositionService.Activate(viewModel, parentViewModel, regionName);
        }

        /// <summary>
        /// Deactivates the views that belongs to the <paramref name="viewModel" /> instance.
        /// </summary>
        /// <param name="this">The <see cref="IUIVisualizerService" /> service self instance.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentException">If <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        [ObsoleteEx(RemoveInVersion = "5.0", TreatAsErrorFromVersion = "4.2", Replacement = "IUICompositionService.Activate(viewModel, parentViewModel, regionName)")]
        public static void Deactivate(this IUIVisualizerService @this, IViewModel viewModel)
        {
            Argument.IsNotNull("@this", @this);

            var uiCompositionService = ResolveTypeFromContainer<IUICompositionService>();
            uiCompositionService.Deactivate(viewModel);
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="this">The <see cref="IUIVisualizerService" /> service self instance.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="openedProc">The callback procedure that will be invoked when the window is opened (registered in the <see cref="IViewManager" />). This value can be <c>null</c>.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <param name="timeOutInMilliseconds">The time out in milliseconds.</param>
        /// <returns><c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="this" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        /// <exception cref="ViewModelNotRegisteredException">The <paramref name="viewModel" /> is not registered by the
        /// <see cref="IUIVisualizerService.Register(System.Type,System.Type)" />
        /// method first.</exception>
        /// <remarks>If the <see cref="IViewManager.GetViewsOfViewModel" /> method returns no active views for the <paramref name="viewModel" /> in the expected <paramref name="timeOutInMilliseconds" /> time
        /// then this method will assume that the view is actually opened and invokes <paramref name="openedProc" /> anyway.</remarks>
        [CLSCompliant(false)]
        public static bool Show(this IUIVisualizerService @this, IViewModel viewModel, Action openedProc = null, EventHandler<UICompletedEventArgs> completedProc = null, uint timeOutInMilliseconds = 10000)
        {
            Argument.IsNotNull("@this", @this);

            bool result = @this.Show(viewModel, completedProc);

            if (result && openedProc != null)
            {
                var startTime = DateTime.Now;
                ThreadPool.QueueUserWorkItem(state =>
                    {
                        var viewManager = ResolveTypeFromContainer<IViewManager>();
                        while (viewManager.GetViewsOfViewModel(viewModel).Length == 0 && DateTime.Now.Subtract(startTime).TotalMilliseconds < timeOutInMilliseconds)
                        {
                            ThreadHelper.Sleep(100);
                        }

                        var dispatcherService = ResolveTypeFromContainer<IDispatcherService>();
                        dispatcherService.Invoke(openedProc);
                    });
            }

            return result;
        }

        #endregion
    }
}