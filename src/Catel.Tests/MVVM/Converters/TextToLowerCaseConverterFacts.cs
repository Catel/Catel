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

    public class TextToLowerCaseConverterFacts
    {
        #region Nested type: TheConvertMethod
        [TestFixture]
        public class TheConvertMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsLowerCaseString()
            {
                var converter = new TextToLowerCaseConverter();
                var actualValue = converter.Convert("LoWeRcAsE", typeof(string), null, (CultureInfo) null);

                Assert.AreEqual("lowercase", actualValue);
            }

            [TestCase]
            public void SecondCallRunsFasterThanFirstOne()
            {
                var converter = new TextToLowerCaseConverter();

                var stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                converter.Convert("LoWeRcAsE", typeof(string), null, (CultureInfo) null);
                stopwatch1.Stop();

                var stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                converter.Convert("LoWeRcAsE", typeof(string), null, (CultureInfo) null);
                stopwatch2.Stop();

                Assert.Less(stopwatch2.Elapsed, stopwatch1.Elapsed);
            }

            [TestCase]
            public void ReturnsNullForNullValue()
            {
                var converter = new TextToLowerCaseConverter();

                Assert.AreEqual(null, converter.Convert(null, typeof (string), null, (CultureInfo) null));
            }
            #endregion
        }
        #endregion
    }
}