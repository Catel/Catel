namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for IsSelectedConverterTest and is intended
    ///to contain all IsSelectedConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class IsSelectedConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof(bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof(bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof(bool), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof(bool), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof(bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof(bool), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof(bool), true, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof(bool?), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof(bool?), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof(bool?), true, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof(bool?), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof(bool?), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof(bool?), true, (CultureInfo)null));
        }
        #endregion
    }
}