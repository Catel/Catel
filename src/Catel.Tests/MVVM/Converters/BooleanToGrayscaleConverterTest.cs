namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanToGrayscaleConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.That(converter.Convert(null, typeof(double), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void Convert_NonBooleanValue()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.That(converter.Convert("string", typeof(double), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.That(converter.Convert(true, typeof(double), null, (CultureInfo)null), Is.EqualTo(1d));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.That(converter.Convert(false, typeof(double), null, (CultureInfo)null), Is.EqualTo(0d));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.That(converter.ConvertBack(null, typeof(object), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }
        #endregion
    }
}