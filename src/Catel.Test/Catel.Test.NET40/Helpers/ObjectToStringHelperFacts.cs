// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectToStringHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ObjectToStringHelperFacts
    {
        [TestClass]
        public class TheToStringMethod
        {
            [TestMethod]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.AreEqual("null", ObjectToStringHelper.ToString(null));
            }

#if !NETFX_CORE
            [TestMethod]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("dbnull", ObjectToStringHelper.ToString(DBNull.Value));
            }
#endif

            [TestMethod]
            public void ReturnsValueForInt()
            {
                Assert.AreEqual("42", ObjectToStringHelper.ToString(42));
            }

            [TestMethod]
            public void ReturnsInvariantValueForDateTime()
            {
                var input = new DateTime(1984, 08, 01, 9, 42, 00);
                var output = ObjectToStringHelper.ToString(input);

                Assert.AreEqual("08/01/1984 09:42:00", output);
            }
        }

        [TestClass]
        public class TheToTypeStringMethod
        {
            [TestMethod]
            public void ReturnsNullStringForNullInstance()
            {
                Assert.AreEqual("null", ObjectToStringHelper.ToTypeString(null));
            }

#if !NETFX_CORE
            [TestMethod]
            public void ReturnsDbNullStringForDbNullInstance()
            {
                Assert.AreEqual("DBNull", ObjectToStringHelper.ToTypeString(DBNull.Value));
            }
#endif

            [TestMethod]
            public void ReturnsTypeNameForInt()
            {
                Assert.AreEqual("Int32", ObjectToStringHelper.ToTypeString(42));
            }
        }
    }
}