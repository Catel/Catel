namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;
    using System.Windows;

    /// <summary>
    ///This is a test class for ReferenceToCollapsingVisibilityConverterTest and is intended
    ///to contain all ReferenceToCollapsingVisibilityConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class ReferenceToCollapsingVisibilityConverterTest
    {
        [TestCase]
        public void Convert_Null()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(null, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_Null_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(null, typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_Null_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(null, typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_ValueType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(1, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_ValueType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(1, typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_ValueType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(1, typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_ReferenceType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(new object(), typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_ReferenceType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(new object(), typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_ReferenceType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(new object(), typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(null, typeof(object), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }
    }
}
