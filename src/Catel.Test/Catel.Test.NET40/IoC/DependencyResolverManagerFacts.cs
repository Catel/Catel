// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolverManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class DependencyResolverManagerFacts
    {
        [TestClass]
        public class TheRegisterDependencyResolverForInstanceMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(null, dependencyResolver));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForInstance(new object(), null));
            }
        }

        [TestClass]
        public class TheGetDependencyResolverForInstanceMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForInstance(null));
            }

            [TestMethod]
            public void ReturnsRegisteredDependencyResolverForRegisteredInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                var myObject = new object();
                
                dependencyResolverManager.RegisterDependencyResolverForInstance(myObject, dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestMethod]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                var myObject = new object();

                dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestMethod]
            public void ReturnsDefaultDependencyResolverForNonRegisteredInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var myObject = new object();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(myObject);

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }
        }

        [TestClass]
        public class TheRegisterDependencyResolverForTypeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(null, dependencyResolver));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullDependencyResolver()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), null));
            }
        }

        [TestClass]
        public class TheGetDependencyResolverForTypeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolverManager.GetDependencyResolverForType(null));
            }

            [TestMethod]
            public void ReturnsRegisteredDependencyResolverForRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();
                var dependencyResolver = new CatelDependencyResolver(new ServiceLocator());
                
                dependencyResolverManager.RegisterDependencyResolverForType(typeof(object), dependencyResolver);

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                Assert.IsTrue(ReferenceEquals(dependencyResolver, resolvedDependencyResolver));
            }

            [TestMethod]
            public void ReturnsDefaultDependencyResolverForNonRegisteredType()
            {
                var dependencyResolverManager = new DependencyResolverManager();

                var resolvedDependencyResolver = dependencyResolverManager.GetDependencyResolverForType(typeof(object));

                Assert.IsTrue(ReferenceEquals(dependencyResolverManager.DefaultDependencyResolver, resolvedDependencyResolver));
            }
        }
    }
}