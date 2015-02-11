﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelDependencyResolverFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;
    using Catel.Services;
    using NUnit.Framework;

    public class CatelDependencyResolverFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNulServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new CatelDependencyResolver(null));
            }
        }

        [TestFixture]
        public class TheCanResolveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolver.CanResolve(null));
            }

            [TestCase]
            public void ReturnsFalseForNonRegisteredType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                Assert.IsFalse(dependencyResolver.CanResolve(typeof(ITestInterface)));
            }

            [TestCase]
            public void ReturnsTrueForRegisteredType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                Assert.IsTrue(dependencyResolver.CanResolve(typeof(IMessageService)));
            }
        }

        [TestFixture]
        public class TheCanResolveAllMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullTypes()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => dependencyResolver.CanResolveAll(null));
            }

            [TestCase]
            public void ReturnsTrueForEmptyArray()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                Assert.IsTrue(dependencyResolver.CanResolveAll(new Type[] { }));
            }

            [TestCase]
            public void ReturnsFalseWhenNotAllTypesCanBeResolved()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                var typesToResolve = new[] { typeof(ITestInterface), typeof(INavigationService), typeof(ITypeFactory) };

                Assert.IsFalse(dependencyResolver.CanResolveAll(typesToResolve));
            }

            [TestCase]
            public void ReturnsTrueWhenAllTypesCanBeResolved()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                serviceLocator.RegisterType<INavigationService, NavigationService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                var typesToResolve = new[] { typeof(IMessageService), typeof(INavigationService), typeof(ITypeFactory) };
                Assert.IsTrue(dependencyResolver.CanResolveAll(typesToResolve));
            }
        }

        [TestFixture]
        public class TheResolveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => dependencyResolver.Resolve(null));
            }

            [TestCase]
            public void ThrowsTypeNotRegisteredForNonRegisteredType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                ExceptionTester.CallMethodAndExpectException<TypeNotRegisteredException>(() => dependencyResolver.Resolve(typeof(ITestInterface)));
            }

            [TestCase]
            public void ReturnsInstanceForRegisteredType()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                Assert.IsNotNull(dependencyResolver.Resolve(typeof(IMessageService)));
            }
        }

        [TestFixture]
        public class TheResolveAllMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullTypes()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => dependencyResolver.ResolveAll(null));
            }

            [TestCase]
            public void ReturnsEmptyArrayForEmptyArray()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                var resolvedObjects = dependencyResolver.ResolveAll(new Type[] {});
                Assert.AreEqual(0, resolvedObjects.Length);
            }

            [TestCase]
            public void ReturnsArrayWithNullValuesForNonRegisteredTypes()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                var typesToResolve = new[] { typeof(IMessageService), typeof(ITestInterface), typeof(ITypeFactory) };
                var resolvedTypes = dependencyResolver.ResolveAll(typesToResolve);

                Assert.IsNotNull(resolvedTypes[0] as IMessageService);
                Assert.IsNull(resolvedTypes[1]);
                Assert.IsNotNull(resolvedTypes[2] as ITypeFactory);
            }

            [TestCase]
            public void ReturnsArrayWithAllValuesForRegisteredTypes()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                serviceLocator.RegisterType<IMessageService, MessageService>();
                serviceLocator.RegisterType<INavigationService, NavigationService>();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();

                var typesToResolve = new[] { typeof(IMessageService), typeof(INavigationService), typeof(ITypeFactory) };
                var resolvedTypes = dependencyResolver.ResolveAll(typesToResolve);

                Assert.IsNotNull(resolvedTypes[0] as IMessageService);
                Assert.IsNotNull(resolvedTypes[1] as INavigationService);
                Assert.IsNotNull(resolvedTypes[2] as ITypeFactory);
            }
        }
    }
}