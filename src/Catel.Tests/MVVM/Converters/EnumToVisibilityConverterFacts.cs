namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;
    using System.Windows;

    [TestFixture]
    public class EnumToVisibilityConverterTest
    {
        public enum ConverterTestEnum
        {
            Value1,

            Value2,

            Value3
        }

        [TestCase(ConverterTestEnum.Value1, "Value1|Value2", Visibility.Visible)]
        [TestCase(ConverterTestEnum.Value1, "Value2|Value3", Visibility.Collapsed)]
        [TestCase(ConverterTestEnum.Value1, "!Value1|Value2", Visibility.Collapsed)]
        [TestCase(ConverterTestEnum.Value1, "!Value2|Value3", Visibility.Visible)]
        public void Convert(object enumValue, string parameter, Visibility expectedVisibility)
        {
            var converter = new EnumToCollapsingVisibilityConverter();
            Assert.AreEqual(expectedVisibility, converter.Convert(enumValue, typeof(Visibility), parameter, (CultureInfo)null));
        }
    }
}
