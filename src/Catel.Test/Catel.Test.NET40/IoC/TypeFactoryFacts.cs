// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;
    using Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class TypeFactoryFacts
    {
        public class StaticCtorClass
        {
            static StaticCtorClass()
            {

            }

            public StaticCtorClass()
            {

            }
        }

        public class DependencyInjectionTestClass : INeedCustomInitialization
        {
            public DependencyInjectionTestClass()
            {
                UsedDefaultConstructor = true;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry)
            {
                IniEntry = iniEntry;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry, int intValue)
                : this(iniEntry)
            {
                IntValue = intValue;
            }

            public DependencyInjectionTestClass(IniEntry iniEntry, int intValue, string stringValue)
                : this(iniEntry, intValue)
            {
                StringValue = stringValue;
            }

            public bool UsedDefaultConstructor { get; private set; }

            public IniEntry IniEntry { get; private set; }

            public int IntValue { get; private set; }

            public string StringValue { get; private set; }

            public bool HasCalledCustomInitialization { get; private set; }

            void INeedCustomInitialization.Initialize()
            {
                HasCalledCustomInitialization = true;
            }
        }

        [TestClass]
        public class TheCreateInstanceMethod
        {
            [TestMethod]
            public void ResolvesTypeWithStaticAndNonStaticConstructorButUsesNonStatic()
            {
                Assert.IsInstanceOfType(TypeFactory.Default.CreateInstance<StaticCtorClass>(), typeof(StaticCtorClass));
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsTrue(instance.UsedDefaultConstructor);
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToFirstConstructor()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                serviceLocator.RegisterInstance(iniEntry);

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsFalse(instance.UsedDefaultConstructor);
                Assert.AreEqual(iniEntry, instance.IniEntry);
                Assert.AreEqual(0, instance.IntValue);
                Assert.AreEqual(null, instance.StringValue);
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToSecondConstructor()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                serviceLocator.RegisterInstance(iniEntry);
                serviceLocator.RegisterInstance(42);

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsFalse(instance.UsedDefaultConstructor);
                Assert.AreEqual(iniEntry, instance.IniEntry);
                Assert.AreEqual(42, instance.IntValue);
                Assert.AreEqual(null, instance.StringValue);
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionUsesConstructorWithMostParametersFirst()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                serviceLocator.RegisterInstance(iniEntry);
                serviceLocator.RegisterInstance(42);
                serviceLocator.RegisterInstance("hi there");

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsFalse(instance.UsedDefaultConstructor);
                Assert.AreEqual(iniEntry, instance.IniEntry);
                Assert.AreEqual(42, instance.IntValue);
                Assert.AreEqual("hi there", instance.StringValue);
            }

            [TestMethod]
            public void CallsCustomInitializationWhenNeeded()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();
                Assert.IsTrue(instance.HasCalledCustomInitialization);
            }

            public class X
            {
                public X(Y y) { }
            }

            public class Y
            {
                public Y(Z z) { }
            }

            public class Z
            {
                public Z(X x) { }
            }

            [TestMethod]
            public void ThrowsCircularDependencyExceptionForInvalidTypeRequestPath()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                serviceLocator.RegisterType<X>();
                serviceLocator.RegisterType<Y>();
                serviceLocator.RegisterType<Z>();

                var ex = ExceptionTester.CallMethodAndExpectException<CircularDependencyException>(() => typeFactory.CreateInstance<X>());

                Assert.AreEqual(4, ex.TypePath.AllTypes.Length);
                Assert.AreEqual(typeof(X), ex.TypePath.FirstType.Type);
                Assert.AreEqual(typeof(X), ex.TypePath.LastType.Type);
            }
        }
    }
}