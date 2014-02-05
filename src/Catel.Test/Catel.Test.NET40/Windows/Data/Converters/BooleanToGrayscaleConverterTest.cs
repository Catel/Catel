// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToGrayscaleConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows.Data.Converters
{
    using System.Globalization;
    using Catel.Windows.Data.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class BooleanToGrayscaleConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert(null, typeof (double), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_NonBooleanValue()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert("string", typeof (double), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_True()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert(true, typeof (double), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_False()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(0d, converter.Convert(false, typeof (double), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(ConverterHelper.DoNothingBindingValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}