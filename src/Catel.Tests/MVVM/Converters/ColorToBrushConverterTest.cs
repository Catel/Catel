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
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.Convert(null, typeof(Brush), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Black).ToString(), converter.Convert(Colors.Black, typeof(Brush), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void Convert_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Green).ToString(), converter.Convert(Colors.Green, typeof(Brush), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(null, typeof(Color), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Black), typeof(Color), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Green.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Green), typeof(Color), null, (CultureInfo)null).ToString());
        }
    }
}
