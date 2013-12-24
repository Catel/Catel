namespace Catel.Test.IoC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.MVVM.Services;
    using Catel.Reflection;
    using Catel.Test.Logging;

    using Data;

#if !NETFX_CORE
    using Castle.Windsor;
    using Microsoft.Practices.Unity;
    using Ninject;
#endif

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ServiceLocatorFacts
    {
#if !NETFX_CORE
        [TestClass]
        public class ExternalContainersLifestyleSynchronizationTest
        {
            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToFalseFromUnityContainer()
            {
                var serviceLocator = new ServiceLocator();
                var unityContainer = new UnityContainer();
                serviceLocator.RegisterExternalContainer(unityContainer);
                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                Assert.AreNotSame(unityContainer.Resolve<ITestInterface>(), unityContainer.Resolve<ITestInterface>());
            }

            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToTrueFromUnityContainer()
            {
                var serviceLocator = new ServiceLocator();
                var unityContainer = new UnityContainer();
                serviceLocator.RegisterExternalContainer(unityContainer);
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                Assert.AreSame(unityContainer.Resolve<ITestInterface>(), unityContainer.Resolve<ITestInterface>());
            }

            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToFalseFromWindsorContainer()
            {
                var serviceLocator = new ServiceLocator();
                var windsorContainer = new WindsorContainer();
                serviceLocator.RegisterExternalContainer(windsorContainer);
                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                Assert.AreNotSame(windsorContainer.Resolve<ITestInterface>(), windsorContainer.Resolve<ITestInterface>());
            }

            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToTrueFromWindsorContainer()
            {
                var serviceLocator = new ServiceLocator();
                var windsorContainer = new WindsorContainer();
                serviceLocator.RegisterExternalContainer(windsorContainer);
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                Assert.AreSame(windsorContainer.Resolve<ITestInterface>(), windsorContainer.Resolve<ITestInterface>());
            }


            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToFalseFromNinjectContainer()
            {
                var serviceLocator = new ServiceLocator();
                var standardKernel = new StandardKernel();
                serviceLocator.RegisterExternalContainer(standardKernel);
                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                Assert.AreNotSame(standardKernel.Get<ITestInterface>(), standardKernel.Get<ITestInterface>());
            }

            [TestMethod]
            public void ResolvesInstancesOfTypeRegisteredWithSingletonParameterToTrueFromNinjectContainer()
            {
                var serviceLocator = new ServiceLocator();
                var standardKernel = new StandardKernel();
                serviceLocator.RegisterExternalContainer(standardKernel);
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                Assert.AreSame(standardKernel.Get<ITestInterface>(), standardKernel.Get<ITestInterface>());
            }

            //[TestMethod]
            //public void AllowsReRegistrationAndSynchronizationOfExternalContainers()
            //{
            //    var serviceLocator = new ServiceLocator();
            //    var unityContainer = new UnityContainer();

            //    serviceLocator.AutomaticallyKeepContainersSynchronized = true;
            //    serviceLocator.RegisterExternalContainer(unityContainer);

            //    // register custom services through catel
            //    serviceLocator.RegisterType<IUIVisualizerService, Catel.MVVM.Services.Test.UIVisualizerService>();

            //    // register custom service through unity directly
            //    unityContainer.RegisterType<IUIVisualizerService, Catel.MVVM.Services.Test.UIVisualizerService>();

            //    // get service through catel
            //    var x = serviceLocator.ResolveType<IUIVisualizerService>();

            //    // get service through unity
            //    var x1 = unityContainer.Resolve<IUIVisualizerService>();

            //    // both containers are equal now with the same service
            //    Assert.AreEqual(x.GetType(), x1.GetType());

            //    // register standard catel service (should be also exported to external container)
            //    serviceLocator.RegisterType<IUIVisualizerService, UIVisualizerService>();

            //    // get service through catel
            //    x = serviceLocator.ResolveType<IUIVisualizerService>();

            //    // get service through unity
            //    x1 = unityContainer.Resolve<IUIVisualizerService>();

            //    Assert.AreEqual(x.GetType(), x1.GetType());
            //}
        }
#endif

        [TestClass]
        public class TheTagSupportFunctionality
        {
            [TestMethod]
            public void AllowsTheSameInterfaceDefinedTwiceWithDifferentTags()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");

                Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "1"));
                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface), "2"));
            }

            [TestMethod]
            public void OverridesRegistrationWithSameTag()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "1");
                serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass1), "2");
                serviceLocator.RegisterType(typeof(ITestInterface), typeof(TestClass2), "1");

                var firstService = serviceLocator.ResolveType(typeof(ITestInterface), "1");
                Assert.AreEqual(typeof(TestClass2), firstService.GetType());
            }
        }

        [TestClass]
        public class TheGenericTypesSupport
        {
            [TestMethod]
            public void CorrectlyResolvesClosedGenericTypeWithSingleInnerType()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType(typeof(IList<int>), typeof(List<int>));

                var resolvedObject = serviceLocator.ResolveType<IList<int>>();
                Assert.IsTrue(resolvedObject is List<int>);
            }

            [TestMethod]
            public void CorrectlyResolvesOpenGenericTypeWithSingleInnerType()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType(typeof(IList<>), typeof(List<>));

                var resolvedObject = serviceLocator.ResolveType<IList<int>>();
                Assert.IsTrue(resolvedObject is List<int>);
            }

            [TestMethod]
            public void CorrectlyResolvesClosedGenericTypeWithMultipleInnerTypes()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType(typeof(IDictionary<string, int>), typeof(Dictionary<string, int>));

                var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                Assert.IsTrue(resolvedObject is Dictionary<string, int>);
            }

            [TestMethod]
            public void CorrectlyResolvesOpenGenericTypeWithMultipleInnerTypes()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>));

                var resolvedObject = serviceLocator.ResolveType<IDictionary<string, int>>();
                Assert.IsTrue(resolvedObject is Dictionary<string, int>);
            }
        }

        [TestClass]
        public class TheIsTypeRegisteredAsSingletonMethod
        {
            [TestMethod]
            public void TheIsTypeRegisteredAsSingleton_Generic()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton<ITestInterface>());
            }

            [TestMethod]
            public void TheIsTypeRegisteredAsSingleton_NonSingleton()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
            }

            [TestMethod]
            public void TheIsTypeRegisteredAsSingleton_UnRegisteredType()
            {
                var serviceLocator = new ServiceLocator();
                Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
            }

            [TestMethod]
            public void TheIsTypeRegisteredAsSingleton_RegisteredTypeViaMissingTypeEvent()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.MissingType += (sender, args) =>
                    {
                        args.ImplementingType = typeof(TestClass1);
                        args.RegistrationType = RegistrationType.Transient;
                    };
                Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
            }

            [TestMethod]
            public void TheIsTypeRegisteredAsSingleton_RegisteredInstanceViaMissingTypeEvent()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.MissingType += (sender, args) =>
                {
                    args.ImplementingInstance = new TestClass1();
                    // NOTE: This value will be ignored, read the docs.
                    args.RegistrationType = RegistrationType.Transient;
                };

                Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton(typeof(ITestInterface)));
            }
        }

        [TestClass]
        public class TheIsTypeRegisteredMethod
        {
            [TestMethod]
            public void IsTypeRegistered_Generic()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
            }

            [TestMethod]
            public void IsTypeRegistered_Null()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.IsTypeRegistered(null));
            }

            [TestMethod]
            public void IsTypeRegistered_UnregisteredType()
            {
                var serviceLocator = new ServiceLocator();

                Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }

            [TestMethod]
            public void IsTypeRegistered_UnregisteredTypeRegisteredInstanceViaMissingType()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.MissingType += (sender, args) =>
                    {
                        args.ImplementingInstance = new TestClass1();
                    };

                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }


            [TestMethod]
            public void IsTypeRegistered_RegisteredAsInstanceInServiceLocator()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }

            [TestMethod]
            public void IsTypeRegistered_RegisteredAsTypeInServiceLocator()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }

#if !NETFX_CORE
            [TestMethod]
            public void IsTypeRegistered_RegisteredTypeInExternalContainer()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                ninjectContainer.Bind<ITestInterface>().To<TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }
#endif

            [TestMethod]
            public void IsTypeRegistered_UnregisteredTypeRegisteredViaMissingTypeEvent()
            {
                var serviceLocator = new ServiceLocator();
                Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));

                serviceLocator.MissingType += (sender, e) => e.ImplementingType = typeof(TestClass1);
                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }
        }

        [TestClass]
        public class TheRegisterInstanceMethod
        {
            [TestMethod]
            public void RegisterInstance_Generic()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1 { Name = "My Instance" });

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                var instance = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreEqual("My Instance", instance.Name);
            }

            [TestMethod]
            public void RegisterInstance_InterfaceTypeNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(
                    () => serviceLocator.RegisterInstance(null, new TestClass1 { Name = "My Instance" }, null));
            }

            [TestMethod]
            public void RegisterInstance_InstanceNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(
                    () => serviceLocator.RegisterInstance(typeof(ITestInterface), null, null));
            }

            [TestMethod]
            public void RegisterInstance_Valid()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1 { Name = "My Instance" });

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                var instance = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreEqual("My Instance", instance.Name);
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForWrongServiceType()
            {
                var serviceLocator = new ServiceLocator();
                var serviceType = typeof(IViewModelLocator);
                var serviceInstance = new ViewLocator();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceLocator.RegisterInstance(serviceType, serviceInstance));
            }

            [TestMethod]
            public void InvokesTypeRegisteredEvent()
            {
                var serviceLocator = new ServiceLocator();

                TypeRegisteredEventArgs eventArgs = null;

                serviceLocator.TypeRegistered += (sender, args) => { eventArgs = args; };
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass2());

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(typeof(ITestInterface), eventArgs.ServiceType);
                Assert.AreEqual(typeof(TestClass2), eventArgs.ServiceImplementationType);
                Assert.AreEqual(RegistrationType.Singleton, eventArgs.RegistrationType);
            }
        }

        [TestClass]
        public class TheRegisterTypeMethod
        {
            [TestMethod]
            public void ThrowsInvalidOperationExceptionForInterfaceAsImplementation()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => serviceLocator.RegisterType<ITestInterface, ITestInterface>());
            }

            [TestMethod]
            public void RegisterType_Generic()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
            }

            [TestMethod]
            public void RegisterType_InterfaceTypeNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RegisterType(null, typeof(TestClass1), null, RegistrationType.Singleton, true, null));
            }

            [TestMethod]
            public void RegisterType_ImplementingTypeNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RegisterType(typeof(ITestInterface), null, null, RegistrationType.Singleton, true, null));
            }

            [TestMethod]
            public void RegisterType_Valid()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
            }

            [TestMethod]
            public void RegisterType_DoubleRegistration()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                serviceLocator.RegisterType<ITestInterface, TestClass2>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                Assert.IsInstanceOfType(serviceLocator.ResolveType<ITestInterface>(), typeof(TestClass2));
            }

            [TestMethod]
            public void RegisterType_DoubleRegistration_ToChangeInstantiationStyle()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                var testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                var testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreSame(testInterfaceRef1, testInterfaceRef2);

                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                testInterfaceRef1 = serviceLocator.ResolveType<ITestInterface>();
                testInterfaceRef2 = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreNotSame(testInterfaceRef1, testInterfaceRef2);
            }

            [TestMethod]
            public void RegisterType_DoubleRegistration_ToChangeInstantiationStyle_And_Type()
            {
                var serviceLocator = new ServiceLocator();
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

            [TestMethod]
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

            [TestMethod]
            public void InvokesTypeRegisteredEvent()
            {
                var serviceLocator = new ServiceLocator();

                TypeRegisteredEventArgs eventArgs = null;

                serviceLocator.TypeRegistered += (sender, args) => { eventArgs = args; };
                serviceLocator.RegisterType<ITestInterface, TestClass2>(registrationType: RegistrationType.Transient);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(typeof(ITestInterface), eventArgs.ServiceType);
                Assert.AreEqual(typeof(TestClass2), eventArgs.ServiceImplementationType);
                Assert.AreEqual(RegistrationType.Transient, eventArgs.RegistrationType);
            }
        }

        [TestClass]
        public class TheResolveTypeMethod
        {
            public class DependencyInjectionTestClass
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
            }

            [TestMethod]
            public void ResoleType_Of_Non_Registered_Non_Abstract_Class_Without_Registration()
            {
                var serviceLocator = new ServiceLocator();
                var dependencyInjectionTestClass = serviceLocator.ResolveType<DependencyInjectionTestClass>();
                Assert.IsNotNull(dependencyInjectionTestClass);
                Assert.IsFalse(serviceLocator.IsTypeRegistered(typeof(DependencyInjectionTestClass)));
            }

            [TestMethod]
            public void ResoleType_Of_Non_Registered_Non_Abstract_Class_Without_Registration_CanResolveNonAbstractTypesWithoutRegistration_In_False()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.CanResolveNonAbstractTypesWithoutRegistration = false;
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => serviceLocator.ResolveType(typeof(DependencyInjectionTestClass)));
            }

            [TestMethod]
            public void ResoleType_Generic_TransientLifestyle()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>(registrationType: RegistrationType.Transient);
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(firstInstance, typeof(TestClass1));

                var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(secondInstance, typeof(TestClass1));

                Assert.AreNotSame(firstInstance, secondInstance);
            }

            [TestMethod]
            public void ResoleType_Generic_SingletonLifestyle()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                var firstInstance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(firstInstance, typeof(TestClass1));

                var secondInstance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(secondInstance, typeof(TestClass1));

                Assert.AreSame(firstInstance, secondInstance);
            }

            [TestMethod]
            public void ResolveType_Generic()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
                Assert.IsInstanceOfType(serviceLocator.ResolveType<ITestInterface>(), typeof(TestClass1));
            }


            [TestMethod]
            public void ResolveType_InterfaceTypeNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.ResolveType(null));
            }

            [TestMethod]
            public void ResolveType_UnregisteredType()
            {
                var serviceLocator = new ServiceLocator();

                Assert.IsFalse(serviceLocator.IsTypeRegistered<ITestInterface>());
                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => serviceLocator.ResolveType(typeof(ITestInterface)));
            }

            [TestMethod]
            public void ResolveType_RegisteredAsInstanceInServiceLocator()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass2 { Name = "instance test" });

                var instance = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreEqual("instance test", instance.Name);

                instance.Name = "changed name";
                var newInstance = serviceLocator.ResolveType<ITestInterface>();
                Assert.AreEqual("changed name", newInstance.Name);
                Assert.AreEqual(instance, newInstance);
                Assert.IsTrue(object.ReferenceEquals(instance, newInstance));
            }

            [TestMethod]
            public void ResolveType_RegisteredAsTypeInServiceLocator()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                var instance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(instance, typeof(TestClass1));
            }

#if !NETFX_CORE
            [TestMethod]
            public void ResolveType_RegisteredInExternalContainer()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                ninjectContainer.Bind<ITestInterface>().To<TestClass1>();

                var instance = serviceLocator.ResolveType<ITestInterface>();
                Assert.IsInstanceOfType(instance, typeof(TestClass1));
            }
#endif

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToDefaultConstructor()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();

                var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                Assert.IsTrue(instance.UsedDefaultConstructor);
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToFirstConstructor()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<DependencyInjectionTestClass, DependencyInjectionTestClass>();
                var iniEntry = new IniEntry { Group = "group", Key = "key", Value = "value" };
                serviceLocator.RegisterInstance(iniEntry);

                var instance = serviceLocator.ResolveType<DependencyInjectionTestClass>();

                Assert.IsFalse(instance.UsedDefaultConstructor);
                Assert.AreEqual(iniEntry, instance.IniEntry);
                Assert.AreEqual(0, instance.IntValue);
                Assert.AreEqual(null, instance.StringValue);
            }

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionFallBackToSecondConstructor()
            {
                var serviceLocator = new ServiceLocator();
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

            [TestMethod]
            public void ResolvesTypeUsingDependencyInjectionUsesConstructorWithMostParametersFirst()
            {
                var serviceLocator = new ServiceLocator();
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

        [TestClass]
        public class TheIsExternalContainerSupportedMethod
        {
            [TestMethod]
            public void IsExternalContainerSupported_ExternalContainerNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.IsExternalContainerSupported(null));
            }

            [TestMethod]
            public void IsExternalContainerSupported_UnsupportedContainer()
            {
                var serviceLocator = new ServiceLocator();
                var container = new object();

                Assert.IsFalse(serviceLocator.IsExternalContainerSupported(container));
            }

#if !NETFX_CORE
            [TestMethod]
            public void IsExternalContainerSupported_SupportedContainer()
            {
                var serviceLocator = new ServiceLocator();
                var container = new StandardKernel();

                Assert.IsTrue(serviceLocator.IsExternalContainerSupported(container));
            }
#endif
        }

        [TestClass]
        public class TheRegisterExternalContainerMethod
        {
            [TestMethod]
            public void RegisterExternalContainer_ExternalContainerNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RegisterExternalContainer(null));
            }

#if !NETFX_CORE
            [TestMethod]
            public void RegisterExternalContainer_ValidContainerAndTestTypeRetrieval()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                ninjectContainer.Bind<ITestInterface>().To<TestClass1>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered(typeof(ITestInterface)));
            }
#endif
        }

        [TestClass]
        public class TheRegisterExternalContainerHelperMethod
        {
            [TestMethod]
            public void RegisterExternalContainerHelper_Null()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RegisterExternalContainerHelper(null));
            }
        }

#if !NETFX_CORE
        [TestClass]
        public class TheAutomaticSynchronizationProperty
        {
            [TestMethod]
            public void AutomaticSynchronization_RegisterExternalContainer()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
            }

            [TestMethod]
            public void AutomaticSynchronization_RegisterInstance()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
            }

            [TestMethod]
            public void AutomaticSynchronization_RegisterType()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterType<ITestInterface, TestClass1>();

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
            }

            [TestMethod]
            public void AutomaticSynchronization_RegisterIfTypeNotYetRegistered()
            {
                var serviceLocator = new ServiceLocator();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterTypeIfNotYetRegistered<ITestInterface, TestClass1>();

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
            }

            [TestMethod]
            public void AutomaticSynchronization_ResolveType()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.AutomaticallyKeepContainersSynchronized = false;
                serviceLocator.RegisterType<ITestInterface, TestClass1>();
                var ninjectContainer = new StandardKernel();
                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.AutomaticallyKeepContainersSynchronized = true;
                serviceLocator.ResolveType<ITestInterface>();

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
            }

            [TestMethod]
            public void ResolvesSameInstanceFromBothContainers()
            {
                var unityContainer = new UnityContainer();

                var serviceLocator = new ServiceLocator();
                serviceLocator.AutomaticallyKeepContainersSynchronized = true;
                serviceLocator.RegisterExternalContainer(unityContainer);

                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();

                var ns1 = unityContainer.Resolve<IMessageService>();
                var ns2 = serviceLocator.ResolveType<IMessageService>();

                Assert.AreEqual(ns1, ns2);
            }
        }

        [TestClass]
        public class TheExportInstancesToExternalContainersMethod
        {
            [TestMethod]
            public void ExportInstancesToExternalContainers_ExternalContainerAlreadyHasInstanceRegistered()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.AutomaticallyKeepContainersSynchronized = false;

                var ninjectContainer = new StandardKernel();
                ninjectContainer.Bind<ITestInterface>().ToConstant(new TestClass1());

                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                serviceLocator.RegisterExternalContainer(ninjectContainer);

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                // Should not throw exception
                serviceLocator.ExportInstancesToExternalContainers();
            }

            [TestMethod]
            public void ExportInstancesToExternalContainers_ExternalContainerHasNoInstanceRegistered()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.AutomaticallyKeepContainersSynchronized = false;

                var ninjectContainer = new StandardKernel();

                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());

                serviceLocator.RegisterExternalContainer(ninjectContainer);

                Assert.IsFalse(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                serviceLocator.ExportInstancesToExternalContainers();

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
            }
        }

        [TestClass]
        public class TheExportToExternalContainersMethod
        {
            [TestMethod]
            public void ExportsBothInstancesAndTypes()
            {
                var serviceLocator = new ServiceLocator();
                serviceLocator.AutomaticallyKeepContainersSynchronized = false;

                var ninjectContainer = new StandardKernel();

                serviceLocator.RegisterExternalContainer(ninjectContainer);
                serviceLocator.RegisterInstance<ITestInterface>(new TestClass1());
                serviceLocator.RegisterType<INotifyPropertyChanged, TestClass1>();

                serviceLocator.RegisterExternalContainer(ninjectContainer);

                Assert.IsFalse(ninjectContainer.GetBindings(typeof(INotifyPropertyChanged)).Any());
                Assert.IsFalse(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());

                serviceLocator.ExportToExternalContainers();

                Assert.IsTrue(ninjectContainer.GetBindings(typeof(INotifyPropertyChanged)).Any());
                Assert.IsTrue(ninjectContainer.GetBindings(typeof(ITestInterface)).Any());
                Assert.IsTrue(serviceLocator.IsTypeRegistered<ITestInterface>());
            }
        }
#endif

        [TestClass]
        public class TheAutoRegisterTypesViaAttributes
        {
            public interface IFooService
            {
            }

            public interface IFooService2
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService))]
            public class FooService : IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService2), ServiceLocatorRegistrationMode.Transient)]
            public class FooService2 : IFooService2
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService))]
            public class NonFooService
            {
            }

            [TestMethod]
            public void RegistersTheImplementationTypesAsInterfaceTypesIfIsSetToTrue()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };

                Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                Assert.IsTrue(serviceLocator.IsTypeRegisteredAsSingleton<IFooService>());

                Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService2>());
                Assert.IsFalse(serviceLocator.IsTypeRegisteredAsSingleton<IFooService2>());

                var resolveType = serviceLocator.ResolveType<IFooService>();
                Assert.IsInstanceOfType(resolveType, typeof(FooService));

                var resolveType2 = serviceLocator.ResolveType<IFooService2>();
                Assert.IsInstanceOfType(resolveType2, typeof(FooService2));
            }

            [TestMethod]
            public void ThrowsInvalidOperationExceptionIfIgnoreRuntimeIncorrectUsageOfRegisterAttributePropertyIsSetToFalseAndSetBackToFalse()
            {
                var serviceLocator = new ServiceLocator { IgnoreRuntimeIncorrectUsageOfRegisterAttribute = false };
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => serviceLocator.AutoRegisterTypesViaAttributes = true);
                Assert.IsFalse(serviceLocator.AutoRegisterTypesViaAttributes);
            }

            [TestMethod]
            public void DoesNotRegisterTheImplementationTypesAsTheInterfaceTypesIfIsSetToFalse()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = false };
                Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
            }

            [TestMethod]
            public void DoesNotRegisterTheImplementationTypesAsTheInterfaceTypesByDefault()
            {
                var serviceLocator = new ServiceLocator();
                Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService>());
                Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
            }
        }

        [TestClass]
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

            public class NonFooService
            {
            }

            [TestMethod]
            public void RegistersWithDefaultNamingConvention()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterTypesUsingDefaultNamingConvention();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
            }

            [TestMethod]
            public void ShouldExcludeSpecifiedType()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterTypesUsingDefaultNamingConvention()
                              .ExcludeType<FooService2>();

                Assert.IsTrue(serviceLocator.IsTypeRegistered<IFooService>());
                Assert.IsFalse(serviceLocator.IsTypeRegistered<IFooService2>());
            }
        }

        [TestClass]
        public class TheResolveTypesMethod
        {
            public interface IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService1")]
            public class FooService1 : IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.Transient, "FooService2")]
            public class FooService2 : IFooService
            {
            }

            [TestMethod]
            public void ReturnsAllAvaliableInstances()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };
                serviceLocator.RegisterInstance(typeof(IFooService), new FooService2(), "FooService3");
                Assert.AreEqual(3, serviceLocator.ResolveTypes<IFooService>().Count());
            }
        }

        [TestClass]
        public class TheAreAllTypesRegisteredMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeArray()
            {
                var serviceLocator = new ServiceLocator();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceLocator.AreAllTypesRegistered(null));
            }

            [TestMethod]
            public void ReturnsFalseWhenAtLeastOneTypeIsNotRegistered()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType<object>();
                serviceLocator.RegisterType<ITestInterface1, TestClass1>();

                Assert.IsFalse(serviceLocator.AreAllTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
            }

            [TestMethod]
            public void ReturnsTrueWhenAllTypesAreRegistered()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType<object>();
                serviceLocator.RegisterType<ITestInterface1, TestClass1>();
                serviceLocator.RegisterType<ITestInterface2, TestClass2>();

                Assert.IsTrue(serviceLocator.AreAllTypesRegistered(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
            }
        }

        [TestClass]
        public class TheResolveAllTypesMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullOrEmptyTypeArray()
            {
                var serviceLocator = new ServiceLocator();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceLocator.ResolveAllTypes(null));
            }

            [TestMethod]
            public void ThrowsNotSupportedExceptionWhenAtLeastOneTypeIsNotRegistered()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterType<object>();
                serviceLocator.RegisterType<ITestInterface1, TestClass1>();

                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => serviceLocator.ResolveAllTypes(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)));
            }

            [TestMethod]
            public void ReturnsAllTypesWhenAllAreRegistered()
            {
                var serviceLocator = new ServiceLocator();

                serviceLocator.RegisterInstance(new object());
                serviceLocator.RegisterType<ITestInterface1, TestClass1>();
                serviceLocator.RegisterType<ITestInterface2, TestClass2>();

                var resolvedTypes = serviceLocator.ResolveAllTypes(typeof(object), typeof(ITestInterface1), typeof(ITestInterface2)).ToArray();

                Assert.AreEqual(3, resolvedTypes.Length);
                Assert.AreEqual(typeof(object), resolvedTypes[0].GetType());
                Assert.AreEqual(typeof(TestClass1), resolvedTypes[1].GetType());
                Assert.AreEqual(typeof(TestClass2), resolvedTypes[2].GetType());
            }
        }

        [TestClass]
        public class TheRemoveInstanceMethod
        {
            public interface IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService1")]
            public class FooService1 : IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService2")]
            public class FooService2 : IFooService
            {
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionIfServiceTypeIsNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RemoveInstance(null, "FooService1"));
            }

            [TestMethod]
            public void RemovesTheInstance()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };

                var instance1 = serviceLocator.ResolveType(typeof(IFooService), "FooService1");
                serviceLocator.RemoveInstance(typeof(IFooService), "FooService1");
                var instance2 = serviceLocator.ResolveType(typeof(IFooService), "FooService1");

                Assert.AreNotEqual(instance1, instance2);
            }
        }

        [TestClass]
        public class TheRemoveAllInstancesMethods
        {
            public interface IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService1")]
            public class FooService1 : IFooService
            {
            }

            [ServiceLocatorRegistration(typeof(IFooService), ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, "FooService2")]
            public class FooService2 : IFooService
            {
            }

            public interface IFoo2Service
            {
            }

            public class Foo2Service : IFoo2Service
            {
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionIfServiceTypeIsNull()
            {
                var serviceLocator = new ServiceLocator();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => serviceLocator.RemoveAllInstances((Type)null));
            }


            [TestMethod]
            public void RemovesAllInstanceOfAType()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };

                var instance1 = serviceLocator.ResolveType(typeof(IFooService), "FooService1");
                var instance2 = serviceLocator.ResolveType(typeof(IFooService), "FooService2");

                serviceLocator.RemoveAllInstances(typeof(IFooService));

                Assert.AreNotEqual(instance1, serviceLocator.ResolveType(typeof(IFooService), "FooService1"));
                Assert.AreNotEqual(instance2, serviceLocator.ResolveType(typeof(IFooService), "FooService2"));
            }

            [TestMethod]
            public void RemovesOnlyTheTaggedInstances()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };
                serviceLocator.RegisterType(typeof(IFoo2Service), typeof(Foo2Service), "FooService2");

                var instance1 = serviceLocator.ResolveType(typeof(IFooService), "FooService1");
                var instance2 = serviceLocator.ResolveType(typeof(IFooService), "FooService2");
                var instance3 = serviceLocator.ResolveType(typeof(IFoo2Service), "FooService2");

                serviceLocator.RemoveAllInstances("FooService2");

                Assert.AreEqual(instance1, serviceLocator.ResolveType(typeof(IFooService), "FooService1"));
                Assert.AreNotEqual(instance2, serviceLocator.ResolveType(typeof(IFooService), "FooService2"));
                Assert.AreNotEqual(instance3, serviceLocator.ResolveType(typeof(IFoo2Service), "FooService2"));
            }

            [TestMethod]
            public void RemovesAllInstances()
            {
                var serviceLocator = new ServiceLocator { AutoRegisterTypesViaAttributes = true };
                serviceLocator.RegisterType(typeof(IFoo2Service), typeof(Foo2Service), "FooService2");

                var instance1 = serviceLocator.ResolveType(typeof(IFooService), "FooService1");
                var instance2 = serviceLocator.ResolveType(typeof(IFooService), "FooService2");
                var instance3 = serviceLocator.ResolveType(typeof(IFoo2Service), "FooService2");

                serviceLocator.RemoveAllInstances();

                Assert.AreNotEqual(instance1, serviceLocator.ResolveType(typeof(IFooService), "FooService1"));
                Assert.AreNotEqual(instance2, serviceLocator.ResolveType(typeof(IFooService), "FooService2"));
                Assert.AreNotEqual(instance3, serviceLocator.ResolveType(typeof(IFoo2Service), "FooService2"));
            }
        }
    }
}
