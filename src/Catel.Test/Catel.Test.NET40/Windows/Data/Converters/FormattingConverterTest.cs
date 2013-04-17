// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormattingConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows.Data.Converters
{
    using System;
    using System.Globalization;
    using Catel.Windows.Data.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class FormattingConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null_NoFormatting()
        {
            var converter = new FormattingConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Null_Formatting()
        {
            var converter = new FormattingConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), "d", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Integer_NoFormatting()
        {
            var converter = new FormattingConverter();
            Assert.AreEqual("5", converter.Convert(5, typeof (string), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Integer_Formatting()
        {
            var converter = new FormattingConverter();
            Assert.AreEqual("5", converter.Convert(5, typeof (string), "d", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Date_NoFormatting()
        {
            var converter = new FormattingConverter();

#if NETFX_CORE
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof (string), null, "nl-NL");
#else
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof(string), null, new CultureInfo("nl-NL"));
#endif

            var firstExpectedValue = "15-12-2010 00:00:00";
            var secondExpectedValue = "15-12-2010 0:00:00";

            if (!string.Equals(value, firstExpectedValue) && !string.Equals(value, secondExpectedValue))
            {
                Assert.Fail("None of the expected values were returned");
            }
        }

        [TestMethod]
        public void Convert_Date_Formatting()
        {
            var converter = new FormattingConverter();

#if NETFX_CORE
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof (string), "d", "nl-NL");
#else
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof(string), "d", new CultureInfo("nl-NL"));
#endif

            var firstExpectedValue = "15-12-2010";
            var secondExpectedValue = "15-12-2010";

            if (!string.Equals(value, firstExpectedValue) && !string.Equals(value, secondExpectedValue))
            {
                Assert.Fail("None of the expected values were returned");
            }
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new FormattingConverter();
            Assert.AreEqual(ConverterHelper.DoNothingBindingValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}