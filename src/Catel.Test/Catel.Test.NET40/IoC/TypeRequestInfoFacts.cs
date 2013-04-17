// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeRequestInfoFacts.cs" company="Catel development team">
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

    public class TypeRequestInfoFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new TypeRequestInfo(null));
            }

            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var typeRequestInfo = new TypeRequestInfo(typeof(int), "mytag");

                Assert.AreEqual(typeof(int), typeRequestInfo.Type);
                Assert.AreEqual("mytag", typeRequestInfo.Tag);
            }
        }

        [TestClass]
        public class TheComparisonMethods
        {
            [TestMethod]
            public void FunctionsCorrectlyForEqualTypes()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag");
                var obj2 = new TypeRequestInfo(typeof(int), "mytag");

                Assert.IsTrue(obj1 == obj2);
                Assert.IsTrue(obj2 == obj1);

                Assert.IsFalse(obj1 != obj2);
                Assert.IsFalse(obj2 != obj1);

                Assert.IsTrue(obj1.Equals(obj2));
                Assert.IsTrue(obj2.Equals(obj1));
            }

            [TestMethod]
            public void FunctionsCorrectlyForDifferentTypes()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag");
                var obj2 = new TypeRequestInfo(typeof(double), "mytag");

                Assert.IsFalse(obj1 == obj2);
                Assert.IsFalse(obj2 == obj1);

                Assert.IsTrue(obj1 != obj2);
                Assert.IsTrue(obj2 != obj1);

                Assert.IsFalse(obj1.Equals(obj2));
                Assert.IsFalse(obj2.Equals(obj1));
            }

            [TestMethod]
            public void FunctionsCorrectlyForDifferentTags()
            {
                var obj1 = new TypeRequestInfo(typeof(int), "mytag1");
                var obj2 = new TypeRequestInfo(typeof(int), "mytag2");

                Assert.IsFalse(obj1 == obj2);
                Assert.IsFalse(obj2 == obj1);

                Assert.IsTrue(obj1 != obj2);
                Assert.IsTrue(obj2 != obj1);

                Assert.IsFalse(obj1.Equals(obj2));
                Assert.IsFalse(obj2.Equals(obj1));
            }
        }
    }
}