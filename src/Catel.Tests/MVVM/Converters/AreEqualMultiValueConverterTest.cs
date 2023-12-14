namespace Catel.Tests.MVVM.Converters
{
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class AreEqualMultiValueConverterTest
    {
        [TestCase]
        public void Convert_NotEnoughValues()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.Convert(new object?[] { 1 }, typeof(bool), null, null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_TooManyValues()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.Convert(new object?[] { 1, 1, 1 }, typeof(bool), null, null), Is.EqualTo(false));
        }

        [TestCase]
        public void Convert_Equal_Null()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.Convert(new object?[] { null, null }, typeof(bool), null, null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_Equal_Integer()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.Convert(new object?[] { 1, 1 }, typeof(bool), null, null), Is.EqualTo(true));
        }

        [TestCase]
        public void Convert_NonEqual()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.Convert(new object?[] { 1, 2 }, typeof(bool), null, null), Is.EqualTo(false));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new AreEqualMultiValueConverter();
            Assert.That(converter.ConvertBack(null, new[] { typeof(object) }, null, null), Is.EqualTo(null));
        }
    }
}
