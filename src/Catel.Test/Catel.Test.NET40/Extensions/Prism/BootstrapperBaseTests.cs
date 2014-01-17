// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapperBaseTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.Prism
{
    //using System;
    //using System.Windows;
    //using System.Windows.Controls;

    //using Catel.IoC;
    //using Catel.Modules;
    //using Catel.MVVM.Services;
    //using Catel.MVVM.Tasks;
    //using Catel.MVVM.ViewModels;

    //using Microsoft.Practices.Prism.Events;
    //using Microsoft.Practices.Prism.Modularity;
    //using Microsoft.Practices.Prism.Regions;
    //using Microsoft.Practices.Prism.Regions.Behaviors;
    //using Microsoft.VisualStudio.TestTools.UnitTesting;

    //using Moq;

    //using ModuleCatalog = Catel.Modules.ModuleCatalog;

    ///// <summary>
    ///// Summary description for BootstraperBaseTests
    ///// </summary>
    //[TestClass]
    //public class BootstrapperBaseTests
    //{
    //    #region Constants

    //    /// <summary>
    //    /// The foo bootstrapper.
    //    /// </summary>
    //    private static FooBootstrapper fooBootstrapper;

    //    /// <summary>
    //    /// The service locator.
    //    /// </summary>
    //    private static IServiceLocator serviceLocator;
    //    #endregion

    //    #region Methods

    //    /// <summary>
    //    /// The test method 1.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIModuleInitializerTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var moduleInitializer = serviceLocator.ResolveType<IModuleInitializer>();
    //        Assert.IsNotNull(moduleInitializer);
    //    }

    //    /// <summary>
    //    /// The initialize bootstrapper if required.
    //    /// </summary>
    //    private void InitializeBootstrapperIfRequired()
    //    {
    //        if (fooBootstrapper == null)
    //        {
    //            serviceLocator = new ServiceLocator();
    //            fooBootstrapper = new FooBootstrapper(serviceLocator);
    //            fooBootstrapper.Run();
    //        }
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i module manager type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIModuleManagerTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var moduleManager = serviceLocator.ResolveType<IModuleManager>();
    //        Assert.IsNotNull(moduleManager);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration region adapter mappings type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationRegionAdapterMappingsTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionAdapterMappings = serviceLocator.ResolveType<RegionAdapterMappings>();
    //        Assert.IsNotNull(regionAdapterMappings);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region manager type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionManagerTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionManager = serviceLocator.ResolveType<IRegionManager>();
    //        Assert.IsNotNull(regionManager);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i event aggregator type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIEventAggregatorTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var eventAggregator = serviceLocator.ResolveType<IEventAggregator>();
    //        Assert.IsNotNull(eventAggregator);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region view registry type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionViewRegistryTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionViewRegistry = serviceLocator.ResolveType<IRegionViewRegistry>();
    //        Assert.IsNotNull(regionViewRegistry);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region behavior factory type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionBehaviorFactoryTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionBehaviorFactory = serviceLocator.ResolveType<IRegionBehaviorFactory>();
    //        Assert.IsNotNull(regionBehaviorFactory);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region navigation journal entry type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionNavigationJournalEntryTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionNavigationJournalEntry = serviceLocator.ResolveType<IRegionNavigationJournalEntry>();
    //        Assert.IsNotNull(regionNavigationJournalEntry);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region navigation journal type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionNavigationJournalTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionNavigationJournal = serviceLocator.ResolveType<IRegionNavigationJournal>();
    //        Assert.IsNotNull(regionNavigationJournal);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region navigation content loader type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionNavigationContentLoaderTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionNavigationContentLoader = serviceLocator.ResolveType<IRegionNavigationContentLoader>();
    //        Assert.IsNotNull(regionNavigationContentLoader);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration i region navigation service loader type could be resolved via service locator.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationIRegionNavigationServiceLoaderTypeCouldBeResolvedViaServiceLocator()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        var regionNavigationService = serviceLocator.ResolveType<IRegionNavigationService>();
    //        Assert.IsNotNull(regionNavigationService);
    //    }

    //    /// <summary>
    //    /// The after run with default configuration auto populate region behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationAutoPopulateRegionBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<AutoPopulateRegionBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default configuration delayed region creation behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationDelayedRegionCreationBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<DelayedRegionCreationBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default configuration region active aware behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationRegionActiveAwareBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<RegionActiveAwareBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default configuration region manager registration behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationRegionManagerRegistrationBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<RegionManagerRegistrationBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default configuration region member lifetime behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationRegionMemberLifetimeBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<RegionMemberLifetimeBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default selector items source sync behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultSelectorItemsSourceSyncBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<SelectorItemsSourceSyncBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default sync region context with host behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultSyncRegionContextWithHostBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<SyncRegionContextWithHostBehavior>());
    //    }

    //    /// <summary>
    //    /// The after run with default configuration bind region context to dependency object behavior is not registered.
    //    /// </summary>
    //    [TestMethod]
    //    public void AfterRunWithDefaultConfigurationBindRegionContextToDependencyObjectBehaviorIsNotRegistered()
    //    {
    //        this.InitializeBootstrapperIfRequired();
    //        Assert.IsFalse(serviceLocator.IsTypeRegistered<BindRegionContextToDependencyObjectBehavior>());
    //    }

    //    #endregion

    //    #region Nested type: FooBootstrapper

    //    /// <summary>
    //    /// The foo bootstrapper.
    //    /// </summary>
    //    private class FooBootstrapper : BootstrapperBase
    //    {
    //        #region Constructors

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FooBootstrapper"/> class.
    //        /// </summary>
    //        /// <param name="serviceLocator">
    //        /// The service locator.
    //        /// </param>
    //        public FooBootstrapper(IServiceLocator serviceLocator = null)
    //            : base(serviceLocator)
    //        {
    //        }

    //        #endregion

    //        #region Methods

    //        /// <summary>
    //        /// The create shell.
    //        /// </summary>
    //        /// <returns>
    //        /// The System.Windows.DependencyObject.
    //        /// </returns>
    //        protected override DependencyObject CreateShell()
    //        {
    //            return new UserControl();
    //        }

    //        /// <summary>
    //        /// The create module catalog.
    //        /// </summary>
    //        /// <returns>
    //        /// The <see cref="IModuleCatalog"/>.
    //        /// </returns>
    //        protected override IModuleCatalog CreateModuleCatalog()
    //        {
    //            return new ModuleCatalog();
    //        }

    //        /// <summary>
    //        /// The configure module catalog.
    //        /// </summary>
    //        protected override void ConfigureModuleCatalog()
    //        {
    //            ((ModuleCatalog)this.ModuleCatalog).AddModule(typeof(FooModule));
    //        }

    //        #endregion
    //    }
    //    #endregion

    //    #region Nested type: FooModule

    //    /// <summary>
    //    /// The foo module.
    //    /// </summary>
    //    public class FooModule : ModuleBase
    //    {
    //        #region Constructors

    //        /// <summary>
    //        /// Initializes a new instance of the <see cref="FooModule"/> class.
    //        /// </summary>
    //        public FooModule()
    //            : base("FooModule")
    //        {
    //        }

    //        #endregion
    //    }
    //    #endregion

    //    #region Nested type: TheRunMethod

    //    /// <summary>
    //    /// The the run method.
    //    /// </summary>
    //    [TestClass]
    //    public class TheRunMethod
    //    {
    //        #region Methods

    //        /// <summary>
    //        /// The throws invalid operation exception if module catalog is null.
    //        /// </summary>
    //        [TestMethod]
    //        public void ThrowsInvalidOperationExceptionIfModuleCatalogIsNull()
    //        {
    //            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new NullModuleCatalogBootstrapper(new ServiceLocator()).Run());
    //        }

    //        /// <summary>
    //        /// Throws module type loader not found exception if module type not found.
    //        /// </summary>
    //        [TestMethod]
    //        public void ThrowsModuleTypeLoaderNotFoundExceptionIfModuleTypeNotFound()
    //        {
    //            ExceptionTester.CallMethodAndExpectException<ModuleTypeLoaderNotFoundException>(() => new UnknowModuleBootstrapper(new ServiceLocator()).Run());
    //        }

    //        /// <summary>
    //        /// Registers a module when module manager notifies its 100 percent of load progress.
    //        /// </summary>
    //        [TestMethod]
    //        public void RegistersAModuleWhenModuleManagerNotifiesIts100PercentOfLoadProgress()
    //        {
    //            var serviceLocator = new ServiceLocator();
    //            var moduleManagerMock = new Mock<IModuleManager>();
    //            moduleManagerMock.Setup(manager => manager.Run()).Raises(manager => manager.ModuleDownloadProgressChanged += null, new ModuleDownloadProgressChangedEventArgs(new ModuleInfo("FooModule", typeof(FooModule).FullName), 100, 100));
    //            serviceLocator.RegisterInstance<IModuleManager>(moduleManagerMock.Object);
    //            new FooBootstrapper(serviceLocator).Run();
    //            Assert.IsTrue(serviceLocator.IsTypeRegistered<FooModule>());
    //        }

    //        /// <summary>
    //        /// The registers a module when module manager notifies its 100 percent of load progress.
    //        /// </summary>
    //        [TestMethod]
    //        public void DoesNotRegisterAModuleIfModuleManagerDoesNotNotifyThe100PercentOfLoadProgress()
    //        {
    //            var serviceLocator = new ServiceLocator();
    //            var moduleManagerMock = new Mock<IModuleManager>();
    //            moduleManagerMock.Setup(manager => manager.Run()).Raises(manager => manager.ModuleDownloadProgressChanged += null, new ModuleDownloadProgressChangedEventArgs(new ModuleInfo("FooModule", typeof(FooModule).FullName), 100, 50));
    //            serviceLocator.RegisterInstance<IModuleManager>(moduleManagerMock.Object);
    //            new FooBootstrapper(serviceLocator).Run();
    //            Assert.IsFalse(serviceLocator.IsTypeRegistered<FooModule>());
    //        }

    //        #endregion

    //        #region Nested type: FooBootstrapper

    //        /// <summary>
    //        /// The foo bootstrapper.
    //        /// </summary>
    //        public class FooBootstrapper : BootstrapperBase
    //        {
    //            #region Constructors

    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="FooBootstrapper"/> class.
    //            /// </summary>
    //            /// <param name="serviceLocator">
    //            /// The service locator.
    //            /// </param>
    //            public FooBootstrapper(IServiceLocator serviceLocator = null)
    //                : base(serviceLocator)
    //            {
    //            }

    //            #endregion

    //            #region Methods

    //            /// <summary>
    //            /// The create shell.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="DependencyObject"/>.
    //            /// </returns>
    //            protected override DependencyObject CreateShell()
    //            {
    //                return new UserControl();
    //            }

    //            /// <summary>
    //            /// The create module catalog.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="IModuleCatalog"/>.
    //            /// </returns>
    //            protected override IModuleCatalog CreateModuleCatalog()
    //            {
    //                return new ModuleCatalog();
    //            }

    //            #endregion
    //        }
    //        #endregion

    //        #region Nested type: FooModule

    //        /// <summary>
    //        /// The foo module.
    //        /// </summary>
    //        public class FooModule : ModuleBase
    //        {
    //            #region Constructors

    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="FooModule"/> class.
    //            /// </summary>
    //            /// <param name="moduleName">
    //            /// The module name.
    //            /// </param>
    //            /// <param name="container">
    //            /// The container.
    //            /// </param>
    //            public FooModule(string moduleName, IServiceLocator container = null)
    //                : base("FooModule", null, container)
    //            {
    //            }

    //            #endregion
    //        }
    //        #endregion

    //        #region Nested type: NullModuleCatalogBootstrapper

    //        /// <summary>
    //        /// The null module catalog bootstrapper.
    //        /// </summary>
    //        public class NullModuleCatalogBootstrapper : BootstrapperBase
    //        {
    //            #region Constructors

    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="NullModuleCatalogBootstrapper"/> class.
    //            /// </summary>
    //            /// <param name="serviceLocator">
    //            /// The service locator.
    //            /// </param>
    //            public NullModuleCatalogBootstrapper(ServiceLocator serviceLocator)
    //                : base(serviceLocator)
    //            {
    //            }

    //            #endregion

    //            #region Methods

    //            /// <summary>
    //            /// The create shell.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="DependencyObject"/>.
    //            /// </returns>
    //            protected override DependencyObject CreateShell()
    //            {
    //                return new UserControl();
    //            }

    //            /// <summary>
    //            /// The create module catalog.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="IModuleCatalog"/>.
    //            /// </returns>
    //            protected override IModuleCatalog CreateModuleCatalog()
    //            {
    //                return null;
    //            }

    //            #endregion
    //        }
    //        #endregion

    //        #region Nested type: UnknowModuleBootstrapper

    //        /// <summary>
    //        /// The null module catalog bootstrapper.
    //        /// </summary>
    //        public class UnknowModuleBootstrapper : BootstrapperBase
    //        {
    //            #region Constructors

    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="UnknowModuleBootstrapper"/> class. 
    //            /// </summary>
    //            /// <param name="serviceLocator">
    //            /// The service locator.
    //            /// </param>
    //            public UnknowModuleBootstrapper(ServiceLocator serviceLocator)
    //                : base(serviceLocator)
    //            {
    //            }

    //            #endregion

    //            #region Methods

    //            /// <summary>
    //            /// The create shell.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="DependencyObject"/>.
    //            /// </returns>
    //            protected override DependencyObject CreateShell()
    //            {
    //                return new UserControl();
    //            }

    //            /// <summary>
    //            /// The create module catalog.
    //            /// </summary>
    //            /// <returns>
    //            /// The <see cref="IModuleCatalog"/>.
    //            /// </returns>
    //            protected override IModuleCatalog CreateModuleCatalog()
    //            {
    //                return new ModuleCatalog();
    //            }

    //            /// <summary>
    //            /// The configure module catalog.
    //            /// </summary>
    //            protected override void ConfigureModuleCatalog()
    //            {
    //                ((ModuleCatalog)this.ModuleCatalog).AddModule(new ModuleInfo("UnknowModuleName", "UnknowModuleType"));
    //            }

    //            #endregion
    //        }
    //        #endregion
    //    }
    //    #endregion

    //    #region Nested type: TheRunWithSplashScreen

    //    /// <summary>
    //    /// The the run with splash screen.
    //    /// </summary>
    //    [TestClass]
    //    public class TheRunWithSplashScreen
    //    {
    //        #region Methods

    //        /// <summary>
    //        /// The registers the boot tasks and calls commit async.
    //        /// </summary>
    //        [TestMethod]
    //        public void RegistersTheBootTasksAndCallsCommitAsync()
    //        {
    //            var splashScreenServiceMock = new Mock<ISplashScreenService>();
    //            var serviceLocator = new ServiceLocator();
    //            serviceLocator.RegisterInstance<ISplashScreenService>(splashScreenServiceMock.Object);
    //            new FooBootstrapper(serviceLocator).RunWithSplashScreen<ProgressNotifyableViewModel>();
    //            splashScreenServiceMock.Verify(service => service.Enqueue(It.IsAny<ITask>()), Times.AtLeast(12));
    //            splashScreenServiceMock.Verify(service => service.CommitAsync<ProgressNotifyableViewModel>(null, null, true), Times.Once());
    //        }

    //        #endregion
    //    }
    //    #endregion
    //}
}