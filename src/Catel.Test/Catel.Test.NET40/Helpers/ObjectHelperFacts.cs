// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ObjectHelperFacts
    {
        [TestClass]
        public class TheAreEqualMethod
        {
            [TestMethod]
            public void ReturnsTrueForBoxedEqualIntegers()
            {
                object obj1 = 5;
                object obj2 = 5;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.IsFalse(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqual(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }

#if NET
            [TestMethod]
            public void ReturnsTrueForTwoDbNullValues()
            {
                object obj1 = DBNull.Value;
                object obj2 = DBNull.Value;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }
#endif

            [TestMethod]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.IsFalse(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqual(obj2, obj1));
            }
        }

        [TestClass]
        public class TheAreEqualReferencesMethod
        {
            [TestMethod]
            public void ReturnsTrueForBoxedEqualIntegers()
            {
                object obj1 = 5;
                object obj2 = 5;

                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestMethod]
            public void ReturnsTrueForDifferentReferenceTypes()
            {
                object obj1 = new { Id = "test" };
                object obj2 = new { Id = "test" };

                Assert.IsTrue(obj1.Equals(obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj2, obj1));
            }
        }        
    }
}