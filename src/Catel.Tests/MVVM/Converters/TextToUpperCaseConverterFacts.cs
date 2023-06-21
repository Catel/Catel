namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    public class TextToUpperCaseConverterFacts
    {
        [TestFixture]
        public class TheConvertMethod
        {
            [TestCase]
            public void Returns_UpperCase_String()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual("UPPERCASE", converter.Convert("UpPeRcAsE", typeof(string), null, (CultureInfo)null));
            }

            [TestCase]
            public void Returns_Null_For_Null_Value()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual(null, converter.Convert(null, typeof(string), null, (CultureInfo)null));
            }
        }
    }
}
