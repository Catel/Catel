namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;
    using System.Windows.Media;

    [TestFixture]
    public class ColorToBrushConverterTest
    {
        [TestCase]
        public void Convert_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.Convert(null, typeof(Brush), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void Convert_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.Convert(Colors.Black, typeof(Brush), null, (CultureInfo)null).ToString(), Is.EqualTo(new SolidColorBrush(Colors.Black).ToString()));
        }

        [TestCase]
        public void Convert_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.Convert(Colors.Green, typeof(Brush), null, (CultureInfo)null).ToString(), Is.EqualTo(new SolidColorBrush(Colors.Green).ToString()));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.ConvertBack(null, typeof(Color), null, (CultureInfo)null).ToString(), Is.EqualTo(Colors.Black.ToString()));
        }

        [TestCase]
        public void ConvertBack_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.ConvertBack(new SolidColorBrush(Colors.Black), typeof(Color), null, (CultureInfo)null).ToString(), Is.EqualTo(Colors.Black.ToString()));
        }

        [TestCase]
        public void ConvertBack_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.That(converter.ConvertBack(new SolidColorBrush(Colors.Green), typeof(Color), null, (CultureInfo)null).ToString(), Is.EqualTo(Colors.Green.ToString()));
        }
    }
}
