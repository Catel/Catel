namespace Catel.Tests.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Caching;
    using Catel.IoC;
    using Catel.MVVM;
    using Catel.Runtime.Serialization;
    using Catel.Services;
    using Catel.Tests.Data;
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
#pragma warning disable IDISP001 // Dispose created.
                var parentServiceLocator = new ServiceLocator();
#pragma warning restore IDISP001 // Dispose created.

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

            [TestCase]
            public void UsesCorrectTypeFactoryToConstructTypes()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var service3 = serviceLocator.ResolveType<IService3>();

                    Assert.That(service3, Is.Not.Null);
                }
            }

            [TestCase]
            public void ResolvesTypeFromItself()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var selfService = serviceLocator.ResolveType<IAppDataService>();

                    Assert.That(selfService, Is.Not.Null);
                    Assert.That(selfService, Is.InstanceOf<CustomAppDataService>());
                }
            }

            [TestCase]
            public void ResolvesTypeFromParent()
            {
                using (var serviceLocator = CreateLocator())
                {
                    var parentService = serviceLocator.ResolveType<ISerializationManager>();

                    Assert.That(parentService, Is.Not.Null);
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

                    Assert.That(isDisposed, Is.True);
                }
            }
        }

        [TestFixture]
        public class TheDeadLockPrevention
        {
            // Note that this class contains very bad code practices, but this way we try to mimic a deadlock

            #region Constants
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
            private static IServiceLocator _serviceLocator;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
#pragma warning restore IDE1006 // Naming Styles
            #endregion

            #region Methods
            [TestCase]
            public void DeadlockIsNotCausedByMultipleInheritedResolving()
            {
#pragma warning disable IDISP007 // Don't dispose injected
                _serviceLocator?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected
                _serviceLocator = IoCFactory.CreateServiceLocator();

                var serviceLocator = _serviceLocator;
                var typeFactory = serviceLocator.ResolveRequiredType<ITypeFactory>();
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

                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.False);
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "1"), Is.True);
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "2"), Is.True);
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
                    Assert.That(firstService.GetType(), Is.EqualTo(typeof(TestClass2)));
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
                    Assert.That(firstService.GetType(), Is.EqualTo(typeof(TestClass3)));
                    Assert.That(ReferenceEquals(testInterface1, firstService.TestInterface1), Is.True);
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
                    Assert.That(resolvedObject is List<int>, Is.True);
                }
            }

            [TestCase]
            public void CorrectlyResolvesOpenGenericTypeWithSingleInnerType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IList<>), typeof(List<>));

                    var resolvedObject = serviceLocator.ResolveType<IList<int>>();
                    Assert.That(resolvedObject is List<int>, Is.True);
                }
            }

            [TestCase]
            public void CorrectlyResolvesClosedGenericTypeWithMultipleInnerTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDictionary<string, int>), typeof(Dictionary<string, int>));

                    var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                    Assert.That(resolvedObject is Dictionary<string, int>, Is.True);
                }
            }

            [TestCase]
            public void CorrectlyResolvesOpenGenericTypeWithMultipleInnerTypes()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));

                    var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                    Assert.That(resolvedObject is Dictionary<string, int>, Is.True);
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
                    Assert.That(serviceLocator.IsTypeRegisteredAsSingleton<ITestInterface>(), Is.True);
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_NonSingleton()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    Assert.That(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)), Is.False);
                }
            }

            [TestCase]
            public void TheIsTypeRegisteredAsSingleton_UnRegisteredType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.That(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)), Is.False);
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
                    Assert.That(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)), Is.False);
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

                    Assert.That(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)), Is.True);
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);
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
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.False);
                }
            }

            [TestCase]
            public void IsTypeRegistered_UnregisteredTypeRegisteredInstanceViaMissingType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.MissingType += (sender, args) => { args.ImplementingInstance = new TestClass1(); };

                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.True);
                }
            }

            [TestCase]
            public void IsTypeRegistered_RegisteredAsInstanceInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.True);
                }
            }

            [TestCase]
            public void IsTypeRegistered_RegisteredAsTypeInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.True);
                }
            }

            [TestCase]
            public void IsTypeRegistered_UnregisteredTypeRegisteredViaMissingTypeEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.False);

                    serviceLocator.MissingType += (sender, e) => e.ImplementingType = typeof(TestClass1);
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface)), Is.True);
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(instance.Name, Is.EqualTo("My Instance"));
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(instance.Name, Is.EqualTo("My Instance"));
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

                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.ServiceType, Is.EqualTo(typeof(ITestInterface)));
                    Assert.That(eventArgs.ServiceImplementationType, Is.EqualTo(typeof(TestClass2)));
                    Assert.That(eventArgs.RegistrationType, Is.EqualTo(RegistrationType.Singleton));
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    serviceLocator.RegisterType<ITestInterface, TestClass2>();

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);
                    Assert.That(serviceLocator.ResolveType<ITestInterface>(), Is.InstanceOf(typeof(TestClass2)));
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
                    Assert.That(testInterfaceRef2, Is.SameAs(testInterfaceRef1));

                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(testInterfaceRef2, Is.Not.SameAs(testInterfaceRef1));
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
                    Assert.That(testInterfaceRef2, Is.SameAs(testInterfaceRef1));
                    Assert.That(typeof(TestClass1), Is.EqualTo(testInterfaceRef2.GetType()));

                    serviceLocator.RegisterType<ITestInterface, TestClass2>(registrationType: RegistrationType.Transient);
                    testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                    testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(testInterfaceRef2, Is.Not.SameAs(testInterfaceRef1));

                    Assert.That(typeof(TestClass2), Is.EqualTo(testInterfaceRef2.GetType()));
                }
            }

            [TestCase]
            public void RegisterType_DoubleRegistration_WitoutChangeInstantiationStyle_And_JustChangingTheType()
            {
                ServiceLocator.Default.RegisterType<ITestInterface, TestClass1>();
                var testInterfaceRef1 = ServiceLocator.Default.ResolveType<ITestInterface>();
                var testInterfaceRef2 = ServiceLocator.Default.ResolveType<ITestInterface>();
                Assert.That(testInterfaceRef2, Is.SameAs(testInterfaceRef1));
                Assert.That(typeof(TestClass1), Is.EqualTo(testInterfaceRef2.GetType()));

                ServiceLocator.Default.RegisterType<ITestInterface, TestClass2>();
                testInterfaceRef1 = ServiceLocator.Default.ResolveType<ITestInterface>();
                testInterfaceRef2 = ServiceLocator.Default.ResolveType<ITestInterface>();
                Assert.That(testInterfaceRef2, Is.SameAs(testInterfaceRef1));

                Assert.That(typeof(TestClass2), Is.EqualTo(testInterfaceRef2.GetType()));
            }

            [TestCase]
            public void InvokesTypeRegisteredEvent()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    TypeRegisteredEventArgs eventArgs = null;

                    serviceLocator.TypeRegistered += (sender, args) => { eventArgs = args; };
                    serviceLocator.RegisterType<ITestInterface, TestClass2>(registrationType: RegistrationType.Transient);

                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.ServiceType, Is.EqualTo(typeof(ITestInterface)));
                    Assert.That(eventArgs.ServiceImplementationType, Is.EqualTo(typeof(TestClass2)));
                    Assert.That(eventArgs.RegistrationType, Is.EqualTo(RegistrationType.Transient));
                }
            }

            [TestCase]
            public void RegistersLateBoundImplementationUsingCallback()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface>((tf, reg) => new TestClass2());

                    var resolvedClass = serviceLocator.ResolveType<ITestInterface>();

                    Assert.That(resolvedClass.GetType(), Is.EqualTo(typeof(TestClass2)));
                }
            }

            [TestCase]
            public void RegistersLateBoundImplementationUsingLateBoundImplementationType()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface>((tf, reg) => new TestClass2());

                    var registeredTypeInfo = serviceLocator.GetRegistrationInfo(typeof(ITestInterface));

                    Assert.That(registeredTypeInfo.IsLateBoundRegistration, Is.True);
                    Assert.That(registeredTypeInfo.ImplementingType, Is.EqualTo(typeof(LateBoundImplementation)));
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    serviceLocator.RemoveType(typeof(ITestInterface));

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.False);
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

                    Assert.That(serviceLocator.ResolveType(typeof(ITestInterface), "1"), Is.Null);
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "2"), Is.True);
                    Assert.That(object.ReferenceEquals(ref1, ref2), Is.True);
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
                        Assert.That(e.ServiceType, Is.EqualTo(typeof(ITestInterface)));
                        Assert.That(e.ServiceImplementationType, Is.EqualTo(typeof(TestClass1)));
                        Assert.That(e.RegistrationType, Is.EqualTo(RegistrationType.Singleton));
                        Assert.That(e.Instance, Is.Null);

                        raisedEvent = true;
                    };

                    serviceLocator.RemoveType(typeof(ITestInterface), "1");

                    Assert.That(raisedEvent, Is.True);
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

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    serviceLocator.RemoveAllTypes(typeof(ITestInterface));

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.False);
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

                    Assert.That(serviceLocator.ResolveType(typeof(ITestInterface), "1"), Is.Null);
                    Assert.That(serviceLocator.ResolveType(typeof(ITestInterface), "2"), Is.Null);
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

                    Assert.That(nonInstanceCounter, Is.EqualTo(2));
                    Assert.That(instanceCounter, Is.EqualTo(1));
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
                    Assert.That(dependencyInjectionTestClass, Is.Not.Null);
                    Assert.That(serviceLocator.IsTypeRegistered(typeof(DependencyInjectionTestClass)), Is.False);
                }
            }

            [TestCase]
            public void ResolveType_Of_Non_Registered_Non_Abstract_Class_Without_Registration_CanResolveNonAbstractTypesWithoutRegistration_In_False()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.CanResolveNonAbstractTypesWithoutRegistration = false;
                    Assert.That(serviceLocator.ResolveType(typeof(DependencyInjectionTestClass)), Is.Null);
                }
            }

            [TestCase]
            public void ResolveType_Generic_TransientLifestyle()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(firstInstance, Is.InstanceOf(typeof(TestClass1)));

                    var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(secondInstance, Is.InstanceOf(typeof(TestClass1)));

                    Assert.That(secondInstance, Is.Not.SameAs(firstInstance));
                }
            }

            [TestCase]
            public void ResolveType_Generic_SingletonLifestyle()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();
                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);

                    var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(firstInstance, Is.InstanceOf(typeof(TestClass1)));

                    var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(secondInstance, Is.InstanceOf(typeof(TestClass1)));

                    Assert.That(secondInstance, Is.SameAs(firstInstance));
                }
            }

            [TestCase]
            public void ResolveType_Generic()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.True);
                    Assert.That(serviceLocator.ResolveType<ITestInterface>(), Is.InstanceOf(typeof(TestClass1)));
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
                    Assert.That(serviceLocator.IsTypeRegistered<ITestInterface>(), Is.False);
                    Assert.That(serviceLocator.ResolveType(typeof(ITestInterface)), Is.Null);
                }
            }

            [TestCase]
            public void ResolveType_RegisteredAsInstanceInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterInstance<ITestInterface>(new TestClass2 { Name = "instance test" });

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(instance.Name, Is.EqualTo("instance test"));

                    instance.Name = "changed name";
                    var newInstance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(newInstance.Name, Is.EqualTo("changed name"));
                    Assert.That(newInstance, Is.EqualTo(instance));
                    Assert.That(object.ReferenceEquals(instance, newInstance), Is.True);
                }
            }

            [TestCase]
            public void ResolveType_RegisteredAsTypeInServiceLocator()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    var instance = serviceLocator.ResolveType<ITestInterface>();
                    Assert.That(instance, Is.InstanceOf(typeof(TestClass1)));
                }
            }

            [TestCase]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();

                    var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                    Assert.That(instance.UsedDefaultConstructor, Is.True);
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

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(0));
                    Assert.That(instance.StringValue, Is.EqualTo(null));
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

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(42));
                    Assert.That(instance.StringValue, Is.EqualTo(null));
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

                    Assert.That(instance.UsedDefaultConstructor, Is.False);
                    Assert.That(instance.IniEntry, Is.EqualTo(iniEntry));
                    Assert.That(instance.IntValue, Is.EqualTo(42));
                    Assert.That(instance.StringValue, Is.EqualTo("hi there"));
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

                    Assert.That(invoked, Is.True);
                }
            }

            [TestCase]
            public void DoesNotCorruptTypeRequestPathOnMultipleCallsWithExceptions()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.ResolveType<ITestInterface>();

                    serviceLocator.RegisterType<ITestInterface, TestClass1>();

                    Assert.That(serviceLocator.ResolveType<ITestInterface>(), Is.Not.Null);
                }
            }
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

                    Assert.That(serviceLocator.AreMultipleTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)), Is.False);
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

                    Assert.That(serviceLocator.AreMultipleTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)), Is.True);
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

                    var results = serviceLocator.ResolveMultipleTypes(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2));

                    Assert.That(results[0] is object, Is.True);
                    Assert.That(results[1] is ITestInterface1, Is.True);
                    Assert.That(results[2], Is.Null);
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

                    Assert.That(resolvedTypes.Length, Is.EqualTo(3));
                    Assert.That(resolvedTypes[0].GetType(), Is.EqualTo(typeof(object)));
                    Assert.That(resolvedTypes[1].GetType(), Is.EqualTo(typeof(TestClass1)));
                    Assert.That(resolvedTypes[2].GetType(), Is.EqualTo(typeof(TestClass2)));
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
                    ArgumentNullException.ThrowIfNull(b);
                }
            }

            public class InterfaceB
            {
            }

            public class ClassB : InterfaceB
            {
                public ClassB(InterfaceA a)
                {
                    ArgumentNullException.ThrowIfNull(a);
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
                    Assert.That(serviceLocator.ResolveType<IDummy>(), Is.Null);
                }
            }

            [TestCase]
            public void ThrowsTypeNotRegisteredByTagException()
            {
                using (var serviceLocator = IoCFactory.CreateServiceLocator())
                {
                    serviceLocator.RegisterType(typeof(IDummy), typeof(Dummy), "SomeTag");

                    Assert.That(serviceLocator.ResolveType(typeof(IDummy), "SomeTag"), Is.Not.Null);
                    Assert.That(serviceLocator.ResolveType<IDummy>(), Is.Null);
                }
            }
        }
    }
}
