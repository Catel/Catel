namespace Catel.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class ObjectToStringHelperFacts
    {
        [TestFixture]
        public class TheToStringMethod
        {
            [TestCase]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToString(null), Is.EqualTo("null"));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToString(DBNull.Value), Is.EqualTo("dbnull"));
            }

            [TestCase]
            public void ReturnsValueForInt()
            {
                Assert.That(ObjectToStringHelper.ToString(42), Is.EqualTo("42"));
            }

            [TestCase]
            public void ReturnsInvariantValueForDateTime()
            {
                var input = new DateTime(1984, 08, 01, 9, 42, 00);
                var output = ObjectToStringHelper.ToString(input);

                Assert.That(output, Is.EqualTo("08/01/1984 09:42:00"));
            }
        }

        [TestFixture]
        public class TheToTypeStringMethod
        {
            [TestCase]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToTypeString(null), Is.EqualTo("null"));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToTypeString(DBNull.Value), Is.EqualTo("DBNull"));
            }

            [TestCase]
            public void ReturnsTypeNameForInt()
            {
                Assert.That(ObjectToStringHelper.ToTypeString(42), Is.EqualTo("Int32"));
            }
        }

        [TestFixture]
        public class TheToFullTypeStringMethod
        {
            [TestCase]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToFullTypeString(null), Is.EqualTo("null"));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.That(ObjectToStringHelper.ToFullTypeString(DBNull.Value), Is.EqualTo("System.DBNull"));
            }

            [TestCase]
            public void ReturnsTypeNameForInt()
            {
                Assert.That(ObjectToStringHelper.ToFullTypeString(42), Is.EqualTo("System.Int32"));
            }
        }
    }
}
