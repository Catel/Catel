// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIVisualizerServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using Caching;
    
    using IoC;
    
    using Logging;

    using Microsoft.Practices.Prism.Regions;

    using MVVM;
    using MVVM.Services;
    using MVVM.Views;
    
    using Windows.Controls;
    
    /// <summary>
    ///     Extension methods for the <see cref="IUIVisualizerService" />.
    /// </summary>
    public static class IUIVisualizerServiceExtensions
    {
        #region Constants

        /// <summary>
        ///  Activation required invalid operation error message.
        /// </summary>
        private const string ActivationRequiredInvalidOperationErrorMessage = "The 'viewModel' must be show at least one time in a region";

        /// <summary>
        /// Reference equals invalid operation exception message.
        /// </summary>
        private const string ReferenceEqualsInvalidOperationExceptionMessage = "The reference of 'viewModel' and 'parentViewModel' are equals, so can't activate a view model into it self";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     The cache storage.
        /// </summary>
        private static readonly ICacheStorage<int, IViewInfo> ViewInfoCacheStorage = new CacheStorage<int, IViewInfo>();
        #endregion

        #region Methods

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion"/> via <see cref="IRegionManager"/> from a given view model.
        /// </summary>
        /// <param name="this">
        /// The <see cref="IUIVisualizerService"/> service self instance.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="regionName"/> is <c>null</c> and the <paramref name="viewModel"/> was no show at least one time in a <see cref="IRegion"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the view that belongs to the <paramref name="viewModel"/> is already the logical child of another element.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the implementation of <see cref="IRegionManager"/> is not registered in the IoC container.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the implementation of IRegionManager is not registered in the IoC container
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Activate(this IUIVisualizerService @this, IViewModel viewModel, string regionName = null)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("viewModel", viewModel);

            if (string.IsNullOrEmpty(regionName))
            {
                Reactivate(viewModel);
            }
            else
            {
                Activate(viewModel, regionName, viewModel.GetService<IRegionManager>());
            }
        }

        /// <summary>
        /// Activates a view into a specific <see cref="IRegion"/> via <see cref="IRegionManager"/> from a given view model.
        /// </summary>
        /// <param name="this">
        /// The <see cref="IUIVisualizerService"/> service self instance.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="parentViewModel">
        /// The parent view model.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If <paramref name="regionName"/> is <c>null</c> and the <paramref name="viewModel"/> was no show at least one time in a <see cref="IRegion"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the view that belongs to the <paramref name="viewModel"/> is already the logical child of another element.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the implementation of <see cref="IRegionManager"/> is not registered in the IoC container.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The <paramref name="viewModel"/> and <paramref name="parentViewModel"/> are equals.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// If the implementation of IRegionManager is not registered in the IoC container
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="parentViewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The <paramref name="regionName"/> is <c>null</c> or whitespace.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Activate(this IUIVisualizerService @this, IViewModel viewModel, IViewModel parentViewModel, string regionName)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("viewModel", viewModel);
            Argument.IsNotNull("parentViewModel", parentViewModel);
            Argument.IsNotNullOrWhitespace("regionName", regionName);

            if (ReferenceEquals(viewModel, parentViewModel))
            {
                var exception = new InvalidOperationException(ReferenceEqualsInvalidOperationExceptionMessage);

                Log.Error(exception);

                throw exception;
            }

            var serviceLocator = viewModel.GetService<IServiceLocator>();

            var viewManager = serviceLocator.ResolveType<IViewManager>();
            IView[] viewsOfParentViewModel = viewManager.GetViewsOfViewModel(parentViewModel);
            IRegionInfo regionInfo = viewsOfParentViewModel.OfType<DependencyObject>().Select(dependencyObject => dependencyObject.GetRegionInfo(regionName, serviceLocator)).FirstOrDefault(info => info != null);
            if (regionInfo != null)
            {
                Activate(viewModel, regionInfo);
                
                if (parentViewModel is IRelationalViewModel)
                {
                    (parentViewModel as IRelationalViewModel).RegisterChildViewModel(viewModel);
                }

                if (viewModel is IRelationalViewModel)
                {
                    (viewModel as IRelationalViewModel).SetParentViewModel(parentViewModel);
                }                
            }
        }
        
        /// <summary>
  		/// Activates a view into a specific <see cref="IRegion"/> via <see cref="IRegionManager"/> from a given view model.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="regionInfo">
        /// The region info.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If the implementation of <see cref="IRegionManager"/> is not registered in the IoC container.
        /// </exception>
        private static void Activate(IViewModel viewModel, IRegionInfo regionInfo)
        {
         	Activate(viewModel, regionInfo.RegionName, regionInfo.RegionManager);
        }

        /// <summary>
  		/// Activates a view into a specific <see cref="IRegion"/> via <see cref="IRegionManager"/> from a given view model.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="regionName">
        /// The region name.
        /// </param>
        /// <param name="regionManager">
        /// The region manager.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// If the implementation of <see cref="IRegionManager"/> is not registered in the IoC container.
        /// </exception>
        private static void Activate(IViewModel viewModel, string regionName, IRegionManager regionManager)
        {
            if (regionManager != null && regionManager.Regions.ContainsRegionWithName(regionName))
            {
                var viewLocator = viewModel.GetService<IViewLocator>();
                if (viewLocator != null)
                {
                    IViewInfo viewInfo = ViewInfoCacheStorage.GetFromCacheOrFetch(viewModel.UniqueIdentifier, () => new ViewInfo(ViewHelper.ConstructViewWithViewModel(viewLocator.ResolveView(viewModel.GetType()), viewModel), regionManager.Regions[regionName]));

                    IRegion region = viewInfo.Region;
                    FrameworkElement view = viewInfo.View;

                    var dispatcherService = viewModel.GetService<IDispatcherService>();
                    dispatcherService.Invoke(() =>
                        {
                            if (!region.ActiveViews.Contains(view))
                            {
                                if (!region.Views.Contains(view))
                                {
                                    region.Add(view);
                                    viewModel.Closed += ViewModelOnClosed;
                                }

                                region.Activate(view);
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Reactivates a view from its viewmodel reference.
        /// </summary>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="viewModel"/> was no show at least one time in a <see cref="IRegion"/>.
        /// </exception>
        private static void Reactivate(IViewModel viewModel)
        {
            IViewInfo viewInfo = ViewInfoCacheStorage[viewModel.UniqueIdentifier];
            if (viewInfo == null)
            {
                throw new InvalidOperationException(ActivationRequiredInvalidOperationErrorMessage);
            }

            FrameworkElement view = viewInfo.View;
            IRegion region = viewInfo.Region;

            var dispatcherService = viewModel.GetService<IDispatcherService>();
            dispatcherService.Invoke(() => region.Activate(view));
        }

        /// <summary>
        /// The view model on closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="viewModelClosedEventArgs">
        /// The view model closed event args.
        /// </param>
        private static void ViewModelOnClosed(object sender, ViewModelClosedEventArgs viewModelClosedEventArgs)
        {
            var viewModel = (IViewModel)sender;
            viewModel.Closed -= ViewModelOnClosed;
            Deactivate(viewModel);
        }

        /// <summary>
        /// Deactivates the views that belongs to the <paramref name="viewModel"/> instance.
        /// </summary>
        /// <param name="this">
        /// The <see cref="IUIVisualizerService"/> service self instance.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="viewModel"/> was no show at least one time in a <see cref="IRegion"/>.
        /// </exception>
        public static void Deactivate(this IUIVisualizerService @this, IViewModel viewModel)
        {
            Deactivate(viewModel);
        }

        /// <summary>
        /// Deactivates the views that belongs to the <paramref name="viewModel"/> instance.
        /// </summary>
        /// <param name="viewModel">
        /// The view model instance.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="viewModel"/> was no show at least one time in a <see cref="IRegion"/>.
        /// </exception>
        private static void Deactivate(IViewModel viewModel)
        {
            Argument.IsNotNull("viewModel", viewModel);

            IViewInfo viewInfo = ViewInfoCacheStorage[viewModel.UniqueIdentifier];
            if (viewInfo == null)
            {
                throw new InvalidOperationException(ActivationRequiredInvalidOperationErrorMessage);
            }

            FrameworkElement view = viewInfo.View;
            IRegion region = viewInfo.Region;

            var dispatcherService = viewModel.GetService<IDispatcherService>();
            dispatcherService.Invoke(() =>
                {
                    region.Deactivate(view);
                    if (viewModel.IsClosed)
                    {
                        ViewInfoCacheStorage.Remove(viewModel.UniqueIdentifier, () => region.Remove(view));
                    }
                });
        }

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="this">
        /// The <see cref="IUIVisualizerService"/> service self instance.
        /// </param>
        /// <param name="viewModel">
        /// The view model.
        /// </param>
        /// <param name="openedProc">
        /// The callback procedure that will be invoked when the window is opened (registered in the <see cref="IViewManager"/>). This value can be <c>null</c>.
        /// </param>
        /// <param name="completedProc">
        /// The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.
        /// </param>
        /// <param name="timeOutInMilliseconds">
        /// The time out in milliseconds.
        /// </param>
        /// <returns>
        /// <c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="viewModel"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ViewModelNotRegisteredException">
        /// The <paramref name="viewModel"/> is not registered by the
        ///     <see cref="IUIVisualizerService.Register(System.Type,System.Type)"/>
        ///     method first.
        /// </exception>
        /// <remarks>
        /// If the <see cref="IViewManager.GetViewsOfViewModel"/> method returns no active views for the <paramref name="viewModel"/> in the expected <paramref name="timeOutInMilliseconds"/> time
        /// then this method will assume that the view is actually opened and invokes <paramref name="openedProc"/> anyway.
        /// </remarks>
        [CLSCompliant(false)]
        public static bool Show(this IUIVisualizerService @this, IViewModel viewModel, Action openedProc = null, EventHandler<UICompletedEventArgs> completedProc = null, uint timeOutInMilliseconds = 10000)
        {
            Argument.IsNotNull("@this", @this);

            bool result = @this.Show(viewModel, completedProc);

            if (result && openedProc != null)
            {
				DateTime startTime = DateTime.Now;
                ThreadPool.QueueUserWorkItem(state =>
                    {
                        var viewManager = viewModel.GetService<IViewManager>();
                        while (viewManager.GetViewsOfViewModel(viewModel).Length == 0 && DateTime.Now.Subtract(startTime).TotalMilliseconds < timeOutInMilliseconds)
                        {
                            ThreadHelper.Sleep(100); 
                        }

                        var dispatcherService = viewModel.GetService<IDispatcherService>();
                        dispatcherService.Invoke(openedProc);
                    });
            }

            return result;
        }

        #endregion

        #region Nested type: ViewInfo

        /// <summary>
        /// The view region item.
        /// </summary>
        internal class ViewInfo : Tuple<FrameworkElement, IRegion>, IViewInfo
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewInfo"/> class.
            /// </summary>
            /// <param name="view">
            /// The view.
            /// </param>
            /// <param name="region">
            /// The region.
            /// </param>
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
                get
                {
                    return this.Item1;
                }
            }

            /// <summary>
            /// Gets Region.
            /// </summary>
            IRegion IViewInfo.Region
            {
                get
                {
                    return this.Item2;
                }
            }
            #endregion
        }
        #endregion
    }
}