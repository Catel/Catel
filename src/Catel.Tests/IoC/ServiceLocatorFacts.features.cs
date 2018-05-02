// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorFacts.features.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using Catel.Reflection;
    using NUnit.Framework;

    /// <summary>
    /// These feature tests are based on http://featuretests.apphb.com/DependencyInjection.html#GeneralInformation
    /// </summary>
    public partial class ServiceLocatorFacts
    {
        [TestFixture]
        public class ListTests
        {
             [TestCase]
             public void Array()
             {
                var serviceLocator = IoCFactory.CreateServiceLocator();

                PrepareContainer(serviceLocator);

                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IService[]>>(serviceLocator);
            }

            [TestCase]
            public void List()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();

                PrepareContainer(serviceLocator);

                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IList<IService>>>(serviceLocator);
            }

            [TestCase]
            public void Collection()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();

                PrepareContainer(serviceLocator);

                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<ICollection<IService>>>(serviceLocator);
            }

            [TestCase]
            public void Enumerable()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();

                PrepareContainer(serviceLocator);

                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IEnumerable<IService>>>(serviceLocator);
            }

            [TestCase]
            public void IReadOnlyCollection()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();

                PrepareContainer(serviceLocator);

                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyCollection<IService>>>(serviceLocator);
            }

            [TestCase]
            public void IReadOnlyList()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
            
                AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyList<IService>>>(serviceLocator);
            }

            private void PrepareContainer(IServiceLocator serviceLocator)
            {
                // No code required
            }

            public void AssertResolvesListDependencyFor<TTestComponent>(IServiceLocator serviceLocator)
                where TTestComponent : IServiceWithListDependency<IEnumerable<IService>>
            {
                serviceLocator.RegisterType<IService, IndependentService>();
                serviceLocator.RegisterTypeWithTag<IService, IndependentService2>("A");

                serviceLocator.RegisterType<TTestComponent>();

                var resolved = serviceLocator.ResolveType<TTestComponent>();

                Assert.IsNotNull(resolved);
                Assert.IsNotNull(resolved.Services);
                Assert.AreEqual(2, resolved.Services.Count());

                Assert.IsTrue(resolved.Services.Any(service => service is IndependentService));
                Assert.IsTrue(resolved.Services.Any(service => service is IndependentService2));
            }
        }

        [TestFixture]
        public class TheOpenGenericFeature
        {
            [TestCase]
            public void CatelInjection()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType<Consumer, Consumer>();
                serviceLocator.RegisterType<Item, Item>();
                serviceLocator.RegisterType(typeof(IInjectable<>), typeof(Injectable<>));

                var model = serviceLocator.ResolveType<Consumer>();

                Assert.IsNotNull(model);
                Assert.IsNotNull(model.Item);
            }
        }

        #region Test classes
        public interface IService
        {
        }

        public class IndependentService : IService
        {
        }
        public class IndependentService2 : IService
        {
        }

        public interface IServiceWithListDependency<out TServiceList>
            where TServiceList : IEnumerable<IService>
        {
            TServiceList Services { get; }
        }

        public class ServiceWithListConstructorDependency<TServiceList> : IServiceWithListDependency<TServiceList>
            where TServiceList : IEnumerable<IService>
        {
            public ServiceWithListConstructorDependency(TServiceList services)
            {
                Services = services;
            }

            public TServiceList Services { get; }
        }

        public interface IInjectable<T>
        {
            T Injected { get; }
        }

        public class Injectable<T> : IInjectable<T>
        {
            public Injectable(T injected)
            {
                Injected = injected;
            }

            public T Injected { get; }
        }

        public class Item
        {
        }

        public class Consumer
        {
            public Consumer(IInjectable<Item> item)
            {
                Item = item;
            }

            public IInjectable<Item> Item { get; }
        }
        #endregion
    }
}