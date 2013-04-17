namespace Catel.Test.IoC
{
    using System;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class WindsorHelperTest
    {
        #region IsValidContainer
        [TestMethod]
        public void IsValidContainer_Null()
        {
            var helper = new WindsorHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.IsValidContainer(null));
        }

        [TestMethod]
        public void IsValidContainer_InvalidContainer()
        {
            var helper = new WindsorHelper();

            var container = new object();
            Assert.IsFalse(helper.IsValidContainer(container));
        }

        [TestMethod]
        public void IsValidContainer_ValidContainer()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            Assert.IsTrue(helper.IsValidContainer(container));
        }
        #endregion

        #region GetRegistrationInfo
        [TestMethod]
        public void GetRegistrationInfo_ContainerNull()
        {
            var helper = new WindsorHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_InterfaceTypeNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(container, null));
        }

        [TestMethod]
        public void GetRegistrationInfo_InvalidContainer()
        {
            var helper = new WindsorHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithoutTypeRegistered()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithSingletonTypeRegistered()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            container.Register(new IRegistration[] { new ComponentRegistration<ITestInterface>().ImplementedBy<TestClass1>().LifestyleSingleton() });

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            // Note that we cannot get the lifestyle from windsor
            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Transient, registrationInfo.RegistrationType);
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithTransientTypeRegistered()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            container.Register(new IRegistration[] { new ComponentRegistration<ITestInterface>().ImplementedBy<TestClass1>().LifestyleTransient() });

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Transient, registrationInfo.RegistrationType);
        }
        #endregion

        #region RegisterType
        [TestMethod]
        public void RegisterType_ContainerNull()
        {
            var helper = new WindsorHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(null, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InterfaceTypeNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, null, typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_ImplementingTypeNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, typeof(ITestInterface), null, RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InvalidContainer()
        {
            var helper = new WindsorHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_Valid()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();

            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));

            helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton);
            Assert.IsTrue(container.Resolve<ITestInterface>() != null);
        }
        #endregion

        #region RegisterInstance
        [TestMethod]
        public void RegisterInstance_ContainerNull()
        {
            var helper = new WindsorHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(null, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InterfaceTypeNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, null, new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InstanceNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, typeof(ITestInterface), null));
        }

        [TestMethod]
        public void RegisterInstance_InvalidContainer()
        {
            var helper = new WindsorHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterInstance(container, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_Valid()
        {
            var helper = new WindsorHelper();
            var instance = new TestClass1() { Name = "test" };

            var container = new WindsorContainer();
            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));

            helper.RegisterInstance(container, typeof(ITestInterface), instance);
            Assert.IsTrue(container.Resolve<ITestInterface>() != null);
            Assert.AreEqual(instance, container.Resolve<ITestInterface>());
        }
        #endregion

        #region ResolveType
        [TestMethod]
        public void ResolveType_ContainerNull()
        {
            var helper = new WindsorHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_InterfaceTypeNull()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(container, null));
        }

        [TestMethod]
        public void ResolveType_InvalidContainer()
        {
            var helper = new WindsorHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_ContainerWithoutTypeRegistered()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_Singleton()
        {
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            container.Register(new IRegistration[] { new ComponentRegistration<ITestInterface>().ImplementedBy<TestClass1>().LifestyleSingleton() });

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
            var helper = new WindsorHelper();

            var container = new WindsorContainer();
            container.Register(new IRegistration[] { new ComponentRegistration<ITestInterface>().ImplementedBy<TestClass1>().LifestyleTransient() });

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
