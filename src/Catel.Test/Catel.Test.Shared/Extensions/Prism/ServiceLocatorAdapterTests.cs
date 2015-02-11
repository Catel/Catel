﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorAdapterTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Test.Extensions.Prism
{
    using System.Linq;

    using Catel.IoC;

    using Microsoft.Practices.Prism.Regions;
    using Microsoft.Practices.Prism.Regions.Behaviors;
    using NUnit.Framework;

    using IServiceLocator = Microsoft.Practices.ServiceLocation.IServiceLocator;

    /// <summary>
    /// The service locator adapter tests.
    /// </summary>
    [TestFixture]
    public class ServiceLocatorAdapterTests
    {
        #region Methods

        /// <summary>
        /// The get all instance just returns empty collection if the type is non registered.
        /// </summary>
        [TestCase]
        public void GetAllInstanceJustReturnsEmptyCollectionIfTheTypeIsNonRegistered()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            var serviceLocatorAdapter = new ServiceLocatorAdapter(serviceLocator);
            IFooInterface[] list = serviceLocatorAdapter.GetAllInstances<IFooInterface>().ToArray();
            Assert.AreEqual(0, list.Length);
        }

        /// <summary>
        /// The get all instance just returns collection of with onyl one resolved instance if the type is registered.
        /// </summary>
        [TestCase]
        public void GetAllInstanceJustReturnsCollectionOfWithOnylOneResolvedInstanceIfTheTypeIsRegistered()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocator.RegisterType<IFooInterface, FooNonAbstractClass>();
            var serviceLocatorAdapter = new ServiceLocatorAdapter(serviceLocator);
            IFooInterface[] list = serviceLocatorAdapter.GetAllInstances<IFooInterface>().ToArray();
            Assert.AreEqual(1, list.Length);
        }

        /// <summary>
        /// The get all instance just returns collection of with onyl one resolved instance of non registered non abstract classes.
        /// </summary>
        [TestCase]
        public void GetAllInstanceJustReturnsCollectionOfWithOnylOneResolvedInstanceOfNonRegisteredNonAbstractClasses()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            var serviceLocatorAdapter = new ServiceLocatorAdapter(serviceLocator);
            FooNonAbstractClass[] list = serviceLocatorAdapter.GetAllInstances<FooNonAbstractClass>().ToArray();
            Assert.AreEqual(1, list.Length);
        }

        /// <summary>
        /// Returns null if type is not registered.
        /// </summary>
        [TestCase]
        public void ReturnsNullIfTypeIsNotRegistered()
        {
            var adapter = new ServiceLocatorAdapter(new ServiceLocator());
            Assert.IsNull(adapter.GetInstance<IFooInterface>());
        }

        /// <summary>
        /// The get instance of non registered and non abstract class returns always a new instance.
        /// </summary>
        [TestCase]
        public void GetInstanceOfNonRegisteredAndNonAbstractClassReturnsAlwaysANewInstance()
        {
            var adapter = new ServiceLocatorAdapter(IoCFactory.CreateServiceLocator());
            var nonAbstractClassInstance1 = adapter.GetInstance<FooNonAbstractClass>();
            var nonAbstractClassInstance2 = adapter.GetInstance<FooNonAbstractClass>();
            Assert.AreNotSame(nonAbstractClassInstance1, nonAbstractClassInstance2);
        }
        
        [TestCase]
        public void GetInstanceOfNonRegisteredAndNonAbstractClassReturnsAlwaysANewInstanceWithCanResolveNonAbstractTypesWithoutRegistrationInFalse()
        {
        	var serviceLocator = IoCFactory.CreateServiceLocator();
        	serviceLocator.CanResolveNonAbstractTypesWithoutRegistration = false;
            var adapter = new ServiceLocatorAdapter(serviceLocator);
            
            var nonAbstractClassInstance1 = adapter.GetInstance<FooNonAbstractClass>();
            var nonAbstractClassInstance2 = adapter.GetInstance<FooNonAbstractClass>();
            Assert.AreNotSame(nonAbstractClassInstance1, nonAbstractClassInstance2);
        }

        /// <summary>
        /// The get instance of singleton registered non abstract class always the same instance.
        /// </summary>
        [TestCase]
        public void GetInstanceOfSingletonRegisteredNonAbstractClassAlwaysTheSameInstance()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocator.RegisterType<FooNonAbstractClass, FooNonAbstractClass>();
            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var nonAbstractClassInstance1 = adapter.GetInstance<FooNonAbstractClass>();
            var nonAbstractClassInstance2 = adapter.GetInstance<FooNonAbstractClass>();

            Assert.AreSame(nonAbstractClassInstance1, nonAbstractClassInstance2);
        }

        /// <summary>
        /// The in the default service locator region adapter mappings is registered as singleton.
        /// </summary>
        [TestCase]
        public void InTheDefaultServiceLocatorRegionAdapterMappingsIsRegisteredAsSingleton()
        {
            var adapter = new ServiceLocatorAdapter();

            var regionAdapterMappings1 = adapter.GetInstance<RegionAdapterMappings>();
            var regionAdapterMappings2 = adapter.GetInstance<RegionAdapterMappings>();

            Assert.AreSame(regionAdapterMappings1, regionAdapterMappings2);
        }

        /// <summary>
        /// The get instance of auto populate region behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfAutoPopulateRegionBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocator.RegisterType<IRegionViewRegistry, RegionViewRegistry>();

            var adapter = new ServiceLocatorAdapter(serviceLocator);
            serviceLocator.RegisterInstance<IServiceLocator>(adapter);

            var behavior1 = adapter.GetInstance<AutoPopulateRegionBehavior>();
            var behavior2 = adapter.GetInstance<AutoPopulateRegionBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance of auto delayed region creation behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfAutoDelayedRegionCreationBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocator.RegisterType<RegionAdapterMappings, RegionAdapterMappings>();

            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<DelayedRegionCreationBehavior>();
            var behavior2 = adapter.GetInstance<DelayedRegionCreationBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance o region member lifetime behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfRegionMemberLifetimeBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<RegionMemberLifetimeBehavior>();
            var behavior2 = adapter.GetInstance<RegionMemberLifetimeBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance of bind region context to dependency object behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfBindRegionContextToDependencyObjectBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();

            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<BindRegionContextToDependencyObjectBehavior>();
            var behavior2 = adapter.GetInstance<BindRegionContextToDependencyObjectBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance of region active aware behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfRegionActiveAwareBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();

            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<RegionActiveAwareBehavior>();
            var behavior2 = adapter.GetInstance<RegionActiveAwareBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance of selector items source sync behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfSelectorItemsSourceSyncBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<SelectorItemsSourceSyncBehavior>();
            var behavior2 = adapter.GetInstance<SelectorItemsSourceSyncBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        /// <summary>
        /// The get instance of sync region context with host behavior returns always a new one.
        /// </summary>
        [TestCase]
        public void GetInstanceOfSyncRegionContextWithHostBehaviorReturnsAlwaysANewOne()
        {
            var serviceLocator = IoCFactory.CreateServiceLocator();
            var adapter = new ServiceLocatorAdapter(serviceLocator);

            var behavior1 = adapter.GetInstance<SyncRegionContextWithHostBehavior>();
            var behavior2 = adapter.GetInstance<SyncRegionContextWithHostBehavior>();

            Assert.AreNotSame(behavior1, behavior2);
        }

        #endregion

        #region Nested type: FooNonAbstractClass

        /// <summary>
        /// The non abstract class.
        /// </summary>
        public class FooNonAbstractClass : IFooInterface
        {
        }
        #endregion

        #region Nested type: IFooInterface

        /// <summary>
        /// The FooInterface interface.
        /// </summary>
        public interface IFooInterface
        {
        }
        #endregion
    }
}

#endif