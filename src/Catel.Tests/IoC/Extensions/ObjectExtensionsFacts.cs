// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class ObjectExtensionsFacts
    {
        [TestFixture]
        public class TheGetTypeFactoryMethod
        {
            [TestCase]
            public void ReturnsDefaultTypeFactoryForObjectNotCreatedWithTypeFactory()
            {
                var obj = new object();
                var defaultTypeFactory = TypeFactory.Default;

#pragma warning disable IDISP001 // Dispose created.
                var usedTypeFactory = obj.GetTypeFactory();
#pragma warning restore IDISP001 // Dispose created.

                Assert.IsTrue(ReferenceEquals(defaultTypeFactory, usedTypeFactory));
            }

            [TestCase]
            public void ReturnsTypeFactoryUsedToCreateObject()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
                    var obj = typeFactory.CreateInstance<object>();

#pragma warning disable IDISP001 // Dispose created.
                    var usedTypeFactory = obj.GetTypeFactory();
#pragma warning restore IDISP001 // Dispose created.

                    Assert.IsTrue(ReferenceEquals(typeFactory, usedTypeFactory));
                }
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverMethod
        {
            [TestCase]
            public void ReturnsDefaultDependencyResolverForObjectNotCreatedWithTypeFactory()
            {
                var obj = new object();
                var defaultDependencyResolver = ServiceLocator.Default.ResolveType<IDependencyResolver>();
                var dependencyResolver = obj.GetDependencyResolver();

                Assert.IsTrue(ReferenceEquals(defaultDependencyResolver, dependencyResolver));
            }

            [TestCase]
            public void ReturnsDependencyResolverUsedToCreateObject()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
                    var typeFactory = dependencyResolver.Resolve<ITypeFactory>();
                    var obj = typeFactory.CreateInstance<object>();

                    var usedDependencyResolver = obj.GetDependencyResolver();

                    Assert.IsTrue(ReferenceEquals(dependencyResolver, usedDependencyResolver));
                }
            }
        }
    }
}
