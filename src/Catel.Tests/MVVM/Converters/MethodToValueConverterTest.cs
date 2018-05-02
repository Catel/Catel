// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodToValueConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
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

            Assert.AreEqual("1234", converter.Convert(1234, typeof (string), "ToString", (CultureInfo)null));
            Assert.AreEqual("ABCD", converter.Convert(" ABCD ", typeof (string), "Trim", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_NullValue()
        {
            var converter = new MethodToValueConverter();

            Assert.IsNull(converter.Convert(null, typeof (string), "ToString", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_InvalidMethod()
        {
            var converter = new MethodToValueConverter();

            Assert.AreEqual("Pineapple", converter.Convert("Pineapple", typeof (string), "InvalidMethodName", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new MethodToValueConverter();

            ExceptionTester.CallMethodAndExpectException<Exception>(() => converter.ConvertBack("ABCD", typeof (string), "ToString", (CultureInfo)null));
        }
        #endregion
    }
}