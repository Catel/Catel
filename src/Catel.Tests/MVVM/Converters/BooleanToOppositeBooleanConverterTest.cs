namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanToOppositeBooleanConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.Convert(null, typeof(bool), null, (CultureInfo)null), Is.EqualTo(null));
        }

        [TestCase]
        public void Convert_NonBoolean()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.Convert("test", typeof(bool), null, (CultureInfo)null), Is.EqualTo(null));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.Convert(true, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.Convert(false, typeof(bool), null, (CultureInfo)null), Is.EqualTo(true));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.ConvertBack(null, typeof(bool), null, (CultureInfo)null), Is.EqualTo(null));
        }

        [TestCase]
        public void ConvertBack_NonBoolean()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.ConvertBack("test", typeof(bool), null, (CultureInfo)null), Is.EqualTo(null));
        }

        [TestCase]
        public void ConvertBack_True()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.ConvertBack(true, typeof(bool), null, (CultureInfo)null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack_False()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.That(converter.ConvertBack(false, typeof(bool), null, (CultureInfo)null), Is.EqualTo(true));
        }
        #endregion
    }
}
