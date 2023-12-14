namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;
    using System.Windows;

    [TestFixture]
    public class EmptyStringToVisibilityConverterTest
    {
        [TestCase]
        public void Convert_Null()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(null, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_EmptyString()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(string.Empty, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_EmptyString_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(string.Empty, typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_EmptyString_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(string.Empty, typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_String()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert("filledstring", typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_String_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert("filledstring", typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_String_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.Convert("filledstring", typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(null, typeof(object), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }
    }
}
