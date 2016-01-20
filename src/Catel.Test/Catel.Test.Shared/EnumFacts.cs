// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;

    using NUnit.Framework;

    public class EnumFacts
    {
        [Flags]
        public enum Enum1
        {
            MyValue = 1,

            MySecondValue = 2
        }
        
        private enum Enum2
        {
            MyValue = 0
        }

        [TestFixture]
        public class TheClearFlagsMethod
        {
            [TestCase]
            public void ReturnsEnumWithClearedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue;

                var clearedFlags = Enum<Enum1>.Flags.ClearFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, clearedFlags);
            }

            [TestCase]
            public void ReturnsEnumWithClearedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var clearedFlags = Enum<Enum1>.Flags.ClearFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, clearedFlags);
            }
        }

        [TestFixture]
        public class TheConvertFromOtherEnumValueMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNonEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(new object()));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForWrongEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MySecondValue));
            }

            [TestCase]
            public void ReturnsConvertedEnumValue()
            {
                Assert.AreEqual(Enum2.MyValue, Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MyValue));
            }
        }

        [TestFixture]
        public class TheGetNameMethod
        {
            [TestCase]
            public void ReturnsNameForIntEnumValue()
            {
                var name = Enum<Enum1>.GetName(2);

                Assert.AreEqual("MySecondValue", name);
            }
        }
        
        [TestFixture]
        public class TheGetNamesMethod
        {
            [TestCase]
            public void ReturnsNamesForEnum()
            {
                var names = Enum<Enum1>.GetNames();

                Assert.AreEqual(2, names.Length);
                Assert.AreEqual("MyValue", names[0]);
                Assert.AreEqual("MySecondValue", names[1]);
            }
        }
        
        [TestFixture]
        public class TheGetValuesMethod
        {
            [TestCase]
            public void ReturnsValuesForEnum()
            {
                var values = Enum<Enum1>.GetValues();

                Assert.AreEqual(2, values.Count);
                Assert.AreEqual(Enum1.MyValue, values[0]);
                Assert.AreEqual(Enum1.MySecondValue, values[1]);
            }
        }
       
        [TestFixture]
        public class TheIsFlagSetMethod
        {
            [TestCase]
            public void ReturnsFalsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;

                Assert.IsFalse(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue));
            }

            [TestCase]
            public void ReturnsTrueForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;

                Assert.IsTrue(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue));
            }
        }

        [TestFixture]
        public class TheSetFlagMethod
        {
            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SetFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }

            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SetFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }
        }

        [TestFixture]
        public class TheSwapFlagMethod
        {
            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SwapFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }

            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var actualFlags = Enum<Enum1>.Flags.SwapFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }
        }

        [TestFixture]
        public class TheToListMethod
        {
            [TestCase]
            public void ReturnsListForEnum()
            {
                var list = Enum<Enum1>.ToList();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(Enum1.MyValue, list[0]);
                Assert.AreEqual(Enum1.MySecondValue, list[1]);
            }
        }

        [TestFixture]
        public class TheParseMethod
        {
            [TestCase]
            public void ThrowsExceptionForInvalidValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum1>.Parse("hi there"));
            }

            [TestCase]
            public void ReturnsTrueForValidValue()
            {
                Assert.AreEqual(Enum1.MySecondValue, Enum<Enum1>.Parse("MySecondValue"));
            }
        }

        [TestFixture]
        public class TheTryParseMethod
        {
            [TestCase("hi there", false, null)]
            [TestCase("hi there", true, null)]
            [TestCase("MySecondValue", false, Enum1.MySecondValue)]
            [TestCase("MySecondValue", true, Enum1.MySecondValue)]
            [TestCase("MYSECONDVALUE", false, null)]
            [TestCase("MYSECONDVALUE", true, Enum1.MySecondValue)]
            public void ReturnsCorrectValueForTryParseMethod(string input, bool ignoreCase, Enum1? expectedResult)
            {
                Enum1 result;

                var parseResult = Enum<Enum1>.TryParse(input, ignoreCase, out result);

                if (!expectedResult.HasValue && !parseResult)
                {
                    return;
                }

                Assert.IsTrue(parseResult);
                Assert.AreEqual(expectedResult.Value, result);
            }
        }
    }
}