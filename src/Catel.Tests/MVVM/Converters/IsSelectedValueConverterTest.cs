namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for IsSelectedValueConverterTest and is intended
    ///to contain all IsSelectedValueConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class IsSelectedValueConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(null, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_1()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(1, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_1_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(1, typeof(bool), 0, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_1_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(1, typeof(bool), "0", (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_1_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(1, typeof(bool), 1, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_1_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(1, typeof(bool), "1", (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_0()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(0, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_0_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(0, typeof(bool), 0, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_0_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(0, typeof(bool), "0", (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_0_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(0, typeof(bool), 1, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_0_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.Convert(0, typeof(bool), "1", (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(null, typeof(int), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_True()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(true, typeof(int), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_True_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(true, typeof(int), 0, (CultureInfo)null), Is.EqualTo(0));
        }

        [TestCase]
        public void ConvertBack_True_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(true, typeof(int), "0", (CultureInfo)null), Is.EqualTo(0));
        }

        [TestCase]
        public void ConvertBack_True_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(true, typeof(int), 1, (CultureInfo)null), Is.EqualTo(1));
        }

        [TestCase]
        public void ConvertBack_True_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(true, typeof(int), "1", (CultureInfo)null), Is.EqualTo(1));
        }

        [TestCase]
        public void ConvertBack_False()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(false, typeof(int), null, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_False_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(false, typeof(int), 0, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_False_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(false, typeof(int), "0", (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_False_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(false, typeof(int), 1, (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }

        [TestCase]
        public void ConvertBack_False_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.That(converter.ConvertBack(false, typeof(int), "1", (CultureInfo)null), Is.EqualTo(ConverterHelper.UnsetValue));
        }
        #endregion
    }
}