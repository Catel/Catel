// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NinjectHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC
{
    using System;
    using System.Linq;

    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    using Ninject;

    [TestClass]
    public class NinjectHelperTest
    {
        #region IsValidContainer
        [TestMethod]
        public void IsValidContainer_Null()
        {
            var helper = new NinjectHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.IsValidContainer(null));
        }

        [TestMethod]
        public void IsValidContainer_InvalidContainer()
        {
            var helper = new NinjectHelper();

            var container = new object();
            Assert.IsFalse(helper.IsValidContainer(container));
        }

        [TestMethod]
        public void IsValidContainer_ValidContainer()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            Assert.IsTrue(helper.IsValidContainer(container));
        }
        #endregion

        #region GetRegistrationInfo
        [TestMethod]
        public void GetRegistrationInfo_ContainerNull()
        {
            var helper = new NinjectHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_InterfaceTypeNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(container, null));
        }

        [TestMethod]
        public void GetRegistrationInfo_InvalidContainer()
        {
            var helper = new NinjectHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithoutTypeRegistered()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithSingletonTypeRegistered()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            container.Bind<ITestInterface>().To<TestClass1>().InSingletonScope();

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Singleton, registrationInfo.RegistrationType);
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithTransientTypeRegistered()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            container.Bind<ITestInterface>().To<TestClass1>().InTransientScope();

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Transient, registrationInfo.RegistrationType);
        }
        #endregion

        #region RegisterType
        [TestMethod]
        public void RegisterType_ContainerNull()
        {
            var helper = new NinjectHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(null, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InterfaceTypeNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, null, typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_ImplementingTypeNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, typeof(ITestInterface), null, RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InvalidContainer()
        {
            var helper = new NinjectHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_Valid()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();

            Assert.IsFalse(GetRegistrationInfo(container, (typeof(ITestInterface))));

            helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton);
            Assert.IsTrue(GetRegistrationInfo(container, typeof(ITestInterface)));
        }
        #endregion

        #region RegisterInstance
        [TestMethod]
        public void RegisterInstance_ContainerNull()
        {
            var helper = new NinjectHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(null, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InterfaceTypeNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, null, new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InstanceNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, typeof(ITestInterface), null));
        }

        [TestMethod]
        public void RegisterInstance_InvalidContainer()
        {
            var helper = new NinjectHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterInstance(container, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_Valid()
        {
            var helper = new NinjectHelper();
            var instance = new TestClass1() { Name = "test" };

            var container = new StandardKernel();
            Assert.IsFalse(GetRegistrationInfo(container, typeof(ITestInterface)));

            helper.RegisterInstance(container, typeof(ITestInterface), instance);
            Assert.IsTrue(GetRegistrationInfo(container, typeof(ITestInterface)));
            Assert.AreEqual(instance, container.Get<ITestInterface>());
        }
        #endregion

        #region ResolveType
        [TestMethod]
        public void ResolveType_ContainerNull()
        {
            var helper = new NinjectHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_InterfaceTypeNull()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(container, null));
        }

        [TestMethod]
        public void ResolveType_InvalidContainer()
        {
            var helper = new NinjectHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_ContainerWithoutTypeRegistered()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_ContainerWithTypeRegistered()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            container.Bind(typeof(ITestInterface)).To<TestClass1>();
            var resolvedInstance = helper.ResolveType(container, typeof(ITestInterface));
            Assert.IsNotNull(resolvedInstance);
            Assert.IsInstanceOfType(resolvedInstance, typeof(TestClass1));
        }

        [TestMethod]
        public void ResolveType_Singleton()
        {
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            container.Bind(typeof(ITestInterface)).To<TestClass1>().InSingletonScope();

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
            var helper = new NinjectHelper();

            var container = new StandardKernel();
            container.Bind(typeof(ITestInterface)).To<TestClass1>().InTransientScope();

            var resolvedInstance1 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance1);
            Assert.IsInstanceOfType(resolvedInstance1, typeof(TestClass1));

            var resolvedInstance2 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance2);
            Assert.IsInstanceOfType(resolvedInstance2, typeof(TestClass1));

            Assert.IsFalse(ReferenceEquals(resolvedInstance1, resolvedInstance2));
        }
        #endregion

        private bool GetRegistrationInfo(IKernel kernel, Type interfaceType)
        {
            return kernel.GetBindings(interfaceType).Any();
        }
    }
}