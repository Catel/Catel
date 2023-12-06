namespace Catel.Tests.MVVM.Converters
{
    using System;
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class MethodToValueConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_ValidMethod()
        {
            var converter = new MethodToValueConverter();

            Assert.That(converter.Convert(1234, typeof(string), "ToString", (CultureInfo)null), Is.EqualTo("1234"));
            Assert.That(converter.Convert(" ABCD ", typeof(string), "Trim", (CultureInfo)null), Is.EqualTo("ABCD"));
        }

        [TestCase]
        public void Convert_NullValue()
        {
            var converter = new MethodToValueConverter();

            Assert.That(converter.Convert(null, typeof(string), "ToString", (CultureInfo)null), Is.Null);
        }

        [TestCase]
        public void Convert_InvalidMethod()
        {
            var converter = new MethodToValueConverter();

            Assert.That(converter.Convert("Pineapple", typeof(string), "InvalidMethodName", (CultureInfo)null), Is.EqualTo("Pineapple"));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new MethodToValueConverter();

            Assert.Throws<NotSupportedException>(() => converter.ConvertBack("ABCD", typeof(string), "ToString", (CultureInfo)null));
        }
        #endregion
    }
}
