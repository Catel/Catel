namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for MultiplyConverterTest and is intended
    ///to contain all MultiplyConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MultiplyConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.Convert(null, typeof(int), null, (CultureInfo)null), Is.EqualTo(0d));
        }

        [TestCase]
        public void Convert_0_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.Convert(0, typeof(int), "5", (CultureInfo)null), Is.EqualTo(0d));
        }

        [TestCase]
        public void Convert_5_Parameter0()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.Convert(5, typeof(int), "0", (CultureInfo)null), Is.EqualTo(0d));
        }

        [TestCase]
        public void Convert_2_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.Convert(2, typeof(int), "5", (CultureInfo)null), Is.EqualTo(10d));
        }

        [TestCase]
        public void ConvertBack_0_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.ConvertBack(5, typeof(int), "0", (CultureInfo)null), Is.EqualTo(double.PositiveInfinity));
        }

        [TestCase]
        public void ConvertBack_5_Parameter0()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.ConvertBack(0, typeof(int), "5", (CultureInfo)null), Is.EqualTo(0d));
        }

        [TestCase]
        public void ConvertBack_10_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.That(converter.ConvertBack(10, typeof(int), "5", (CultureInfo)null), Is.EqualTo(2d));
        }
        #endregion
    }
}