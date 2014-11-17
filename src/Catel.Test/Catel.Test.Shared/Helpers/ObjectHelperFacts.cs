// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using NUnit.Framework;

#if !NETFX_CORE
    using System;
#endif

    public class ObjectHelperFacts
    {
        [TestFixture]
        public class TheAreEqualMethod
        {
            [TestCase]
            public void ReturnsTrueForBoxedEqualIntegers()
            {
                object obj1 = 5;
                object obj2 = 5;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }

            [TestCase]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.IsFalse(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqual(obj2, obj1));
            }

            [TestCase]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }

#if NET
            [TestCase]
            public void ReturnsTrueForTwoDbNullValues()
            {
                object obj1 = DBNull.Value;
                object obj2 = DBNull.Value;

                Assert.IsTrue(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqual(obj2, obj1));
            }
#endif

            [TestCase]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.IsFalse(ObjectHelper.AreEqual(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqual(obj2, obj1));
            }
        }

        [TestFixture]
        public class TheAreEqualReferencesMethod
        {
            [TestCase]
            public void ReturnsTrueForBoxedEqualIntegers()
            {
                object obj1 = 5;
                object obj2 = 5;

                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestCase]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestCase]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsTrue(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestCase]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj1, obj2));
                Assert.IsFalse(ObjectHelper.AreEqualReferences(obj2, obj1));
            }

            [TestCase]
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