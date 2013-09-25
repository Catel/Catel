// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumFacts.cs" company="Catel development team">
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

    public class EnumFacts
    {
        [Flags]
        private enum Enum1
        {
            MyValue = 1,

            MySecondValue = 2
        }
        
        private enum Enum2
        {
            MyValue = 0
        }

        [TestClass]
        public class TheClearFlagsMethod
        {
            [TestMethod]
            public void ReturnsEnumWithClearedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue;

                var clearedFlags = Enum<Enum1>.Flags.ClearFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, clearedFlags);
            }

            [TestMethod]
            public void ReturnsEnumWithClearedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var clearedFlags = Enum<Enum1>.Flags.ClearFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, clearedFlags);
            }
        }

        [TestClass]
        public class TheConvertFromOtherEnumValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNonEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(new object()));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForWrongEnumValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MySecondValue));
            }

            [TestMethod]
            public void ReturnsConvertedEnumValue()
            {
                Assert.AreEqual(Enum2.MyValue, Enum<Enum2>.ConvertFromOtherEnumValue(Enum1.MyValue));
            }
        }

        [TestClass]
        public class TheGetNameMethod
        {
            [TestMethod]
            public void ReturnsNameForIntEnumValue()
            {
                var name = Enum<Enum1>.GetName(2);

                Assert.AreEqual("MySecondValue", name);
            }
        }
        
        [TestClass]
        public class TheGetNamesMethod
        {
            [TestMethod]
            public void ReturnsNamesForEnum()
            {
                var names = Enum<Enum1>.GetNames();

                Assert.AreEqual(2, names.Length);
                Assert.AreEqual("MyValue", names[0]);
                Assert.AreEqual("MySecondValue", names[1]);
            }
        }
        
        [TestClass]
        public class TheGetValuesMethod
        {
            [TestMethod]
            public void ReturnsValuesForEnum()
            {
                var values = Enum<Enum1>.GetValues();

                Assert.AreEqual(2, values.Count);
                Assert.AreEqual(Enum1.MyValue, values[0]);
                Assert.AreEqual(Enum1.MySecondValue, values[1]);
            }
        }
       
        [TestClass]
        public class TheIsFlagSetMethod
        {
            [TestMethod]
            public void ReturnsFalsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;

                Assert.IsFalse(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue));
            }

            [TestMethod]
            public void ReturnsTrueForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;

                Assert.IsTrue(Enum<Enum1>.Flags.IsFlagSet(flags, Enum1.MySecondValue));
            }
        }

        [TestClass]
        public class TheSetFlagMethod
        {
            [TestMethod]
            public void ReturnsUpdatedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SetFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }

            [TestMethod]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SetFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }
        }

        [TestClass]
        public class TheSwapFlagMethod
        {
            [TestMethod]
            public void ReturnsUpdatedFlagsForEnumWithoutFlagSet()
            {
                var flags = Enum1.MyValue;
                var expectedFlags = Enum1.MyValue | Enum1.MySecondValue;

                var actualFlags = Enum<Enum1>.Flags.SwapFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }

            [TestMethod]
            public void ReturnsUpdatedFlagsForEnumWithFlagSet()
            {
                var flags = Enum1.MyValue | Enum1.MySecondValue;
                var expectedFlags = Enum1.MyValue;

                var actualFlags = Enum<Enum1>.Flags.SwapFlag(flags, Enum1.MySecondValue);
                Assert.AreEqual(expectedFlags, actualFlags);
            }
        }

        [TestClass]
        public class TheToListMethod
        {
            [TestMethod]
            public void ReturnsListForEnum()
            {
                var list = Enum<Enum1>.ToList();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(Enum1.MyValue, list[0]);
                Assert.AreEqual(Enum1.MySecondValue, list[1]);
            }
        }

        [TestClass]
        public class TheParseMethod
        {
            [TestMethod]
            public void ThrowsExceptionForInvalidValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => Enum<Enum1>.Parse("hi there"));
            }

            [TestMethod]
            public void ReturnsTrueForValidValue()
            {
                Assert.AreEqual(Enum1.MySecondValue, Enum<Enum1>.Parse("MySecondValue"));
            }
        }

        [TestClass]
        public class TheTryParseMethod
        {
            [TestMethod]
            public void ReturnsFalseForInvalidValue()
            {
                Enum1 result;

                Assert.IsFalse(Enum<Enum1>.TryParse("hi there", out result));
            }

            [TestMethod]
            public void ReturnsTrueForValidValue()
            {
                Enum1 result;

                Assert.IsTrue(Enum<Enum1>.TryParse("MySecondValue", out result));
                Assert.AreEqual(Enum1.MySecondValue, result);
            }
        }
    }
}