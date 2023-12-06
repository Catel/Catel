namespace Catel.Tests.MVVM.Converters
{
    using System;
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for ShortDateFormattingConverterTest and is intended
    ///to contain all ShortDateFormattingConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class ShortDateFormattingConverterTest
    {
        [TestCase]
        public void Convert_Null_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.Convert(null, typeof(string), null, new CultureInfo("nl-NL")), Is.EqualTo(string.Empty));
        }

        [TestCase]
        public void Convert_Null_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.Convert(null, typeof(string), "d", new CultureInfo("nl-NL")), Is.EqualTo(string.Empty));
        }

        [TestCase]
        public void Convert_Date_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.Convert(new DateTime(2010, 12, 16), typeof(string), null, new CultureInfo("nl-NL")), Is.EqualTo("16-12-2010"));
        }

        [TestCase]
        public void Convert_Date_Formatting()
        {
            var converter = new ShortDateFormattingConverter();

            var value = converter.Convert(new DateTime(2010, 12, 15), typeof(string), "G", new CultureInfo("nl-NL"));

            var firstExpectedValue = "15-12-2010 00:00:00";
            var secondExpectedValue = "15-12-2010 0:00:00";

            if (!string.Equals(value, firstExpectedValue) && !string.Equals(value, secondExpectedValue))
            {
                Assert.Fail("None of the expected values were returned");
            }
        }

        [TestCase]
        public void ConvertBack_Null_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.ConvertBack(null, typeof(DateTime), null, new CultureInfo("nl-NL")), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_Null_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.ConvertBack(null, typeof(DateTime), "G", new CultureInfo("nl-NL")), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_Date_NoFormatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.ConvertBack("16-12-2010", typeof(DateTime), null, new CultureInfo("nl-NL")), Is.EqualTo(new DateTime(2010, 12, 16)));
        }

        [TestCase]
        public void ConvertBack_Date_Formatting()
        {
            var converter = new ShortDateFormattingConverter();
            Assert.That(converter.ConvertBack("16-12-2010 0:00:00", typeof(DateTime), "G", new CultureInfo("nl-NL")), Is.EqualTo(new DateTime(2010, 12, 16)));
        }
    }
}
