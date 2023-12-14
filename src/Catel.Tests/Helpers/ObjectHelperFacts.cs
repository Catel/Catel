namespace Catel.Tests
{
    using NUnit.Framework;
    using System;

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

                Assert.That(ObjectHelper.AreEqual(obj1, obj2), Is.True);
                Assert.That(ObjectHelper.AreEqual(obj2, obj1), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.That(ObjectHelper.AreEqual(obj1, obj2), Is.False);
                Assert.That(ObjectHelper.AreEqual(obj2, obj1), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.That(ObjectHelper.AreEqual(obj1, obj2), Is.True);
                Assert.That(ObjectHelper.AreEqual(obj2, obj1), Is.True);
            }

            [TestCase]
            public void ReturnsTrueForTwoDbNullValues()
            {
                object obj1 = DBNull.Value;
                object obj2 = DBNull.Value;

                Assert.That(ObjectHelper.AreEqual(obj1, obj2), Is.True);
                Assert.That(ObjectHelper.AreEqual(obj2, obj1), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.That(ObjectHelper.AreEqual(obj1, obj2), Is.False);
                Assert.That(ObjectHelper.AreEqual(obj2, obj1), Is.False);
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

                Assert.That(ObjectHelper.AreEqualReferences(obj1, obj2), Is.True);
                Assert.That(ObjectHelper.AreEqualReferences(obj2, obj1), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForBoxedDifferentIntegers()
            {
                object obj1 = 5;
                object obj2 = 6;

                Assert.That(ObjectHelper.AreEqualReferences(obj1, obj2), Is.False);
                Assert.That(ObjectHelper.AreEqualReferences(obj2, obj1), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForTwoNullValues()
            {
                object obj1 = null;
                object obj2 = null;

                Assert.That(ObjectHelper.AreEqualReferences(obj1, obj2), Is.True);
                Assert.That(ObjectHelper.AreEqualReferences(obj2, obj1), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForOneNullValue()
            {
                object obj1 = 5;
                object obj2 = null;

                Assert.That(ObjectHelper.AreEqualReferences(obj1, obj2), Is.False);
                Assert.That(ObjectHelper.AreEqualReferences(obj2, obj1), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForDifferentReferenceTypes()
            {
                object obj1 = new { Id = "test" };
                object obj2 = new { Id = "test" };

                Assert.That(obj1, Is.EqualTo(obj2));
                Assert.That(ObjectHelper.AreEqualReferences(obj1, obj2), Is.False);
                Assert.That(ObjectHelper.AreEqualReferences(obj2, obj1), Is.False);
            }
        }
    }
}
