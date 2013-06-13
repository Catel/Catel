// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringToObjectHelperFacts.cs" company="Catel development team">
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

    public class StringToObjectHelperFacts
    {
        // TODO: Write unit tests

        [TestClass]
        public class TheGetValueAsEnumMethod
        {
            public enum TestEnum
            {
                Value1,

                Value2,

                Value3
            }

            [TestMethod]
            public void ReturnsDefaultValueForInvalidValue()
            {
                var enumValue = StringToObjectHelper.ToEnum("bla", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value3, enumValue);
            }

            [TestMethod]
            public void ReturnsRightValueForValidValue()
            {
                var enumValue = StringToObjectHelper.ToEnum("Value2", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value2, enumValue);
            }
        }
    }
}