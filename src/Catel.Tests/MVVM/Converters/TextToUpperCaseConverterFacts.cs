namespace Catel.Tests.MVVM.Converters
{
    using System.Diagnostics;
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
            public void Second_Call_Runs_Faster_Than_First_One()
            {
                var converter = new TextToLowerCaseConverter();

                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                converter.Convert("UpPeRcAsE", typeof(string), null, (CultureInfo)null);
                stopwatch1.Stop();

                Stopwatch stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                converter.Convert("UpPeRcAsE", typeof(string), null, (CultureInfo)null);
                stopwatch2.Stop();

                Assert.Less(stopwatch2.Elapsed, stopwatch1.Elapsed);
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
