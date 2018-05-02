// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToLowerCaseConverterFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.Converters
{
    using System.Diagnostics;
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    public class TextToUpperCaseConverterFacts
    {
        #region Nested type: TheConvertMethod
        [TestFixture]
        public class TheConvertMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsUpperCaseString()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual("UPPERCASE", converter.Convert("UpPeRcAsE", typeof(string), null, (CultureInfo)null));
            }

            [TestCase]
            public void SecondCallRunsFasterThanFirstOne()
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
            public void ReturnsNullForNullValue()
            {
                var converter = new TextToUpperCaseConverter();

                Assert.AreEqual(null, converter.Convert(null, typeof(string), null, (CultureInfo)null));
            }
            #endregion
        }
        #endregion
    }
}