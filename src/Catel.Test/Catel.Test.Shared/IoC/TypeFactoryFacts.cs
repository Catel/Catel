﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;
    using Catel.Messaging;
    using Catel.Services;
    using Catel.Test.MVVM.ViewModels.TestClasses;
    using Data;
    using NUnit.Framework;

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

        public class AdvancedDependencyInjectionTestClass
        {
            public AdvancedDependencyInjectionTestClass(int intValue, IMessageService messageService, INavigationService navigationService)
            {
                Argument.IsNotNull(() => messageService);
                Argument.IsNotNull(() => navigationService);

                IntValue = intValue;
            }

            public AdvancedDependencyInjectionTestClass(string stringValue, int intValue, long longValue, IMessageService messageService, INavigationService navigationService)
            {
                Argument.IsNotNull(() => messageService);
                Argument.IsNotNull(() => navigationService);

                StringValue = stringValue;
                IntValue = intValue;
                LongValue = longValue;
            }

            public int IntValue { get; private set; }

            public string StringValue { get; private set; }

            public long LongValue { get; private set; }
        }

        [TestFixture]
        public class TheCreateInstanceMethod
        {
            [TestCase]
            public void ResolvesTypeWithStaticAndNonStaticConstructorButUsesNonStatic()
            {
                Assert.IsInstanceOf(typeof(StaticCtorClass), TypeFactory.Default.CreateInstance<StaticCtorClass>());
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsTrue(instance.UsedDefaultConstructor);
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToFirstConstructor()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                serviceLocator.RegisterInstance(iniEntry);

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();

                Assert.IsFalse(instance.UsedDefaultConstructor);
                Assert.AreEqual(iniEntry, instance.IniEntry);
                Assert.AreEqual(0, instance.IntValue);
                Assert.AreEqual(null, instance.StringValue);
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToSecondConstructor()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
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

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionUsesConstructorWithMostParametersFirst()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
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

            [TestCase]
            public void CallsCustomInitializationWhenNeeded()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();
                Assert.IsTrue(instance.HasCalledCustomInitialization);
            }

            [TestCase]
            public void AutomaticallyRegistersDependencyResolverInDependencyResolverManager()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
                var dependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();

                var instance = typeFactory.CreateInstance<DependencyInjectionTestClass>();
                var dependencyResolverManager = DependencyResolverManager.Default;
                var actualDependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(instance);

                Assert.AreEqual(dependencyResolver, actualDependencyResolver);
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

            [TestCase]
            public void ThrowsCircularDependencyExceptionForInvalidTypeRequestPath()
            {
                var serviceLocator = IoCFactory.CreateServiceLocator();
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

        [TestFixture]
        public class TheCreateInstanceWithAutoCompletionMethod
        {
            public class Person
            {
                public string FirstName { get; set; }
                public string LastName { get; set; }
            }

            public class ClassWithDynamicConstructor
            {
                public ClassWithDynamicConstructor(dynamic person)
                {
                    IsDynamicConstructorCalled = true;
                }

                public ClassWithDynamicConstructor(Person person)
                {
                    IsTypedConstructorCalled = true;
                }

                public bool IsDynamicConstructorCalled { get; private set; }

                public bool IsTypedConstructorCalled { get; private set; }
            }

            public class ClassWithSeveralMatchesForDependencyInjection
            {
                public ClassWithSeveralMatchesForDependencyInjection(IUIVisualizerService uiVisualizerService, IMessageService messageService)
                {
                }

                [InjectionConstructor]
                public ClassWithSeveralMatchesForDependencyInjection(IMessageMediator messageMediator, IMessageService messageService)
                {
                    IsRightConstructorUsed = true;
                }

                public bool IsRightConstructorUsed { get; private set; }
            }

            public class ClassWithPropertyInjection
            {
                [Inject]
                public IUIVisualizerService UiVisualizerService { get; set; }
            }

            [TestCase]
            public void CreatesTypeUsingSimpleCustomInjectionAndAutoCompletion()
            {
                var typeFactory = TypeFactory.Default;
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AdvancedDependencyInjectionTestClass>(42);

                Assert.IsNotNull(instance);
                Assert.AreEqual(42, instance.IntValue);
            }

            [TestCase]
            public void CreatesTypeUsingComplexCustomInjectionAndAutoCompletion()
            {
                var typeFactory = TypeFactory.Default;
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<AdvancedDependencyInjectionTestClass>("string", 42, 42L);

                Assert.IsNotNull(instance);
                Assert.AreEqual("string", instance.StringValue);
                Assert.AreEqual(42, instance.IntValue);
                Assert.AreEqual(42L, instance.LongValue);
            }

            [TestCase]
            public void CreatesTypeWhenDynamicConstructorIsAvailable()
            {
                var typeFactory = TypeFactory.Default;

                var person = new Person {FirstName = "John", LastName = "Doe"};
                var instance = typeFactory.CreateInstanceWithParametersAndAutoCompletion<ClassWithDynamicConstructor>(person);

                Assert.IsFalse(instance.IsDynamicConstructorCalled);
                Assert.IsTrue(instance.IsTypedConstructorCalled);
            }

            [TestCase]
            public void CreatesTypeWithInjectionConstructorAttribute()
            {
                var typeFactory = TypeFactory.Default;

                var instance = typeFactory.CreateInstance<ClassWithSeveralMatchesForDependencyInjection>();

                Assert.IsTrue(instance.IsRightConstructorUsed);
            }

            [TestCase]
            public void CreatesTypeWithPropertyInjection()
            {
                var typeFactory = TypeFactory.Default;

                var instance = typeFactory.CreateInstance<ClassWithPropertyInjection>();

                Assert.IsNotNull(instance.UiVisualizerService);
            }
        }
    }
}