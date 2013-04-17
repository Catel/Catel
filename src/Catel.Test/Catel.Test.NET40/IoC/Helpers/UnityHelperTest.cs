namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;
    using Microsoft.Practices.Unity;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class UnityHelperTest
    {
        #region IsValidContainer
        [TestMethod]
        public void IsValidContainer_Null()
        {
            var helper = new UnityHelper();
            
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.IsValidContainer(null));
        }

        [TestMethod]
        public void IsValidContainer_InvalidContainer()
        {
            var helper = new UnityHelper();

            var container = new object();
            Assert.IsFalse(helper.IsValidContainer(container));
        }

        [TestMethod]
        public void IsValidContainer_ValidContainer()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            Assert.IsTrue(helper.IsValidContainer(container));
        }
        #endregion

        #region GetRegistrationInfo
        [TestMethod]
        public void GetRegistrationInfo_ContainerNull()
        {
            var helper = new UnityHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_InterfaceTypeNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(container, null));
        }

        [TestMethod]
        public void GetRegistrationInfo_InvalidContainer()
        {
            var helper = new UnityHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithoutTypeRegistered()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithSingletonTypeRegistered()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            container.RegisterType<ITestInterface, TestClass1>(new ContainerControlledLifetimeManager());

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Singleton, registrationInfo.RegistrationType);
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithTransientTypeRegistered()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            container.RegisterType<ITestInterface, TestClass1>(new TransientLifetimeManager());

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Transient, registrationInfo.RegistrationType);
        }
        #endregion

        #region RegisterType
        [TestMethod]
        public void RegisterType_ContainerNull()
        {
            var helper = new UnityHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(null, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InterfaceTypeNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, null, typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_ImplementingTypeNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, typeof(ITestInterface), null, RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InvalidContainer()
        {
            var helper = new UnityHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_Valid()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            Assert.IsFalse(container.IsRegistered(typeof(ITestInterface)));

            helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton);
            Assert.IsTrue(container.IsRegistered(typeof(ITestInterface)));
        }
        #endregion

        #region RegisterInstance
        [TestMethod]
        public void RegisterInstance_ContainerNull()
        {
            var helper = new UnityHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(null, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InterfaceTypeNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, null, new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InstanceNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, typeof(ITestInterface), null));
        }

        [TestMethod]
        public void RegisterInstance_InvalidContainer()
        {
            var helper = new UnityHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterInstance(container, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_Valid()
        {
            var helper = new UnityHelper();
            var instance = new TestClass1() { Name = "test" };

            var container = new UnityContainer();
            Assert.IsFalse(container.IsRegistered(typeof(ITestInterface)));

            helper.RegisterInstance(container, typeof(ITestInterface), instance);
            Assert.IsTrue(container.IsRegistered(typeof(ITestInterface)));
            Assert.AreEqual(instance, container.Resolve<ITestInterface>());
        }
        #endregion

        #region ResolveType
        [TestMethod]
        public void ResolveType_ContainerNull()
        {
            var helper = new UnityHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_InterfaceTypeNull()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(container, null));
        }

        [TestMethod]
        public void ResolveType_InvalidContainer()
        {
            var helper = new UnityHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_ContainerWithoutTypeRegistered()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_Singleton()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            container.RegisterType<ITestInterface, TestClass1>(new ContainerControlledLifetimeManager());

            var resolvedInstance1 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance1);
            Assert.IsInstanceOfType(resolvedInstance1, typeof(TestClass1));

            var resolvedInstance2 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance2);
            Assert.IsInstanceOfType(resolvedInstance2, typeof(TestClass1));

            Assert.IsTrue(ReferenceEquals(resolvedInstance1, resolvedInstance2));
        }

        [TestMethod]
        public void ResolveType_Transient()
        {
            var helper = new UnityHelper();

            var container = new UnityContainer();
            container.RegisterType<ITestInterface, TestClass1>(new TransientLifetimeManager());

            var resolvedInstance1 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance1);
            Assert.IsInstanceOfType(resolvedInstance1, typeof(TestClass1));

            var resolvedInstance2 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance2);
            Assert.IsInstanceOfType(resolvedInstance2, typeof(TestClass1));

            Assert.IsFalse(ReferenceEquals(resolvedInstance1, resolvedInstance2));
        }
        #endregion
    }
}
