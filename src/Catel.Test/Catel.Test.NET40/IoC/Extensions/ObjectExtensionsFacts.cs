// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.IoC
{
    using System;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ObjectExtensionsFacts
    {
        [TestClass]
        public class TheGetTypeFactoryMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullObject()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ObjectExtensions.GetTypeFactory(null));
            }

            [TestMethod]
            public void ReturnsDefaultTypeFactoryForObjectNotCreatedWithTypeFactory()
            {
                var obj = new object();
                var defaultTypeFactory = ServiceLocator.Default.ResolveType<ITypeFactory>();
                var usedTypeFactory = obj.GetTypeFactory();

                Assert.IsTrue(ReferenceEquals(defaultTypeFactory, usedTypeFactory));
            }

            [TestMethod]
            public void ReturnsTypeFactoryUsedToCreateObject()
            {
                var serviceLocator = new ServiceLocator();
                var typeFactory = serviceLocator.ResolveType<ITypeFactory>();
                var obj = typeFactory.CreateInstance<object>();

                var usedTypeFactory = obj.GetTypeFactory();

                Assert.IsTrue(ReferenceEquals(typeFactory, usedTypeFactory));
            }
        }
    }
}