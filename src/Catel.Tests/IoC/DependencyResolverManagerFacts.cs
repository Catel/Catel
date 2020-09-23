// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolverManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.IoC
{
    using System;
    using Catel.IoC;
    using NUnit.Framework;

    public class DependencyResolverManagerFacts
    {
        [TestFixture]
        public class TheRegisterDependencyResolverForInstanceMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(null, dependencyResolver));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(new object(), null));
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverForInstanceMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForInstance(null));
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                var myObject = new object();
                
                dependencyResolverManager.RegisterDependencyResolverForInstance(myObject, dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                var myObject = new object();

                dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestCase]
            public void ReturnsDefaultDependencyResolverForNonRegisteredInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var myObject = new object();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }
        }

        [TestFixture]
        public class TheRegisterDependencyResolverForTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(null, dependencyResolver));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), null));
            }
        }

        [TestFixture]
        public class TheGetDependencyResolverForTypeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                Assert.Throws<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForType(null));
            }

            [TestCase]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                
                dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestCase]
            public void ReturnsDefaultDependencyResolverForNonRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }

            [TestFixture]
            public class TheResolveMissingType
            {
                interface IDummy
                {

                }
                [TestCase]
                public void ThrowsTypeNotRegisteredException()
                {
                    var dependencyResolverManager = new DependencyResolverManager();

                    var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                    Assert.Throws<TypeNotRegisteredException>(() => resolvedDependencyResolver.Resolve(typeof(IDummy)));
                }
            }
        }
    }
}