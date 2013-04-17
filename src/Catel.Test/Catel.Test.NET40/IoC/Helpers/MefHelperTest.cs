namespace Catel.Test.IoC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class MefHelperTest
    {
        #region IsValidContainer
        [TestMethod]
        public void IsValidContainer_Null()
        {
            var helper = new MefHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.IsValidContainer(null));
        }

        [TestMethod]
        public void IsValidContainer_InvalidContainer()
        {
            var helper = new MefHelper();

            var container = new object();
            Assert.IsFalse(helper.IsValidContainer(container));
        }

        [TestMethod]
        public void IsValidContainer_ValidContainer()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            Assert.IsTrue(helper.IsValidContainer(container));
        }
        #endregion

        #region GetRegistrationInfo
        [TestMethod]
        public void GetRegistrationInfo_ContainerNull()
        {
            var helper = new MefHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_InterfaceTypeNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.GetRegistrationInfo(container, null));
        }

        [TestMethod]
        public void GetRegistrationInfo_InvalidContainer()
        {
            var helper = new MefHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithoutTypeRegistered()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            Assert.IsNull(helper.GetRegistrationInfo(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithSingletonTypeRegistered()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            container.RegisterType(typeof(ITestInterface), typeof(TestClass1));

            var registrationInfo = helper.GetRegistrationInfo(container, typeof (ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Singleton, registrationInfo.RegistrationType);
        }

        [TestMethod]
        public void GetRegistrationInfo_ContainerWithTransientTypeRegistered()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            container.RegisterType(typeof(ITestInterface), typeof(TestClass1));

            var registrationInfo = helper.GetRegistrationInfo(container, typeof(ITestInterface));

            Assert.AreEqual(typeof(ITestInterface), registrationInfo.DeclaringType);
            Assert.AreEqual(RegistrationType.Singleton, registrationInfo.RegistrationType);
        }
        #endregion

        #region RegisterType
        [TestMethod]
        public void RegisterType_ContainerNull()
        {
            var helper = new MefHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(null, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InterfaceTypeNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, null, typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_ImplementingTypeNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterType(container, typeof(ITestInterface), null, RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_InvalidContainer()
        {
            var helper = new MefHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }

        [TestMethod]
        public void RegisterType_Valid()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterType(container, typeof(ITestInterface), typeof(TestClass1), RegistrationType.Singleton));
        }
        #endregion

        #region RegisterInstance
        [TestMethod]
        public void RegisterInstance_ContainerNull()
        {
            var helper = new MefHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(null, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InterfaceTypeNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, null, new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_InstanceNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.RegisterInstance(container, typeof(ITestInterface), null));
        }

        [TestMethod]
        public void RegisterInstance_InvalidContainer()
        {
            var helper = new MefHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.RegisterInstance(container, typeof(ITestInterface), new TestClass1()));
        }

        [TestMethod]
        public void RegisterInstance_Valid()
        {
            var helper = new MefHelper();
            var instance = new TestClass1() { Name = "test" };

            var container = new CompositionContainer();
            Assert.IsFalse(container.GetRegistrationInfo(typeof(ITestInterface)));

            helper.RegisterInstance(container, typeof(ITestInterface), instance);
            Assert.IsTrue(container.GetRegistrationInfo(typeof(ITestInterface)));
            Assert.AreEqual(instance, container.ResolveType<ITestInterface>());
        }
        #endregion

        #region ResolveType
        [TestMethod]
        public void ResolveType_ContainerNull()
        {
            var helper = new MefHelper();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(null, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_InterfaceTypeNull()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => helper.ResolveType(container, null));
        }

        [TestMethod]
        public void ResolveType_InvalidContainer()
        {
            var helper = new MefHelper();

            var container = new object();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_ContainerWithoutTypeRegistered()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => helper.ResolveType(container, typeof(ITestInterface)));
        }

        [TestMethod]
        public void ResolveType_Singleton()
        {
            var helper = new MefHelper();

            var container = new CompositionContainer();
            container.RegisterType<ITestInterface, TestClass1>();

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
            var helper = new MefHelper();

            var container = new CompositionContainer();
            container.RegisterType<ITestInterface, TestClass1>();

            var resolvedInstance1 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance1);
            Assert.IsInstanceOfType(resolvedInstance1, typeof(TestClass1));

            var resolvedInstance2 = helper.ResolveType(container, typeof(ITestInterface));

            Assert.IsNotNull(resolvedInstance2);
            Assert.IsInstanceOfType(resolvedInstance2, typeof(TestClass1));

            // Note that MEF does not support transient types
            Assert.IsTrue(ReferenceEquals(resolvedInstance1, resolvedInstance2));
        }
        #endregion
    }

    /// <summary>
    /// Mef extensions to make sure that MEF is used in the right way.
    /// </summary>
    public static class MefExtensions
    {
        public static bool GetRegistrationInfo(this CompositionContainer container, Type interfaceType)
        {
            string key = AttributedModelServices.GetContractName(interfaceType);

            var exports = container.GetExports<object>(key);

            return exports.Any();
        }

        public static void RegisterType<TInterface, TImplementation>(this CompositionContainer container)
        {
            RegisterType(container, typeof(TInterface), typeof(TImplementation));
        }

        public static void RegisterType(this CompositionContainer container, Type interfaceType, Type implementingType)
        {
            string key = AttributedModelServices.GetContractName(interfaceType);

            container.ComposeExportedValue(key, Activator.CreateInstance(implementingType));
        }

        public static void RegisterInstance<TInterface>(this CompositionContainer container, TInterface interfaceType, TInterface instance)
            where TInterface : Type
        {
            string key = AttributedModelServices.GetContractName(interfaceType);

            container.ComposeExportedValue(key, instance);
        }

        public static object ResolveType<TInterface>(this CompositionContainer container)
        {
            return ResolveType(container, typeof(TInterface));
        }

        public static object ResolveType(this CompositionContainer container, Type interfaceType)
        {
            string key = AttributedModelServices.GetContractName(interfaceType);

            var exports = container.GetExports<object>(key);
            if (exports.Any())
            {
                return exports.First().Value;
            }

            return null;
        }
    }
}
