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
                Assert.AreEqual("null", ObjectToStringHelper.ToString(null));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("dbnull", ObjectToStringHelper.ToString(DBNull.Value));
            }

            [TestCase]
            public void ReturnsValueForInt()
            {
                Assert.AreEqual("42", ObjectToStringHelper.ToString(42));
            }

            [TestCase]
            public void ReturnsInvariantValueForDateTime()
            {
                var input = new DateTime(1984, 08, 01, 9, 42, 00);
                var output = ObjectToStringHelper.ToString(input);

                Assert.AreEqual("08/01/1984 09:42:00", output);
            }
        }

        [TestFixture]
        public class TheToTypeStringMethod
        {
            [TestCase]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.AreEqual("null", ObjectToStringHelper.ToTypeString(null));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("DBNull", ObjectToStringHelper.ToTypeString(DBNull.Value));
            }

            [TestCase]
            public void ReturnsTypeNameForInt()
            {
                Assert.AreEqual("Int32", ObjectToStringHelper.ToTypeString(42));
            }
        }

        [TestFixture]
        public class TheToFullTypeStringMethod
        {
            [TestCase]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.AreEqual("null", ObjectToStringHelper.ToFullTypeString(null));
            }

            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("System.DBNull", ObjectToStringHelper.ToFullTypeString(DBNull.Value));
            }

            [TestCase]
            public void ReturnsTypeNameForInt()
            {
                Assert.AreEqual("System.Int32", ObjectToStringHelper.ToFullTypeString(42));
            }
        }
    }
}
