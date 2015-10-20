// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompositionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    using Catel.Caching;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.MVVM.Views;

#if PRISM6
    using Prism.Regions;
#else
    using Microsoft.Practices.Prism.Regions;
#endif

    using Reflection;
    using Threading;

    /// <summary>
    /// The user interface composition service.
    /// </summary>
    public sealed class UICompositionService : ViewModelServiceBase, IUICompositionService
    {
        #region Constants
        /// <summary>
        /// Reference equals invalid operation exception message.
        /// </summary>
        private const string ReferenceEqualsInvalidOperationExceptionMessage = "The reference of 'viewModel' and 'parentViewModel' are equals, so can't activate a view model into it self";

        /// <summary>
        ///  Activation required invalid operation error message.
        /// </summary>
        private const string ActivationRequiredInvalidOperationErrorMessage = "The 'viewModel' must be show at least one time in a region";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The cache storage.
        /// </summary>
        private static readonly ICacheStorage<int, IViewInfo> ViewInfoCacheStorage = new CacheStorage<int, IViewInfo>();
        #endregion

        #region Fields
        private readonly IRegionManager _regionManager;
        private readonly IViewManager _viewManager;
        private readonly IViewLocator _viewLocator;
        private readonly IDispatcherService _dispatcherService;
        private readonly IViewModelManager _viewModelManager;
        private readonly IViewModelFactory _viewModelFactory;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UICompositionService" /> class.
        /// </summary>
        /// <param name="regionManager">The region manager</param>
        /// <param name="viewManager">The view manager</param>
        /// <param name="viewLocator">The view locator</param>
        /// <param name="dispatcherService">The dispatcher service</param>
        /// <param name="viewModelManager">The view model manager.</param>
        /// <param name="viewModelFactory">The view model factory.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="regionManager" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewManager" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewLocator" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="dispatcherService" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModelManager" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModelFactory" /> is <c>null</c>.</exception>
        public UICompositionService(IRegionManager regionManager, IViewManager viewManager, IViewLocator viewLocator, 
            IDispatcherService dispatcherService, IViewModelManager viewModelManager, IViewModelFactory viewModelFactory)
        {
            Argument.IsNotNull(() => regionManager);
            Argument.IsNotNull(() => viewManager);
            Argument.IsNotNull(() => viewLocator);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => viewModelManager);
            Argument.IsNotNull(() => viewModelFactory);

            _regionManager = regionManager;
            _viewManager = viewManager;
            _viewLocator = viewLocator;
            _dispatcherService = dispatcherService;
            _viewModelManager = viewModelManager;
            _viewModelFactory = viewModelFactory;
        }
        #endregion

        #region IUICompositionService Members

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> via <see cref="IRegionManager" /> from a given view model.
        /// </summary>
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
        public void Activate(IViewModel viewModel, IViewModel parentViewModel, string regionName)
        {
            Argument.IsNotNull(() => viewModel);
            Argument.IsNotNull(() => parentViewModel);
            Argument.IsNotNullOrWhitespace(() => regionName);

            if (ReferenceEquals(viewModel, parentViewModel))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>(ReferenceEqualsInvalidOperationExceptionMessage);
            }

            var viewsOfParentViewModel = _viewManager.GetViewsOfViewModel(parentViewModel);
            var regionInfo = viewsOfParentViewModel.OfType<DependencyObject>().Select(dependencyObject => dependencyObject.GetRegionInfo(regionName)).FirstOrDefault(info => info != null);
            if (regionInfo != null)
            {
                Activate(viewModel, regionInfo);

                var parentRelationalViewModel = parentViewModel as IRelationalViewModel;
                if (parentRelationalViewModel != null)
                {
                    parentRelationalViewModel.RegisterChildViewModel(viewModel);
                }

                var relationalViewModel = viewModel as IRelationalViewModel;
                if (relationalViewModel != null)
                {
                    relationalViewModel.SetParentViewModel(parentViewModel);
                }
            }
        }

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> from a given view model and the region name.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="regionName">The region name.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="regionName" /> is <c>null</c> and the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="NotSupportedException">If the implementation of IRegionManager is not registered in the IoC container</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel" /> is <c>null</c>.</exception>
        public void Activate(IViewModel viewModel, string regionName = null)
        {
            Argument.IsNotNull(() => viewModel);

            if (string.IsNullOrEmpty(regionName))
            {
                Reactivate(viewModel);
            }
            else
            {
                Activate(viewModel, regionName, this._regionManager);
            }
        }

        /// <summary>
        /// Tries to activate an existing view model in the specified region name. If there is no view model alive, it will create one
        /// and navigate to that view model.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="regionName">Name of the region.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModelType" /> is <c>null</c>.</exception>
        public void Activate(Type viewModelType, string regionName)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNullOrWhitespace("regionName", regionName);

            var activeVm = (from vm in _viewModelManager.ActiveViewModels
                            where viewModelType == vm.GetType()
                            select vm).FirstOrDefault();
            if (activeVm == null)
            {
                Log.Debug("Could not find active instance of '{0}', trying to instantiate", viewModelType.GetSafeFullName());

                activeVm = _viewModelFactory.CreateViewModel(viewModelType, null);
            }

            if (activeVm == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Could not find an existing instance of '{0}' and could not construct it using the IViewModelFactory. It is impossible to activate this view model.", viewModelType.GetSafeFullName());
            }

            Activate(activeVm, regionName);
        }

        /// <summary>
        /// Deactivates the views that belongs to the <paramref name="viewModel" /> instance.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="InvalidOperationException">If the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        public void Deactivate(IViewModel viewModel)
        {
            Argument.IsNotNull(() => viewModel);

            Log.Debug("Deactivating '{0}'", viewModel.GetType().GetSafeFullName());

            var viewInfo = ViewInfoCacheStorage[viewModel.UniqueIdentifier];
            if (viewInfo == null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>(ActivationRequiredInvalidOperationErrorMessage);
            }

            var view = viewInfo.View;
            var region = viewInfo.Region;

            _dispatcherService.Invoke(() =>
            {
                region.Deactivate(view);
                if (viewModel.IsClosed)
                {
                    ViewInfoCacheStorage.Remove(viewModel.UniqueIdentifier, () => region.Remove(view));
                }
            }, true);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> from a given view model and the <see cref="IRegionInfo"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="regionInfo">The region info</param>
        private void Activate(IViewModel viewModel, IRegionInfo regionInfo)
        {
            Activate(viewModel, regionInfo.RegionName, regionInfo.RegionManager);
        }

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion" /> from a given view model, the region name and the <see cref="RegionManager"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="regionName">The region name</param>
        /// <param name="regionManager">The region Manager</param>
        private void Activate(IViewModel viewModel, string regionName, IRegionManager regionManager)
        {
            Log.Debug("Activating '{0}' in region '{1}'", viewModel.GetType().GetSafeFullName(), regionName);

            if (regionManager != null && regionManager.Regions.ContainsRegionWithName(regionName))
            {
                var viewInfo = ViewInfoCacheStorage.GetFromCacheOrFetch(viewModel.UniqueIdentifier, () => new ViewInfo(ViewHelper.ConstructViewWithViewModel(this._viewLocator.ResolveView(viewModel.GetType()), viewModel), regionManager.Regions[regionName]));

                var region = viewInfo.Region;
                var view = viewInfo.View;

                _dispatcherService.Invoke(() =>
                {
                    if (!region.ActiveViews.Contains(view))
                    {
                        if (!region.Views.Contains(view))
                        {
                            region.Add(view);
                            viewModel.ClosedAsync += ViewModelOnClosedAsync;
                        }

                        region.Activate(view);
                    }
                }, true);
            }
        }

        /// <summary>
        /// Called when a view model is closed.
        /// </summary>
        /// <param name="sender">The view model</param>
        /// <param name="e">The event args</param>
        private Task ViewModelOnClosedAsync(object sender, ViewModelClosedEventArgs e)
        {
            var viewModel = (IViewModel)sender;
            viewModel.ClosedAsync -= ViewModelOnClosedAsync;
            Deactivate(viewModel);

            return TaskHelper.Completed;
        }

        /// <summary>
        /// Reactivates a view from its viewmodel reference.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="InvalidOperationException">If the <paramref name="viewModel" /> was no show at least one time in a <see cref="IRegion" />.</exception>
        private void Reactivate(IViewModel viewModel)
        {
            var viewInfo = ViewInfoCacheStorage[viewModel.UniqueIdentifier];
            if (viewInfo == null)
            {
                throw new InvalidOperationException(ActivationRequiredInvalidOperationErrorMessage);
            }

            var view = viewInfo.View;
            var region = viewInfo.Region;

            _dispatcherService.Invoke(() => region.Activate(view), true);
        }
        #endregion

        #region Nested type: ViewInfo
        /// <summary>
        /// The view region item.
        /// </summary>
        private class ViewInfo : Tuple<FrameworkElement, IRegion>, IViewInfo
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ViewInfo" /> class.
            /// </summary>
            /// <param name="view">The view.</param>
            /// <param name="region">The region.</param>
            public ViewInfo(FrameworkElement view, IRegion region)
                : base(view, region)
            {
            }
            #endregion

            #region IViewInfo Members
            /// <summary>
            /// Gets View.
            /// </summary>
            FrameworkElement IViewInfo.View
            {
                get { return Item1; }
            }

            /// <summary>
            /// Gets Region.
            /// </summary>
            IRegion IViewInfo.Region
            {
                get { return Item2; }
            }
            #endregion
        }
        #endregion
    }
}