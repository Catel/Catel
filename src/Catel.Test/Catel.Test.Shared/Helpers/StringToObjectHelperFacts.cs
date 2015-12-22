// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringToObjectHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;

    using NUnit.Framework;

    public class StringToObjectHelperFacts
    {
        public enum TestEnum
        {
            Value1,

            Value2,

            Value3
        }

        [TestFixture]
        public class TheGetValueAsTimeSpanMethod
        {
            [TestCase]
            public void ReturnsRightValueForValidValue()
            {
                var timespanValue = StringToObjectHelper.ToTimeSpan("1.23:12:21");
                var expectedTimespan = new TimeSpan(1, 23, 12, 21);

                Assert.AreEqual(expectedTimespan, timespanValue);
            }
        }

        [TestFixture]
        public class TheToRightTypeMethod
        {
            [TestCase]
            public void SupportsEnum()
            {
                var enumValue = StringToObjectHelper.ToRightType(typeof(TestEnum), "Value3");

                Assert.AreEqual(TestEnum.Value3, enumValue);
            }
        }

        [TestFixture]
        public class TheGetValueAsEnumMethod
        {
            [TestCase]
            public void ReturnsDefaultValueForInvalidValue()
            {
                var enumValue = StringToObjectHelper.ToEnum("bla", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value3, enumValue);
            }

            [TestCase]
            public void ReturnsRightValueForValidValue()
            {
                var enumValue = StringToObjectHelper.ToEnum("Value2", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value2, enumValue);
            }
        }
    }
}