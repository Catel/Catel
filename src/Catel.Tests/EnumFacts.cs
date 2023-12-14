namespace Catel.Tests
{
    using System;

    using NUnit.Framework;

    public class EnumFacts
    {
        [Flags]
        public enum Enum1
        {
            None = 0,

            MyValue = 1,

            MySecondValue = 2,

            MyThirdValue = 4
        }

        private enum Enum2
        {
            MyValue = 0
        }

        [TestFixture]
        public class TheGetValuesFromFlagsMethod
        {
            [TestCase(Enum1.MySecondValue | Enum1.MyThirdValue, new[] { Enum1.MySecondValue, Enum1.MyThirdValue })]
            [TestCase(Enum1.MyThirdValue, new[] { Enum1.MyThirdValue })]
            public void ReturnsCorrectFlags(Enum1 flags, Enum1[] expectedValues)
            {
                var actualValues = Enum<Enum1>.Flags.GetValues(flags);

                Assert.That(actualValues, Is.EqualTo(expectedValues));
            }
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
                Assert.That(clearedFlags, Is.EqualTo(expectedFlags));
            }

            [TestCase]
            public void ReturnsEnumWithClearedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var clearedFlags = Enum<Enum1>.Flags.ClearFlag(flags, Enum1.MySecondValue);
                Assert.That(clearedFlags, Is.EqualTo(expectedFlags));
            }
        }

        [TestFixture]
        public class TheConvertFromOtherEnumValueMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullEnumValue()
            {
                Assert.Throws<ArgumentNullException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNonEnumValue()
            {
                Assert.Throws<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(new object()));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForWrongEnumValue()
            {
                Assert.Throws<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MySecondValue));
            }

            [TestCase]
            public void ReturnsConvertedEnumValue()
            {
                Assert.That(Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MyValue), Is.EqualTo(Enum2.MyValue));
            }
        }

        [TestFixture]
        public class TheGetNameMethod
        {
            [TestCase]
            public void ReturnsNameForIntEnumValue()
            {
                var name = Enum<Enum1>.GetName(2);

                Assert.That(name, Is.EqualTo("MySecondValue"));
            }
        }

        [TestFixture]
        public class TheGetNamesMethod
        {
            [TestCase]
            public void ReturnsNamesForEnum()
            {
                var names = Enum<Enum1>.GetNames();

                Assert.That(names.Length, Is.EqualTo(4));
                Assert.That(names[0], Is.EqualTo("None"));
                Assert.That(names[1], Is.EqualTo("MyValue"));
                Assert.That(names[2], Is.EqualTo("MySecondValue"));
                Assert.That(names[3], Is.EqualTo("MyThirdValue"));
            }
        }

        [TestFixture]
        public class TheGetValuesMethod
        {
            [TestCase]
            public void ReturnsValuesForEnum()
            {
                var values = Enum<Enum1>.GetValues();

                Assert.That(values.Count, Is.EqualTo(4));
                Assert.That(values[0], Is.EqualTo(Enum1.None));
                Assert.That(values[1], Is.EqualTo(Enum1.MyValue));
                Assert.That(values[2], Is.EqualTo(Enum1.MySecondValue));
                Assert.That(values[3], Is.EqualTo(Enum1.MyThirdValue));
            }
        }

        [TestFixture]
        public class TheIsFlagSetMethod
        {
            [TestCase]
            public void ReturnsFalsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;

                Assert.That(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;

                Assert.That(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue), Is.True);
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
                Assert.That(actualFlags, Is.EqualTo(expectedFlags));
            }

            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SetFlag(flags, Enum1.MySecondValue);
                Assert.That(actualFlags, Is.EqualTo(expectedFlags));
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
                Assert.That(actualFlags, Is.EqualTo(expectedFlags));
            }

            [TestCase]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var actualFlags = Enum<Enum1>.Flags.SwapFlag(flags, Enum1.MySecondValue);
                Assert.That(actualFlags, Is.EqualTo(expectedFlags));
            }
        }

        [TestFixture]
        public class TheToListMethod
        {
            [TestCase]
            public void ReturnsListForEnum()
            {
                var list = Enum<Enum1>.ToList();

                Assert.That(list.Count, Is.EqualTo(4));
                Assert.That(list[0], Is.EqualTo(Enum1.None));
                Assert.That(list[1], Is.EqualTo(Enum1.MyValue));
                Assert.That(list[2], Is.EqualTo(Enum1.MySecondValue));
                Assert.That(list[3], Is.EqualTo(Enum1.MyThirdValue));
            }
        }

        [TestFixture]
        public class TheParseMethod
        {
            [TestCase]
            public void ThrowsExceptionForInvalidValue()
            {
                Assert.Throws<ArgumentException>(() => Enum<Enum1>.Parse("hi there"));
            }

            [TestCase]
            public void ReturnsTrueForValidValue()
            {
                Assert.That(Enum<Enum1>.Parse("MySecondValue"), Is.EqualTo(Enum1.MySecondValue));
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

                Assert.That(parseResult, Is.True);
                Assert.That(result, Is.EqualTo(expectedResult.Value));
            }
        }
    }
}