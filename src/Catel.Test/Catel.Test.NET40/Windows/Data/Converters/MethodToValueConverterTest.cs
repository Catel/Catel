// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodToValueConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows.Data.Converters
{
    using System;
    using System.Globalization;
    using Catel.Windows.Data.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class MethodToValueConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_ValidMethod()
        {
            var converter = new MethodToValueConverter();

            Assert.AreEqual("1234", converter.Convert(1234, typeof (string), "ToString", (CultureInfo)null));
            Assert.AreEqual("ABCD", converter.Convert(" ABCD ", typeof (string), "Trim", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_NullValue()
        {
            var converter = new MethodToValueConverter();

            Assert.IsNull(converter.Convert(null, typeof (string), "ToString", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_InvalidMethod()
        {
            var converter = new MethodToValueConverter();

            Assert.AreEqual("Pineapple", converter.Convert("Pineapple", typeof (string), "InvalidMethodName", (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new MethodToValueConverter();

            ExceptionTester.CallMethodAndExpectException<Exception>(() => converter.ConvertBack("ABCD", typeof (string), "ToString", (CultureInfo)null));
        }
        #endregion
    }
}