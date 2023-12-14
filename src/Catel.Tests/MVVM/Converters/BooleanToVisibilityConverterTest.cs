namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;
    using System.Windows;

    [TestFixture]
    public class BooleanToVisibilityConverterTest
    {
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(null, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_NonBoolean()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert("string", typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(true, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_True_ParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(true, typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_True_ParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(true, typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(false, typeof(Visibility), null, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void Convert_FalseParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(false, typeof(Visibility), true, (CultureInfo)null), Is.EqualTo(Visibility.Visible));
        }

        [TestCase]
        public void Convert_FalseParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.Convert(false, typeof(Visibility), false, (CultureInfo)null), Is.EqualTo(Visibility.Collapsed));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(null, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack_Visible()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Visible, typeof(bool), null, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void ConvertBack_VisibleParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Visible, typeof(bool), true, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack_VisibleParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Visible, typeof(bool), false, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void ConvertBack_Collapsed()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack_CollapsedParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Collapsed, typeof(bool), true, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void ConvertBack_CollapsedParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.That(converter.ConvertBack(Visibility.Collapsed, typeof(bool), false, (CultureInfo)null), Is.EqualTo(false));
        }
    }
}
