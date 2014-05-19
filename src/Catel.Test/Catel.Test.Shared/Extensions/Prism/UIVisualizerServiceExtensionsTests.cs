// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIVisualizerServiceExtensionsTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.Prism
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;
    using Microsoft.Practices.Prism.Regions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
#endif

    /// <summary>
    /// The ui visualizer service extensions tests.
    /// </summary>
    public class UIVisualizerServiceExtensionsTests
    {
        #region Constants
        /// <summary>
        /// The main region name.
        /// </summary>
        private const string MainRegionName = "MainRegion";
        #endregion

        #region Nested type: FooParentViewModel
        /// <summary>
        /// The foo parent view model.
        /// </summary>
        public class FooParentViewModel : ViewModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FooParentViewModel"/> class.
            /// </summary>
            /// <param name="serviceLocator">
            /// The service locator.
            /// </param>
            public FooParentViewModel(IServiceLocator serviceLocator)
                : base(serviceLocator)
            {
            }
            #endregion

            #region Methods
            public IEnumerable<IViewModel> GetChildViewModelsWrapper()
            {
                return GetChildViewModels();
            }
            #endregion
        }
        #endregion

        #region Nested type: FooParentViewModelView
        /// <summary>
        /// The foo parent view model view.
        /// </summary>
        public class FooParentViewModelView : UserControl
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FooParentViewModelView"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The view model.
            /// </param>
            public FooParentViewModelView(IViewModel viewModel)
                : base(viewModel)
            {
            }
            #endregion
        }
        #endregion

        #region Nested type: FooViewModel
        /// <summary>
        /// The foo view model.
        /// </summary>
        public class FooViewModel : ViewModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FooViewModel"/> class.
            /// </summary>
            /// <param name="serviceLocator">
            /// The service locator.
            /// </param>
            public FooViewModel(IServiceLocator serviceLocator)
                : base(serviceLocator)
            {
            }
            #endregion

            #region Methods
            /// <summary>
            /// The get parent view model.
            /// </summary>
            /// <returns>
            /// The <see cref="IViewModel"/>.
            /// </returns>
            public IViewModel GetParentViewModel()
            {
                return ParentViewModel;
            }
            #endregion
        }
        #endregion

        #region Nested type: FooViewModelView
        /// <summary>
        /// The foo view model view.
        /// </summary>
        public class FooViewModelView : UserControl
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="FooViewModelView"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The view model.
            /// </param>
            public FooViewModelView(IViewModel viewModel)
                : base(viewModel)
            {
            }
            #endregion
        }
        #endregion

        #region Nested type: TheActivateMethod
        /// <summary>
        /// The the show method.
        /// </summary>
        [TestClass]
        public class TheActivateMethod
        {
            #region Fields
            /// <summary>
            /// The active view collection.
            /// </summary>
            private Mock<IViewsCollection> _activeViewCollection;

            /// <summary>
            /// The main region mock.
            /// </summary>
            private Mock<IRegion> _mainRegionMock;

            /// <summary>
            /// The region collection mock.
            /// </summary>
            private Mock<IRegionCollection> _regionCollectionMock;

            /// <summary>
            /// The region manager mock.
            /// </summary>
            private Mock<IRegionManager> _regionManagerMock;

            /// <summary>
            /// The service locator.
            /// </summary>
            private IServiceLocator _serviceLocator;

            /// <summary>
            /// The view collection.
            /// </summary>
            private Mock<IViewsCollection> _viewCollection;
            #endregion

            #region Methods
            /// <summary>
            /// The init.
            /// </summary>
            [TestInitialize]
            public void Init()
            {
                _regionManagerMock = new Mock<IRegionManager>();
                _activeViewCollection = new Mock<IViewsCollection>();
                _viewCollection = new Mock<IViewsCollection>();
                _regionCollectionMock = new Mock<IRegionCollection>();
                _mainRegionMock = new Mock<IRegion>();

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());

                //_serviceLocator = IoCFactory.CreateServiceLocator(true);
                _serviceLocator = ServiceLocator.Default;
                _serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);

                _serviceLocator.RegisterType<IViewLocator, ViewLocator>();
                _serviceLocator.RegisterType<IViewModelLocator, ViewModelLocator>();
                _serviceLocator.RegisterType<IUIVisualizerService, UIVisualizerService>();
                _serviceLocator.RegisterType<IUICompositionService, UICompositionService>();
            }

            ///// <summary>
            ///// The throws not supported exception if the region manager is not avaliable.
            ///// </summary>
            //[TestMethod]
            //public void ThrowsNotSupportedExceptionIfTheRegionManagerIsNotAvaliable()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();
            //    serviceLocator.RegisterType<IViewLocator, ViewLocator>();
            //    serviceLocator.RegisterType<IViewModelLocator, ViewModelLocator>();
            //    serviceLocator.RegisterType<IUIVisualizerService, UIVisualizerService>();

            //    var fooViewModel = new FooViewModel(serviceLocator);

            //    var viewLocator = serviceLocator.ResolveType<IViewLocator>();
            //    var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();

            //    viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
            //    viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

            //    var uiVisualizerService = serviceLocator.ResolveType<IUIVisualizerService>();
            //    ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => uiVisualizerService.Activate(fooViewModel, MainRegionName));
            //}

            /// <summary>
            /// The throws invalid operation if is the first activation of a view model is requested with out region.
            /// </summary>
            [TestMethod]
            public void ThrowsInvalidOperationIfIsTheFirstActivationOfAViewModelIsRequestedWithOutRegion()
            {
                _serviceLocator.RegisterInstance(_regionManagerMock.Object);
                var fooViewModel = new FooViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uiVisualizerService.Activate(fooViewModel));
            }

            /// <summary>
            /// The test.
            /// </summary>
            [TestMethod]
            public void CallsAddAndActivateMethodsOfAnExistingRegionWithTheViewThatsBelongToTheViewModel()
            {
                SetupDefaultRegionManager();

                var fooViewModel = new FooViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                uiVisualizerService.Activate(fooViewModel, MainRegionName);

                VerifyDefaultRegionManagerBehavior();
            }

            /// <summary>
            /// The allows the reactivation of a deactivated view model.
            /// </summary>
            [TestMethod]
            public void AllowsTheReactivationOfADeactivatedViewModel()
            {
                SetupDefaultRegionManager();

                var fooViewModel = new FooViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                uiVisualizerService.Activate(fooViewModel, MainRegionName);

                uiVisualizerService.Deactivate(fooViewModel);

                uiVisualizerService.Activate(fooViewModel);

                _regionManagerMock.VerifyGet(collection => collection.Regions, Times.AtLeast(1));
                _regionCollectionMock.Verify(collection => collection.ContainsRegionWithName(MainRegionName), Times.Once());
                _regionCollectionMock.Verify(collection => collection[MainRegionName], Times.Once());
                _mainRegionMock.Verify(region => region.Add(It.IsAny<FooViewModelView>()), Times.Once());
                _mainRegionMock.Verify(region => region.Deactivate(It.IsAny<FooViewModelView>()), Times.Once());
                _mainRegionMock.Verify(region => region.Activate(It.IsAny<FooViewModelView>()), Times.Exactly(2));
            }

            /// <summary>
            /// The test.
            /// </summary>
            [TestMethod]
            public void NotCallsAddOrActivateMethodsOfViewThatsBelongToTheViewModelIsAlreadyActive()
            {
                _activeViewCollection.Setup(collection => collection.Contains(It.IsAny<FooViewModelView>())).Returns(true);

                _mainRegionMock.SetupGet(region => region.ActiveViews).Returns(_activeViewCollection.Object);
                _mainRegionMock.SetupGet(region => region.Views).Returns(_viewCollection.Object);

                _regionCollectionMock.Setup(collection => collection.ContainsRegionWithName(MainRegionName)).Returns(true);
                _regionCollectionMock.SetupGet(collection => collection[MainRegionName]).Returns(_mainRegionMock.Object);

                _regionManagerMock.SetupGet(manager => manager.Regions).Returns(_regionCollectionMock.Object);

                _serviceLocator.RegisterInstance<IRegionManager>(_regionManagerMock.Object);
                var fooViewModel = new FooViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                uiVisualizerService.Activate(fooViewModel, MainRegionName);

                _regionManagerMock.VerifyGet(collection => collection.Regions, Times.AtLeast(1));
                _regionCollectionMock.Verify(collection => collection.ContainsRegionWithName(MainRegionName), Times.Once());
                _regionCollectionMock.Verify(collection => collection[MainRegionName], Times.Once());
                _mainRegionMock.Verify(region => region.Add(It.IsAny<FooViewModelView>()), Times.Never());
                _mainRegionMock.Verify(region => region.Activate(It.IsAny<FooViewModelView>()), Times.Never());
            }

            /// <summary>
            /// The tries to resolve the views of the parent view model using the view manager.
            /// </summary>
            [TestMethod]
            public void TriesToResolveTheViewsOfTheParentViewModelUsingTheViewManager()
            {
                var fooViewModel = new FooViewModel(_serviceLocator);
                var fooParentViewModel = new FooParentViewModel(_serviceLocator);

                var viewManagerMock = new Mock<IViewManager>();
                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooParentViewModel)).Returns(new IView[] {});

                _serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);

                _serviceLocator.ResolveType<IUIVisualizerService>().Activate(fooViewModel, fooParentViewModel, MainRegionName);

                viewManagerMock.Verify(manager => manager.GetViewsOfViewModel(fooParentViewModel), Times.Once());
            }

            /// <summary>
            /// The sets the existing region manager and creates a parent child relationship between the view model with the region and the injected one.
            /// </summary>
            [TestMethod]
            public void SetsTheExistingRegionManagerAndCreatesAParentChildRelationshipBetweenTheViewModelWithTheRegionAndTheInjectedOne()
            {
                SetupDefaultRegionManager();

                var fooViewModel = new FooViewModel(_serviceLocator);
                var fooParentViewModel = new FooParentViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var viewManagerMock = new Mock<IViewManager>();
                var fooParentViewModelView = new FooParentViewModelView(fooParentViewModel);
                fooParentViewModelView.SetRegionManager(_regionManagerMock.Object);
                RegionManager.SetRegionName(fooParentViewModelView, MainRegionName);

                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooParentViewModel)).Returns(new IView[] {fooParentViewModelView});

                _serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);
                _serviceLocator.ResolveType<IUIVisualizerService>().Activate(fooViewModel, fooParentViewModel, MainRegionName);

                VerifyDefaultRegionManagerBehavior();

                Assert.AreEqual(fooParentViewModel, fooViewModel.GetParentViewModel());
                Assert.IsTrue(fooParentViewModel.GetChildViewModelsWrapper().Contains(fooViewModel));
            }

            /// <summary>
            /// The make a call to resolve the region manager if not found in the visual tree and creates a parent child relationship between the view model with the region and the injected one.
            /// </summary>
            [TestMethod]
            public void MakeACallToResolveTheRegionManagerIfNotFoundInTheVisualTreeAndCreatesAParentChildRelationshipBetweenTheViewModelWithTheRegionAndTheInjectedOne()
            {
                SetupDefaultRegionManager();

                _regionManagerMock.Setup(manager => manager.CreateRegionManager()).Returns(_regionManagerMock.Object);

                var fooViewModel = new FooViewModel(_serviceLocator);
                var fooParentViewModel = new FooParentViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var viewManagerMock = new Mock<IViewManager>();
                var fooParentViewModelView = new FooParentViewModelView(fooParentViewModel);

                RegionManager.SetRegionName(fooParentViewModelView, MainRegionName);

                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooParentViewModel)).Returns(new IView[] {fooParentViewModelView});

                _serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);

                _serviceLocator.ResolveType<IUIVisualizerService>().Activate(fooViewModel, fooParentViewModel, MainRegionName);

                VerifyDefaultRegionManagerBehavior();

                Assert.AreEqual(fooParentViewModel, fooViewModel.GetParentViewModel());
                Assert.IsTrue(fooParentViewModel.GetChildViewModelsWrapper().Contains(fooViewModel));

                _regionManagerMock.Verify(manager => manager.CreateRegionManager(), Times.Once());
            }

            /// <summary>
            /// The throws argument null exception if view model is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionIfViewModelIsNull()
            {
                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => uiVisualizerService.Activate(null, MainRegionName));
            }

            /// <summary>
            /// The throws argument null exception if parent view model is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionIfParentViewModelIsNull()
            {
                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => uiVisualizerService.Activate(new FooViewModel(_serviceLocator), null, MainRegionName));
            }

            /// <summary>
            /// The throws invalid operation exception if view model and parent view model are reference equals.
            /// </summary>
            [TestMethod]
            public void ThrowsInvalidOperationExceptionIfViewModelAndParentViewModelAreReferenceEquals()
            {
                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                var fooViewModel = new FooViewModel(_serviceLocator);
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uiVisualizerService.Activate(fooViewModel, fooViewModel, MainRegionName));
            }

            /// <summary>
            /// The setup region manager behavior related with main region.
            /// </summary>
            private void SetupDefaultRegionManager(IServiceLocator serviceLocator = null)
            {
                if (serviceLocator == null)
                {
                    serviceLocator = _serviceLocator;
                }

                _activeViewCollection.Setup(collection => collection.Contains(It.IsAny<FooViewModelView>())).Returns(false);
                _viewCollection.Setup(collection => collection.Contains(It.IsAny<FooViewModelView>())).Returns(false);
                _mainRegionMock.SetupGet(region => region.ActiveViews).Returns(_activeViewCollection.Object);
                _mainRegionMock.SetupGet(region => region.Views).Returns(_viewCollection.Object);
                _regionCollectionMock.Setup(collection => collection.ContainsRegionWithName(MainRegionName)).Returns(true);
                _regionCollectionMock.SetupGet(collection => collection[MainRegionName]).Returns(_mainRegionMock.Object);
                _regionManagerMock.SetupGet(manager => manager.Regions).Returns(_regionCollectionMock.Object);

                serviceLocator.RegisterInstance<IRegionManager>(_regionManagerMock.Object);
            }

            /// <summary>
            /// The verify region manager behavior.
            /// </summary>
            private void VerifyDefaultRegionManagerBehavior()
            {
                _regionManagerMock.VerifyGet(collection => collection.Regions, Times.AtLeast(1));
                _regionCollectionMock.Verify(collection => collection.ContainsRegionWithName(MainRegionName), Times.Once());
                _regionCollectionMock.Verify(collection => collection[MainRegionName], Times.Once());
                _mainRegionMock.Verify(region => region.Add(It.IsAny<FooViewModelView>()), Times.Once());
                _mainRegionMock.Verify(region => region.Activate(It.IsAny<FooViewModelView>()), Times.Once());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheDeactivateMethod
        /// <summary>
        /// The the deactivate method.
        /// </summary>
        [TestClass]
        public class TheDeactivateMethod
        {
            #region Fields
            /// <summary>
            /// The active view collection.
            /// </summary>
            private Mock<IViewsCollection> _activeViewCollection;

            /// <summary>
            /// The main region mock.
            /// </summary>
            private Mock<IRegion> _mainRegionMock;

            /// <summary>
            /// The region collection mock.
            /// </summary>
            private Mock<IRegionCollection> _regionCollectionMock;

            /// <summary>
            /// The region manager mock.
            /// </summary>
            private Mock<IRegionManager> _regionManagerMock;

            /// <summary>
            /// The service locator.
            /// </summary>
            private IServiceLocator _serviceLocator;

            /// <summary>
            /// The view collection.
            /// </summary>
            private Mock<IViewsCollection> _viewCollection;
            #endregion

            #region Methods
            /// <summary>
            /// The init.
            /// </summary>
            [TestInitialize]
            public void Init()
            {
                _regionManagerMock = new Mock<IRegionManager>();
                _activeViewCollection = new Mock<IViewsCollection>();
                _viewCollection = new Mock<IViewsCollection>();
                _regionCollectionMock = new Mock<IRegionCollection>();
                _mainRegionMock = new Mock<IRegion>();

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());

                _serviceLocator = ServiceLocator.Default;
                _serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);

                _serviceLocator.RegisterType<IViewLocator, ViewLocator>();
                _serviceLocator.RegisterType<IViewModelLocator, ViewModelLocator>();
                _serviceLocator.RegisterType<IUIVisualizerService, UIVisualizerService>();
                _serviceLocator.RegisterType<IUICompositionService, UICompositionService>();
            }

            /// <summary>
            /// The setup region manager behavior related with main region.
            /// </summary>
            private void SetupRegionManagerBehaviorRelatedWithMainRegion()
            {
                _activeViewCollection.Setup(collection => collection.Contains(It.IsAny<FooViewModelView>())).Returns(false);
                _viewCollection.Setup(collection => collection.Contains(It.IsAny<FooViewModelView>())).Returns(false);
                _mainRegionMock.SetupGet(region => region.ActiveViews).Returns(_activeViewCollection.Object);
                _mainRegionMock.SetupGet(region => region.Views).Returns(_viewCollection.Object);
                _regionCollectionMock.Setup(collection => collection.ContainsRegionWithName(MainRegionName)).Returns(true);
                _regionCollectionMock.SetupGet(collection => collection[MainRegionName]).Returns(_mainRegionMock.Object);
                _regionManagerMock.SetupGet(manager => manager.Regions).Returns(_regionCollectionMock.Object);
                _serviceLocator.RegisterInstance<IRegionManager>(_regionManagerMock.Object);
            }

            /// <summary>
            /// The is called is the method is close and remove method of the region is called.
            /// </summary>
            [TestMethod]
            public void IsCalledIsTheMethodIsCloseAndRemoveMethodOfTheRegionIsCalled()
            {
                SetupRegionManagerBehaviorRelatedWithMainRegion();

                var fooViewModel = new FooViewModel(_serviceLocator);

                var viewLocator = _serviceLocator.ResolveType<IViewLocator>();
                var viewModelLocator = _serviceLocator.ResolveType<IViewModelLocator>();

                viewModelLocator.Register(typeof (FooViewModelView), typeof (FooViewModel));
                viewLocator.Register(typeof (FooViewModel), typeof (FooViewModelView));

                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                uiVisualizerService.Activate(fooViewModel, MainRegionName);

                fooViewModel.CloseViewModel(true);

                _mainRegionMock.Verify(region => region.Remove(It.IsAny<FooViewModelView>()), Times.Once());
            }

            /// <summary>
            /// The throws argument exception if the view model is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentExceptionIfTheViewModelIsNull()
            {
                _serviceLocator.RegisterInstance<IRegionManager>(_regionManagerMock.Object);
                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => uiVisualizerService.Deactivate(null));
            }

            /// <summary>
            /// The throws invalid operation exception if the view model is null.
            /// </summary>
            [TestMethod]
            public void ThrowsInvalidOperationExceptionIfTheViewModelIsNull()
            {
                var uiVisualizerService = _serviceLocator.ResolveType<IUIVisualizerService>();
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => uiVisualizerService.Deactivate(new FooViewModel(_serviceLocator)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheShowMethod
        /// <summary>
        /// The the show method.
        /// </summary>
        [TestClass]
        public class TheShowMethod
#if SILVERLIGHT
            : SilverlightTest
#endif
        {
#if SILVERLIGHT
            [TestMethod]
            [Asynchronous]
            public void TheOpenedActionIsCalledWhenViewManagerHaveRegisteredAViewForTheViewModel()
            {
                 var serviceLocator = IoCFactory.CreateServiceLocator();
                var fooViewModel = new FooViewModel(serviceLocator);

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());
                var visualizerServiceMock = new Mock<IUIVisualizerService>();
                visualizerServiceMock.Setup(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null)).Returns(true);
                var viewManagerMock = new Mock<IViewManager>();
                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooViewModel)).Returns(new IView[] { new FooViewModelView(fooViewModel) });
                
                serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);
            	serviceLocator.RegisterInstance<IUIVisualizerService>(visualizerServiceMock.Object);
            	serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);
            
                serviceLocator.ResolveType<IUIVisualizerService>().Show(fooViewModel, () =>
                    {
                        visualizerServiceMock.Verify(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null), Times.Once());
                        viewManagerMock.Verify(manager => manager.GetViewsOfViewModel(fooViewModel), Times.AtLeastOnce());  
                        this.EnqueueTestComplete();
                    });
            }

            [TestMethod]
            [Asynchronous]
            public void TheOpenedActionIsCalledEvenWhenThereNoViewsAvailablesInTheExpectedTimeForTheCurrentViewModelButUnlockingTheInspectionThread()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var fooViewModel = new FooViewModel(serviceLocator);

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());
                var visualizerServiceMock = new Mock<IUIVisualizerService>();
                visualizerServiceMock.Setup(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null)).Returns(true);
                var viewManagerMock = new Mock<IViewManager>();
                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooViewModel)).Returns(new IView[] { });

                serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);
                serviceLocator.RegisterInstance<IUIVisualizerService>(visualizerServiceMock.Object);
                serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);

                
                serviceLocator.ResolveType<IUIVisualizerService>().Show(fooViewModel, () =>
                    {
                        visualizerServiceMock.Verify(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null), Times.Once());
                        viewManagerMock.Verify(manager => manager.GetViewsOfViewModel(fooViewModel), Times.AtLeastOnce());
                        this.EnqueueTestComplete();
                    });
            }
            
#else

            /// <summary>
            /// The the opened action is called when view manager have registered a view for the view model.
            /// </summary>
            [TestMethod]
            public void TheOpenedActionIsCalledWhenViewManagerHaveRegisteredAViewForTheViewModel()
            {
                var serviceLocator = ServiceLocator.Default;
                var fooViewModel = new FooViewModel(serviceLocator);

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());
                var visualizerServiceMock = new Mock<IUIVisualizerService>();
                visualizerServiceMock.Setup(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null)).Returns(true);
                var viewManagerMock = new Mock<IViewManager>();
                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooViewModel)).Returns(new IView[] {new FooViewModelView(fooViewModel)});

                serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);
                serviceLocator.RegisterInstance<IUIVisualizerService>(visualizerServiceMock.Object);
                serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);

                var @event = new AutoResetEvent(false);

                serviceLocator.ResolveType<IUIVisualizerService>().Show(fooViewModel, () => @event.Set());

                @event.WaitOne(1000);

                visualizerServiceMock.Verify(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null), Times.Once());
                viewManagerMock.Verify(manager => manager.GetViewsOfViewModel(fooViewModel), Times.AtLeastOnce());
            }

            /// <summary>
            /// The the opened action is called even when there no views availables in the expected time for the current view model but unlocking the inspection thread.
            /// </summary>
            [TestMethod]
            public void TheOpenedActionIsCalledEvenWhenThereNoViewsAvailablesInTheExpectedTimeForTheCurrentViewModelButUnlockingTheInspectionThread()
            {
                var serviceLocator = ServiceLocator.Default;
                var fooViewModel = new FooViewModel(serviceLocator);

                var dispatcherServiceMock = new Mock<IDispatcherService>();
                dispatcherServiceMock.Setup(service => service.Invoke(It.IsAny<Action>())).Callback((Action action) => action.Invoke());
                var visualizerServiceMock = new Mock<IUIVisualizerService>();
                visualizerServiceMock.Setup(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null)).Returns(true);
                var viewManagerMock = new Mock<IViewManager>();
                viewManagerMock.Setup(manager => manager.GetViewsOfViewModel(fooViewModel)).Returns(new IView[] {});

                serviceLocator.RegisterInstance<IDispatcherService>(dispatcherServiceMock.Object);
                serviceLocator.RegisterInstance<IUIVisualizerService>(visualizerServiceMock.Object);
                serviceLocator.RegisterInstance<IViewManager>(viewManagerMock.Object);

                var @event = new AutoResetEvent(false);

                serviceLocator.ResolveType<IUIVisualizerService>().Show(fooViewModel, () => @event.Set());

                @event.WaitOne(20000);

                visualizerServiceMock.Verify(service => service.Show(It.Is<FooViewModel>(model => ReferenceEquals(model, fooViewModel)), null), Times.Once());
                viewManagerMock.Verify(manager => manager.GetViewsOfViewModel(fooViewModel), Times.AtLeastOnce());
            }

#endif
        }
        #endregion
    }
}

#endif