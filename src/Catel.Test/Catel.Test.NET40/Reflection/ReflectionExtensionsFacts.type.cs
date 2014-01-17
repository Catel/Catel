// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensionsFacts.type.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Reflection
{
    using System;
    using System.Collections.Generic;
    using Catel.Reflection;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public partial class ReflectionExtensionsFacts
    {
        [TestClass]
        public class TheIsInstanceOfTypeExMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ReflectionExtensions.IsInstanceOfTypeEx(null, new object()));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullObjectToCheck()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ReflectionExtensions.IsInstanceOfTypeEx(typeof(object), null));
            }

            [TestMethod]
            public void ReturnsTrueForEqualReferenceType()
            {
                var type = typeof (InvalidOperationException);
                var instance = new InvalidOperationException();

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestMethod]
            public void ReturnsTrueForInheritingReferenceType()
            {
                var type = typeof(Exception);
                var instance = new InvalidOperationException();

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));              
            }

            [TestMethod]
            public void ReturnsFalseForNonInheritingReferenceType()
            {
                var type = typeof(Exception);
                var instance = new EventArgs();

                Assert.IsFalse(type.IsInstanceOfTypeEx(instance));
            }

            [TestMethod]
            public void ReturnsTrueForEqualValueType()
            {
                var type = typeof(int);
                var instance = 32;

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestMethod]
            public void ReturnsTrueForSpecialValueTypes()
            {
                var type = typeof(Int64);
                var instance = 32;

                Assert.IsTrue(type.IsInstanceOfTypeEx(instance));
            }

            [TestMethod]
            public void ReturnsFalseForNonInheritingValueType()
            {
                var type = typeof(bool);
                var instance = 32;

                Assert.IsFalse(type.IsInstanceOfTypeEx(instance));
            }            
        }
    }
}