// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortDateFormattingConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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

    /// <summary>
    ///This is a test class for ShortDateFormattingConverterTest and is intended
    ///to contain all ShortDateFormattingConverterTest Unit Tests
    ///</summary>
    [TestClass]
    public class ShortDateFormattingConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), null, new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void Convert_Null_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(string.Empty, converter.Convert(null, typeof (string), "d", new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void Convert_Date_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual("16-12-2010", converter.Convert(new DateTime(2010, 12, 16), typeof (string), null, new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void Convert_Date_Formatting()
        {
            var converter = new ShortDateFormattingConverter();

#if NETFX_CORE
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof (string), "G", "nl-NL");
#else
            var value = converter.Convert(new DateTime(2010, 12, 15), typeof(string), "G", new CultureInfo("nl-NL"));
#endif

            var firstExpectedValue = "15-12-2010 00:00:00";
            var secondExpectedValue = "15-12-2010 0:00:00";

            if (!string.Equals(value, firstExpectedValue) && !string.Equals(value, secondExpectedValue))
            {
                Assert.Fail("None of the expected values were returned");
            }
        }

        [TestMethod]
        public void ConvertBack_Null_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (DateTime), null, new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void ConvertBack_Null_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (DateTime), "G", new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void ConvertBack_Date_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(new DateTime(2010, 12, 16), converter.ConvertBack("16-12-2010", typeof (DateTime), null, new CultureInfo("nl-NL")));
        }

        [TestMethod]
        public void ConvertBack_Date_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.AreEqual(new DateTime(2010, 12, 16), converter.ConvertBack("16-12-2010 0:00:00", typeof (DateTime), "G", new CultureInfo("nl-NL")));
        }
        #endregion
    }
}