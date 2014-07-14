// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorFacts.features.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        #region Test classes
        public interface IService
        {
        }

        public class IndependentService : IService
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
            private readonly TServiceList _services;

            public ServiceWithListConstructorDependency(TServiceList services)
            {
                _services = services;
            }

            public TServiceList Services
            {
                get { return _services; }
            }
        }
        #endregion

        [TestFixture]
        public class ListTests
        {
            //[TestCase]
            //public void Array()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IService[]>>(serviceLocator);
            //}

            //[TestCase]
            //public void List()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IList<IService>>>(serviceLocator);
            //}

            //[TestCase]
            //public void Collection()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<ICollection<IService>>>(serviceLocator);
            //}

            //[TestCase]
            //public void Enumerable()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IEnumerable<IService>>>(serviceLocator);
            //}

            //[TestCase]
            //public void IReadOnlyCollection()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();
            //
            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyCollection<IService>>>(serviceLocator);
            //}

            //[TestCase]
            //public void IReadOnlyList()
            //{
            //    var serviceLocator = IoCFactory.CreateServiceLocator();
            //
            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyList<IService>>>(serviceLocator);
            //}

            private void PrepareContainer(IServiceLocator serviceLocator)
            {
                // No code required
            }

            public void AssertResolvesListDependencyFor<TTestComponent>(IServiceLocator serviceLocator)
                where TTestComponent : IServiceWithListDependency<IEnumerable<IService>>
            {
                serviceLocator.RegisterType<IService, IndependentService>();
                serviceLocator.RegisterType<TTestComponent>();

                var resolved = serviceLocator.ResolveType<TTestComponent>();

                Assert.IsNotNull(resolved);
                Assert.IsNotNull(resolved.Services);
                Assert.AreEqual(1, resolved.Services.Count());
                Assert.IsTrue(typeof(IndependentService).IsAssignableFromEx(resolved.Services.First().GetType()));
            }
        }
    }
}