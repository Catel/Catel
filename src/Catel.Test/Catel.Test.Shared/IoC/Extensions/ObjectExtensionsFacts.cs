// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
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
                var usedTypeFactory = obj.GetTypeFactory();

                Assert.IsTrue(ReferenceEquals(defaultTypeFactory, usedTypeFactory));
            }

            [TestCase]
            public void ReturnsTypeFactoryUsedToCreateObject()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
                var obj = typeFactory.CreateInstance<object>();

                var usedTypeFactory = obj.GetTypeFactory();

                Assert.IsTrue(ReferenceEquals(typeFactory, usedTypeFactory));
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
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
                var typeFactory = dependencyResolver.Resolve<ITypeFactory>();
                var obj = typeFactory.CreateInstance<object>();

                var usedDependencyResolver = obj.GetDependencyResolver();

                Assert.IsTrue(ReferenceEquals(dependencyResolver, usedDependencyResolver));
            }
        }
    }
}