// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectToStringHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
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

#if !NETFX_CORE
            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("dbnull", ObjectToStringHelper.ToString(DBNull.Value));
            }
#endif

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

#if !NETFX_CORE
            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("DBNull", ObjectToStringHelper.ToTypeString(DBNull.Value));
            }
#endif

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

#if !NETFX_CORE
            [TestCase]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("System.DBNull", ObjectToStringHelper.ToFullTypeString(DBNull.Value));
            }
#endif

            [TestCase]
            public void ReturnsTypeNameForInt()
            {
                Assert.AreEqual("System.Int32", ObjectToStringHelper.ToFullTypeString(42));
            }
        }
    }
}