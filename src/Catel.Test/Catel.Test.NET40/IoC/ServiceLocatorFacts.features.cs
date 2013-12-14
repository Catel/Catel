// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorFacts.features.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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

        [TestClass]
        public class ListTests
        {
            //[TestMethod]
            //public void Array()
            //{
            //    var serviceLocator = new ServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IService[]>>(serviceLocator);
            //}

            //[TestMethod]
            //public void List()
            //{
            //    var serviceLocator = new ServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IList<IService>>>(serviceLocator);
            //}

            //[TestMethod]
            //public void Collection()
            //{
            //    var serviceLocator = new ServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<ICollection<IService>>>(serviceLocator);
            //}

            //[TestMethod]
            //public void Enumerable()
            //{
            //    var serviceLocator = new ServiceLocator();

            //    PrepareContainer(serviceLocator);

            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IEnumerable<IService>>>(serviceLocator);
            //}

            //[TestMethod]
            //public void IReadOnlyCollection()
            //{
            //    var serviceLocator = new ServiceLocator();
            //
            //    AssertResolvesListDependencyFor<ServiceWithListConstructorDependency<IReadOnlyCollection<IService>>>(serviceLocator);
            //}

            //[TestMethod]
            //public void IReadOnlyList()
            //{
            //    var serviceLocator = new ServiceLocator();
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