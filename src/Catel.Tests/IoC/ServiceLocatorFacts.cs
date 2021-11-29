// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.IoC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Catel.Caching;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Catel.Services;
    using Catel.Tests.Data;
    using Moq;
    using NUnit.Framework;

    public partial class ServiceLocatorFacts
    {
        [TestFixture]
        public class ParentServiceLocatorFacts
        {
            private class CustomAppDataService : AppDataService
            {

            }

            public interface IService1
            {

            }

            public class Service1 : IService1
            {

            }

            public interface IService2
            {

            }

            public class Service2 : IService2
            {
                public Service2(IService1 dependency)
                {

                }
            }

            public interface IService3
            {

            }

            public class Service3 : IService3
            {
                public Service3(IService2 dependency)
                {

                }
            }

            private static ServiceLocator CreateLocator()
            {
                using (var parentServiceLocator = new ServiceLocator())
                {

                    // Default registered service locator
                    var coreModule = new CoreModule();
                    coreModule.Initialize(parentServiceLocator);

                    // Override in child
                    var childServiceLocator = new ServiceLocator(parentServiceLocator);
                    childServiceLocator.RegisterType<IAppDataService, CustomAppDataService>();

                    // Set up nested hierarchy for type construction, so for construction the types should go into
                    // child => parent => child
                    childServiceLocator.RegisterType<IService1, Service1>();
                    parentServiceLocator.RegisterType<IService2, Service2>();
                    childServiceLocator.RegisterType<IService3, Service3>();

                    return childServiceLocator;
                }
            }

            [TestCase]
            public void UsesCorrectTypeFactoryToConstructTypes()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var service3 = serviceLocator.ResolveType<IService3>();

                    Assert.IsNotNull(service3);
                }
            }

            [TestCase]
            public void ResolvesTypeFromItself()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var selfService = serviceLocator.ResolveType<IAppDataService>();

                    Assert.IsNotNull(selfService);
                    Assert.IsInstanceOf<CustomAppDataService>(selfService);
                }
            }

            [TestCase]
            public void ResolvesTypeFromParent()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var parentService = serviceLocator.ResolveType<ISerializationManager>();

                    Assert.IsNotNull(parentService);
                }
            }
        }

        [TestFixture]
        public class IDisposableImplementation
        {
            private sealed class Disposable : IDisposable
            {
                public event EventHandler<System.EventArgs> Disposed;

                public void Dispose()
                {
                    Disposed?.Invoke(this, System.EventArgs.Empty);
                }
            }

            [Test]
            public void DisposesAllDisposableInstances()
            {
                var isDisposed = false;

                using (var disposable = new Disposable())
                {
                    disposable.Disposed += (sender, e) => isDisposed = true;

                    using (var serviceLocator = new ServiceLocator())
                    {
                        serviceLocator.RegisterInstance(typeof(Disposable), disposable);
                    }

                    Assert.IsTrue(isDisposed);
                }
            }
        }

        [TestFixture]
        public class TheDeadLockPrevention
        {
            // Note that this class contains very bad code practices, but this way we try to mimic a deadlock

            #region Constants
#pragma warning disable IDE1006 // Naming Styles
            private static IServiceLocator _serviceLocator;
#pragma warning restore IDE1006 // Naming Styles
            #endregion

            #region Methods
            [TestCase]
            public void DeadlockIsNotCausedByMultipleInheritedResolving()
            {
                _serviceLocator?.Dispose();
                _serviceLocator = IoCFactory.CreateServiceLocator();

                var serviceLocator = _serviceLocator;
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
                serviceLocator.RegisterType<IInterfaceB, ClassB>();
                serviceLocator.RegisterType<IInterfaceC, ClassC>();

                var classA = typeFactory.CreateInstance<ClassA>();
                serviceLocator.RegisterInstance<IInterfaceA>(classA);
            }
            #endregion

            #region Nested type: ClassA
            public class ClassA : IInterfaceA
            {
                #region Fields
                private readonly ICacheStorage<string, string> _cache = new CacheStorage<string, string>();
                #endregion

                #region Constructors
                public ClassA()
                {
                    SomeMethodResolvingTypes();
                }
                #endregion

                #region Methods
                private string SomeMethodResolvingTypes()
                {
                    return _cache.GetFromCacheOrFetch("key", () =>
                    {
                        var classB = new ClassB();
                        return "done!";
                    });
                }
                #endregion
            }
            #endregion

            #region Nested type: ClassB
            public class ClassB : IInterfaceB
            {
                #region Constructors
                public ClassB()
                {
                    var classB = _serviceLocator.ResolveType<IInterfaceC>();
                }
                #endregion
            }
            #endregion

            #region Nested type: ClassC
            public class ClassC : IInterfaceC
            {
                #region Constructors
                public ClassC()
                {
                }
                #endregion
            }
            #endregion

            #region Nested type: IInterfaceA
            public interface IInterfaceA
            {
            }
            #endregion

            #region Nested type: IInterfaceB
            public interface IInterfaceB
            {
            }
            #endregion

            #region Nested type: IInterfaceC
            public interface IInterfaceC
            {
            }
            #endregion
        }

        [TestFixture]
        public class TheTagSupportFunctionality
        {
            #region Methods
            [TestCase]
            public void AllowsTheSameInterfaceDefinedTwiceWithDifferentTags()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                    Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "1"));
                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "2"));
                }
            }

            [TestCase]
            public void OverridesRegistrationWithSameTag()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass2), "1");

                    var firstService = serviceLocator.ResolveType(typeof(ITestInterface), "1");
                    Assert.AreEqual(typeof(TestClass2), firstService.GetType());
                }
            }

            [TestCase]
            public void ResolvesInnerDependenciesWithRightTag()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface1), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface2), typeof(TestClass2), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface3), typeof(TestClass3), "1");

                    var testInterface1 = serviceLocator.ResolveType<ITestInterface1>("1");

                    var firstService = (ITestInterface3)serviceLocator.ResolveType(typeof(ITestInterface3), "1");
                    Assert.AreEqual(typeof(TestClass3), firstService.GetType());
                    Assert.IsTrue(ReferenceEquals(testInterface1, firstService.TestInterface1));
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheGenericTypesSupport
        {
            #region Methods
            [TestCase]
            public void CorrectlyResolvesClosedGenericTypeWithSingleInnerType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IList<int>), typeof(List<int>));

                    var resolvedObject = serviceLocator.ResolveType<IList<int>>();
                    Assert.IsTrue(resolvedObject is List<int>);
                }
            }

            [TestCase]
            public void CorrectlyResolvesOpenGenericTypeWithSingleInnerType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IList<>), typeof(List<>));

                    var resolvedObject = serviceLocator.ResolveType<IList<int>>();
                    Assert.IsTrue(resolvedObject is List<int>);
                }
            }

            [TestCase]
            public void CorrectlyResolvesClosedGenericTypeWithMultipleInnerTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDictionary<string, int>), typeof(Dictionary<string, int>));

                    var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                    Assert.IsTrue(resolvedObject is Dictionary<string, int>);
                }
            }

            [TestCase]
            public void CorrectlyResolvesOpenGenericTypeWithMultipleInnerTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));

                    var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                    Assert.IsTrue(resolvedObject is Dictionary<string, int>);
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheIsTypeRegisteredAsSingletonMethod
        {
            #region Methods
            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();
                    Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton<ITestInterface>());
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_NonSingleton()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_UnRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_RegisteredTypeViaMissingTypeEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.MissingType += (sender, args) =>
                    {
                        args.ImplementingType = typeof(TestClass1);
                        args.RegistrationType = RegistrationType.Transient;
                    };
                    Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_RegisteredInstanceViaMissingTypeEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.MissingType += (sender, args) =>
                    {
                        args.ImplementingInstance = new TestClass1();
                        // NOTE: This value will be ignored, read the docs.
                        args.RegistrationType = RegistrationType.Transient;
                    };

                    Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheIsTypeRegisteredMethod
        {
            [TestCase]
            public void IsTypeRegistered_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                }
            }

            [TestCase]
            public void IsTypeRegistered_Null()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.IsTypeRegistered(null));
                }
            }

            [TestCase]
            public void IsTypeRegistered_UnregisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void IsTypeRegistered_UnregisteredTypeRegisteredInstanceViaMissingType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.MissingType += (sender, args) => { args.ImplementingInstance = new TestClass1(); };

                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void IsTypeRegistered_RegisteredAsInstanceInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void IsTypeRegistered_RegisteredAsTypeInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void IsTypeRegistered_UnregisteredTypeRegisteredViaMissingTypeEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));

                    serviceLocator.MissingType += (sender, e) => e.ImplementingType = typeof(TestClass1);
                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                }
            }
        }

        [TestFixture]
        public class TheRegisterInstanceMethod
        {
            #region Methods
            [TestCase]
            public void RegisterInstance_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass1 { Name = "My Instance" });

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreEqual("My Instance", instance.Name);
                }
            }

            [TestCase]
            public void RegisterInstance_InterfaceTypeNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RegisterInstance(null, new TestClass1 { Name = "My Instance" }, null));
                }
            }

            [TestCase]
            public void RegisterInstance_InstanceNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RegisterInstance(typeof(ITestInterface), null, null));
                }
            }

            [TestCase]
            public void RegisterInstance_Valid()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass1 { Name = "My Instance" });

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreEqual("My Instance", instance.Name);
                }
            }

            [TestCase]
            public void ThrowsArgumentExceptionForWrongServiceType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var serviceType = typeof(IViewModelLocator);
                    var serviceInstance = new ViewLocator();

                    Assert.Throws<ArgumentException>(() => serviceLocator.RegisterInstance(serviceType, serviceInstance));
                }
            }

            [TestCase]
            public void InvokesTypeRegisteredEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    TypeRegisteredEventArgs eventArgs = null;

                    serviceLocator.TypeRegistered += (sender, args) => { eventArgs = args; };
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass2());

                    Assert.IsNotNull(eventArgs);
                    Assert.AreEqual(typeof(ITestInterface), eventArgs.ServiceType);
                    Assert.AreEqual(typeof(TestClass2), eventArgs.ServiceImplementationType);
                    Assert.AreEqual(RegistrationType.Singleton, eventArgs.RegistrationType);
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheRegisterTypeMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsInvalidOperationExceptionForInterfaceAsImplementation()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<InvalidOperationException>(() => serviceLocator.RegisterType<ITestInterface, ITestInterface>());
                }
            }

            [TestCase]
            public void RegisterType_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                }
            }

            [TestCase]
            public void RegisterType_InterfaceTypeNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RegisterType((Type)null, typeof(TestClass1), null, RegistrationType.Singleton, true));
                }
            }

            [TestCase]
            public void RegisterType_ImplementingTypeNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RegisterType(typeof(ITestInterface), (Type)null, null, RegistrationType.Singleton, true));
                }
            }

            [TestCase]
            public void RegisterType_Valid()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    serviceLocator.RegisterType<ITestInterface, TestClass2>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                    Assert.IsInstanceOf(typeof(TestClass2), serviceLocator.ResolveType<ITestInterface>());
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration_ToChangeInstantiationStyle()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();
                    var testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    var testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreSame(testInterfaceRef1, testInterfaceRef2);

                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreNotSame(testInterfaceRef1, testInterfaceRef2);
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration_ToChangeInstantiationStyle_And_Type()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();
                    var testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    var testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreSame(testInterfaceRef1, testInterfaceRef2);
                    Assert.AreEqual(testInterfaceRef2.GetType(), typeof(TestClass1));

                    serviceLocator.RegisterType<ITestInterface, TestClass2>(registrationType: RegistrationType.Transient);
                    testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreNotSame(testInterfaceRef1, testInterfaceRef2);

                    Assert.AreEqual(testInterfaceRef2.GetType(), typeof(TestClass2));
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration_WitoutChangeInstantiationStyle_And_JustChangingTheType()
            {
                ServiceLocator.Default.RegisterType<ITestInterface, TestClass1>();
                var testInterfaceRef1 = ServiceLocator.Default.ResolveType<ITestInterface>();
                var testInterfaceRef2 = ServiceLocator.Default.ResolveType<ITestInterface>();
                Assert.AreSame(testInterfaceRef1, testInterfaceRef2);
                Assert.AreEqual(testInterfaceRef2.GetType(), typeof(TestClass1));

                ServiceLocator.Default.RegisterType<ITestInterface, TestClass2>();
                testInterfaceRef1 = ServiceLocator.Default.ResolveType<ITestInterface>();
                testInterfaceRef2 = ServiceLocator.Default.ResolveType<ITestInterface>();
                Assert.AreSame(testInterfaceRef1, testInterfaceRef2);

                Assert.AreEqual(testInterfaceRef2.GetType(), typeof(TestClass2));
            }

            [TestCase]
            public void InvokesTypeRegisteredEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    TypeRegisteredEventArgs eventArgs = null;

                    serviceLocator.TypeRegistered += (sender, args) => { eventArgs = args; };
                    serviceLocator.RegisterType<ITestInterface, TestClass2>(registrationType: RegistrationType.Transient);

                    Assert.IsNotNull(eventArgs);
                    Assert.AreEqual(typeof(ITestInterface), eventArgs.ServiceType);
                    Assert.AreEqual(typeof(TestClass2), eventArgs.ServiceImplementationType);
                    Assert.AreEqual(RegistrationType.Transient, eventArgs.RegistrationType);
                }
            }

            [TestCase]
            public void RegistersLateBoundImplementationUsingCallback()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface>((tf, reg) => new TestClass2());

                    var resolvedClass = serviceLocator.ResolveType<ITestInterface>();

                    Assert.AreEqual(typeof(TestClass2), resolvedClass.GetType());
                }
            }

            [TestCase]
            public void RegistersLateBoundImplementationUsingLateBoundImplementationType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface>((tf, reg) => new TestClass2());

                    var registeredTypeInfo = serviceLocator.GetRegistrationInfo(typeof(ITestInterface));

                    Assert.IsTrue(registeredTypeInfo.IsLateBoundRegistration);
                    Assert.AreEqual(typeof(LateBoundImplementation), registeredTypeInfo.ImplementingType);
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheRemoveTypeMethod
        {
            #region Methods
            [TestCase]
            public void RemoveType_ThrowsArgumentNullExceptionIfServiceTypeIsNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RemoveType(null, "TestClass1"));
                }
            }

            [TestCase]
            public void RemoveType_NoTag()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    serviceLocator.RemoveType(typeof(ITestInterface));

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<ITestInterface>());
                }
            }

            [TestCase]
            public void RemovesOnlyTheTaggedTypeAndInstances()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                    var ref1 = serviceLocator.ResolveType(typeof(ITestInterface), "2");

                    serviceLocator.RemoveType(typeof(ITestInterface), "1");

                    var ref2 = serviceLocator.ResolveType(typeof(ITestInterface), "2");

                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType(typeof(ITestInterface), "1"));
                    Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "2"));
                    Assert.IsTrue(object.ReferenceEquals(ref1, ref2));
                }
            }

            [TestCase]
            public void RaisesTypeUnregisteredEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1));
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                    var ref1 = serviceLocator.ResolveType(typeof(ITestInterface), "2");

                    var raisedEvent = false;

                    serviceLocator.TypeUnregistered += (sender, e) =>
                    {
                        Assert.AreEqual(typeof(ITestInterface), e.ServiceType);
                        Assert.AreEqual(typeof(TestClass1), e.ServiceImplementationType);
                        Assert.AreEqual(RegistrationType.Singleton, e.RegistrationType);
                        Assert.IsNull(e.Instance);

                        raisedEvent = true;
                    };

                    serviceLocator.RemoveType(typeof(ITestInterface), "1");

                    Assert.IsTrue(raisedEvent);
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheRemoveAllTypesMethod
        {
            #region Methods
            [TestCase]
            public void RemoveAllTypes_ThrowsArgumentNullExceptionIfServiceTypeIsNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.RemoveAllTypes(null));
                }
            }

            [TestCase]
            public void RemoveAllTypes_Simple()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    serviceLocator.RemoveAllTypes(typeof(ITestInterface));

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<ITestInterface>());
                }
            }

            [TestCase]
            public void RemoveAllTypes_Tags()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                    serviceLocator.RemoveAllTypes(typeof(ITestInterface));

                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType(typeof(ITestInterface), "1"));
                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType(typeof(ITestInterface), "2"));
                }
            }

            [TestCase]
            public void RaisesTypeUnregisteredEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1));
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                    serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                    var ref1 = serviceLocator.ResolveType(typeof(ITestInterface), "2");

                    var nonInstanceCounter = 0;
                    var instanceCounter = 0;

                    serviceLocator.TypeUnregistered += (sender, e) =>
                    {
                        if (e.Instance is not null)
                        {
                            instanceCounter++;
                        }
                        else
                        {
                            nonInstanceCounter++;
                        }
                    };

                    serviceLocator.RemoveAllTypes(typeof(ITestInterface));

                    Assert.AreEqual(2, nonInstanceCounter);
                    Assert.AreEqual(1, instanceCounter);
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheResolveTypeMethod
        {
            public class DependencyInjectionTestClass
            {
                #region Constructors
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
                #endregion

                #region Properties
                public bool UsedDefaultConstructor { get; private set; }

                public IniEntry IniEntry { get; private set; }

                public int IntValue { get; private set; }

                public string StringValue { get; private set; }
                #endregion
            }

            [TestCase]
            public void ResolveType_Of_Non_Registered_Non_Abstract_Class_Without_Registration()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    var dependencyInjectionTestClass = serviceLocator.ResolveType<DependencyInjectionTestClass>();
                    Assert.IsNotNull(dependencyInjectionTestClass);
                    Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(DependencyInjectionTestClass)));
                }
            }

            [TestCase]
            public void ResolveType_Of_Non_Registered_Non_Abstract_Class_Without_Registration_CanResolveNonAbstractTypesWithoutRegistration_In_False()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.CanResolveNonAbstractTypesWithoutRegistration = false;
                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType(typeof(DependencyInjectionTestClass)));
                }
            }

            [TestCase]
            public void ResolveType_Generic_TransientLifestyle()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.IsInstanceOf(typeof(TestClass1), firstInstance);

                    var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.IsInstanceOf(typeof(TestClass1), secondInstance);

                    Assert.AreNotSame(firstInstance, secondInstance);
                }
            }

            [TestCase]
            public void ResolveType_Generic_SingletonLifestyle()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                    var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.IsInstanceOf(typeof(TestClass1), firstInstance);

                    var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.IsInstanceOf(typeof(TestClass1), secondInstance);

                    Assert.AreSame(firstInstance, secondInstance);
                }
            }

            [TestCase]
            public void ResolveType_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                    Assert.IsInstanceOf(typeof(TestClass1), serviceLocator.ResolveType<ITestInterface>());
                }
            }

            [TestCase]
            public void ResolveType_InterfaceTypeNull()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveType(null));
                }
            }

            [TestCase]
            public void ResolveType_UnregisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<ITestInterface>());
                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType(typeof(ITestInterface)));
                }
            }

            [TestCase]
            public void ResolveType_RegisteredAsInstanceInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass2 { Name = "instance test" });

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreEqual("instance test", instance.Name);

                    instance.Name = "changed name";
                    var newInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.AreEqual("changed name", newInstance.Name);
                    Assert.AreEqual(instance, newInstance);
                    Assert.IsTrue(object.ReferenceEquals(instance, newInstance));
                }
            }

            [TestCase]
            public void ResolveType_RegisteredAsTypeInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.IsInstanceOf(typeof(TestClass1), instance);
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.IsTrue(instance.UsedDefaultConstructor);
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToFirstConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();
                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.IsFalse(instance.UsedDefaultConstructor);
                    Assert.AreEqual(iniEntry, instance.IniEntry);
                    Assert.AreEqual(0, instance.IntValue);
                    Assert.AreEqual(null, instance.StringValue);
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToSecondConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();
                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);
                    serviceLocator.RegisterInstance(42);

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.IsFalse(instance.UsedDefaultConstructor);
                    Assert.AreEqual(iniEntry, instance.IniEntry);
                    Assert.AreEqual(42, instance.IntValue);
                    Assert.AreEqual(null, instance.StringValue);
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionUsesConstructorWithMostParametersFirst()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();
                    var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                    serviceLocator.RegisterInstance(iniEntry);
                    serviceLocator.RegisterInstance(42);
                    serviceLocator.RegisterInstance("hi there");

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.IsFalse(instance.UsedDefaultConstructor);
                    Assert.AreEqual(iniEntry, instance.IniEntry);
                    Assert.AreEqual(42, instance.IntValue);
                    Assert.AreEqual("hi there", instance.StringValue);
                }
            }

            [TestCase]
            public void InvokesTypeInstantiatedEventForInstantiatedTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();

                    var invoked = false;

                    serviceLocator.TypeInstantiated += (sender, e) => invoked = true;

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.IsTrue(invoked);
                }
            }

            [TestCase]
            public void DoesNotCorruptTypeRequestPathOnMultipleCallsWithExceptions()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.TryResolveType<ITestInterface>();

                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.IsNotNull(serviceLocator.ResolveType<ITestInterface>());
                }
            }
        }

        [TestFixture]
        public class TheAutoRegisterTypesViaAttributes
        {
            #region Methods
            [TestCase]
            public void RegistersTheImplementationTypesAsInterfaceTypesIfIsSetToTrue()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.AutoRegisterTypesViaAttributes = true;

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton<IFooService>());

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                    Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton<IFooService2>());

                    var resolveType = serviceLocator.ResolveType<IFooService>();
                    Assert.IsInstanceOf(typeof(FooService), resolveType);

                    var resolveType2 = serviceLocator.ResolveType<IFooService2>();
                    Assert.IsInstanceOf(typeof(FooService2), resolveType2);
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionIfIgnoreRuntimeIncorrectUsageOfRegisterAttributePropertyIsSetToFalseAndSetBackToFalse()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.IgnoreRuntimeIncorrectUsageOfRegisterAttribute = false;

                    Assert.Throws<InvalidOperationException>(() => serviceLocator.AutoRegisterTypesViaAttributes = true);
                    Assert.IsFalse(serviceLocator.AutoRegisterTypesViaAttributes);
                }
            }

            [TestCase]
            public void DoesNotRegisterTheImplementationTypesAsTheInterfaceTypesIfIsSetToFalse()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.AutoRegisterTypesViaAttributes = false;

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void DoesNotRegisterTheImplementationTypesAsTheInterfaceTypesByDefault()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }
            #endregion

            #region Nested type: FooService
            [ServiceLocatorRegistration(typeof(IFooService))]
            public class FooService : IFooService
            {
            }
            #endregion

            #region Nested type: FooService2
            [ServiceLocatorRegistration(typeof(IFooService2), ServiceLocatorRegistrationMode.Transient)]
            public class FooService2 : IFooService2
            {
            }
            #endregion

            #region Nested type: IFooService
            public interface IFooService
            {
            }
            #endregion

            #region Nested type: IFooService2
            public interface IFooService2
            {
            }
            #endregion

            #region Nested type: NonFooService
            [ServiceLocatorRegistration(typeof(IFooService))]
            public class NonFooService
            {
            }
            #endregion
        }

        [TestFixture]
        public class TheAutoRegisterTypesViaConventions
        {
            public interface IFooService
            {
            }

            public interface IFooService2
            {
            }

            public class FooService : IFooService
            {
            }

            public class FooService2 : IFooService2
            {
            }

            public class FooService3 : IFooService, IFooService2
            {
            }

            public class NonFooService
            {
            }

            public interface IWindowsIdentityService { }

            public class WindowsIdentityService : IWindowsIdentityService { }

            [TestCase]
            public void RegistersWithDefaultNamingConvention()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IRegistrationConventionHandler, RegistrationConventionHandler>();

                    serviceLocator.RegisterTypesUsingDefaultNamingConvention();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                }
            }

            //Test case for issue CTL-673
            [TestCase]
            public void RegistersWithDefaultNamingConventionForITwice()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IRegistrationConventionHandler, RegistrationConventionHandler>();

                    serviceLocator.RegisterTypesUsingDefaultNamingConvention();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IWindowsIdentityService>());
                }
            }

            [TestCase]
            public void RegistersWithDefaultFirstInterfaceConvention()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<IRegistrationConventionHandler, RegistrationConventionHandler>();

                    serviceLocator.RegisterTypesUsingDefaultFirstInterfaceConvention();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());

                    var services = serviceLocator.ResolveTypes<IFooService>();

                    Assert.IsNotNull(services.OfType<FooService3>());
                }
            }

            [TestCase]
            public void ShouldExcludeAllTypesOfNamespaceContainingSpecifiedInterfaceType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeAllTypesOfNamespaceContaining<IFooService>();

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldExcludeAllTypesOfNamespaceContainingSpecifiedClassType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeAllTypesOfNamespaceContaining<FooService>();

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldExcludeAllTypesOfTheSpecifiedNamespace()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeAllTypesOfNamespace("Catel.Tests.IoC");

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldExcludeSpecifiedInterfaceType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeType<IFooService2>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldExcludeSpecifiedClassType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeType<FooService2>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldExcludeTypeUsingSpecifiedPredicate()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeTypesWhere(type => type == typeof(IFooService2));

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldIncludeSpecifiedInterfaceType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .IncludeType<IFooService2>()
                              .IncludeType<FooService2>();

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldIncludeSpecifiedClassType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .IncludeType<IFooService2>()
                              .IncludeType<FooService2>();

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldIncludeTypeUsingSpecifiedPredicate()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .IncludeTypesWhere(type => type == typeof(IFooService2))
                              .IncludeType<FooService2>();

                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                }
            }

            [TestCase]
            public void ShouldIncludeAllTypesOfTheSpecifiedNamespace()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .IncludeAllTypesOfNamespace("Catel.Tests.IoC");

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IService>());
                }
            }

            [TestCase]
            public void ShouldIncludeAllTypesOfNamespaceContaining()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .IncludeAllTypesOfNamespaceContaining<IFooService>();

                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                    Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                    Assert.IsFalse(serviceLocator.IsTypeRegistered<IService>());
                }
            }
        }

        [TestFixture]
        public class TheResolveTypesMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsAllAvaliableInstances()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.AutoRegisterTypesViaAttributes = true;

                    serviceLocator.RegisterInstance(typeof(IFooService), new FooService2(), "FooService3");

                    Assert.AreEqual(3, serviceLocator.ResolveTypes<IFooService>().Count());
                }
            }
            #endregion

            #region Nested type: FooService1
            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService1")]
            public class FooService1 : IFooService
            {
            }
            #endregion

            #region Nested type: FooService2
            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.Transient, "FooService2")]
            public class FooService2 : IFooService
            {
            }
            #endregion

            #region Nested type: IFooService
            public interface IFooService
            {
            }
            #endregion
        }

        [TestFixture]
        public class TheAreMultipleTypesRegisteredMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeArray()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.AreMultipleTypesRegistered(null));
                }
            }

            [TestCase]
            public void ReturnsFalseWhenAtLeastOneTypeIsNotRegistered()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<object>();
                    serviceLocator.RegisterType<ITestInterface1, TestClass1>();

                    Assert.IsFalse(serviceLocator.AreMultipleTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
                }
            }

            [TestCase]
            public void ReturnsTrueWhenAllTypesAreRegistered()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<object>();
                    serviceLocator.RegisterType<ITestInterface1, TestClass1>();
                    serviceLocator.RegisterType<ITestInterface2, TestClass2>();

                    Assert.IsTrue(serviceLocator.AreMultipleTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheResolveMultipleMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeArray()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<ArgumentNullException>(() => serviceLocator.ResolveMultipleTypes(null));
                }
            }

            [TestCase]
            public void ThrowsTypeNotRegisteredExceptionWhenAtLeastOneTypeIsNotRegistered()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<object>();
                    serviceLocator.RegisterType<ITestInterface1, TestClass1>();

                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveMultipleTypes(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
                }
            }

            [TestCase]
            public void ReturnsAllTypesWhenAllAreRegistered()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance(new object());
                    serviceLocator.RegisterType<ITestInterface1, TestClass1>();
                    serviceLocator.RegisterType<ITestInterface2, TestClass2>();

                    var resolvedTypes = serviceLocator.ResolveMultipleTypes(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)).ToArray();

                    Assert.AreEqual(3, resolvedTypes.Length);
                    Assert.AreEqual(typeof(object), resolvedTypes[0].GetType());
                    Assert.AreEqual(typeof(TestClass1), resolvedTypes[1].GetType());
                    Assert.AreEqual(typeof(TestClass2), resolvedTypes[2].GetType());
                }
            }
            #endregion
        }

        [TestFixture]
        public class TheResolveTypeWithCircularDependenciesBehavior
        {
            public class InterfaceA
            {
            }

            public class ClassA : InterfaceA
            {
                public ClassA(InterfaceB b)
                {
                    Argument.IsNotNull(() => b);
                }
            }

            public class InterfaceB
            {
            }

            public class ClassB : InterfaceB
            {
                public ClassB(InterfaceA a)
                {
                    Argument.IsNotNull(() => a);
                }
            }

            [TestCase]
            public void ThrowsCircularDependencyException()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<InterfaceA, ClassA>();
                    serviceLocator.RegisterType<InterfaceB, ClassB>();

                    Assert.Throws<CircularDependencyException>(() => serviceLocator.ResolveType<InterfaceA>());
                }
            }
        }

        [TestFixture]
        public class TheResolveMissingType
        {
            private interface IDummy
            {

            }

            public class Dummy : IDummy
            {

            }

            [TestCase]
            public void ThrowsTypeNotRegisteredException()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType<IDummy>());
                }
            }

            [TestCase]
            public void ThrowsTypeNotRegisteredByTagException()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDummy), typeof(Dummy), "SomeTag");

                    Assert.IsNotNull(serviceLocator.ResolveType(typeof(IDummy), "SomeTag"));
                    Assert.Throws<TypeNotRegisteredException>(() => serviceLocator.ResolveType<IDummy>());
                }
            }
        }
    }
}
