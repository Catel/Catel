namespace Catel.Tests
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
        public class The_ToTimeSpan_Method
        {
            [TestCase]
            public void Returns_Right_Value_For_Valid_Value()
            {
                var timespanValue = StringToObjectHelper.ToTimeSpan("1.23:12:21");
                var expectedTimespan = new TimeSpan(1, 23, 12, 21);

                Assert.AreEqual(expectedTimespan, timespanValue);
            }
        }

        [TestFixture]
        public class The_ToRightType_Method
        {
            [TestCase("42", ExpectedResult = 42)]
            [TestCase("-42", ExpectedResult = -42)]
            public int Supports_Integer(string input)
            {
                var actualValue = StringToObjectHelper.ToRightType<int>(input);
                return actualValue;
            }

            [TestCase("1", ExpectedResult = true)]
            [TestCase("true", ExpectedResult = true)]
            [TestCase("TRUE", ExpectedResult = true)]
            [TestCase("0", ExpectedResult = false)]
            [TestCase("false", ExpectedResult = false)]
            [TestCase("FALSE", ExpectedResult = false)]
            public bool Supports_Boolean(string input)
            {
                var actualValue = StringToObjectHelper.ToRightType<bool>(input);
                return actualValue;
            }

            [TestCase]
            public void Supports_Enum()
            {
                var enumValue = StringToObjectHelper.ToRightType(typeof(TestEnum), "Value3");

                Assert.AreEqual(TestEnum.Value3, enumValue);
            }

            [TestCase("0", ExpectedResult = (byte)0)]
            [TestCase("10", ExpectedResult = (byte)10)]
            [TestCase("123", ExpectedResult = (byte)123)]
            [TestCase("01", ExpectedResult = (byte)1)]
            public byte Supports_Bytes(string input)
            {
                var byteValue = StringToObjectHelper.ToRightType<byte>(input);
                return byteValue;
            }
        }

        [TestFixture]
        public class The_ToEnum_Method
        {
            [TestCase]
            public void Returns_Default_Value_For_Invalid_Value()
            {
                var enumValue = StringToObjectHelper.ToEnum("bla", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value3, enumValue);
            }

            [TestCase]
            public void Returns_Right_Value_For_Valid_Value()
            {
                var enumValue = StringToObjectHelper.ToEnum("Value2", TestEnum.Value3);

                Assert.AreEqual(TestEnum.Value2, enumValue);
            }
        }
    }
}
