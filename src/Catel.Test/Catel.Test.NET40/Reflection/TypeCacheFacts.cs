// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeCacheFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Reflection
{
    using System;

    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class TypeCacheFacts
    {
        [TestClass]
        public class TheGetTypeMethod
        {
            [TestMethod]
            public void ReturnsTypeFromMsCorlib()
            {
                var type = TypeCache.GetType("System.String");

                Assert.AreEqual(typeof(string), type);
            }

            [TestMethod]
            public void ReturnsTypeFromSystem()
            {
                var type = TypeCache.GetType("System.Uri");

                Assert.AreEqual(typeof(Uri), type);
            }

            [TestMethod]
            public void ReturnsTypeFromSystemCore()
            {
                var type = TypeCache.GetType("System.Lazy`1");

                Assert.AreEqual(typeof(Lazy<>), type);
            }
        }
    }
}