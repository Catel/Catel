// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToOppositeBooleanConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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
    public class BooleanToOppositeBooleanConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(null, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_NonBoolean()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(null, converter.Convert("test", typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_True()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(false, converter.Convert(true, typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_False()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(true, converter.Convert(false, typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_Null()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(null, converter.ConvertBack(null, typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_NonBoolean()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(null, converter.ConvertBack("test", typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(false, converter.ConvertBack(true, typeof(bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False()
        {
            var converter = new BooleanToOppositeBooleanConverter();
            Assert.AreEqual(true, converter.ConvertBack(false, typeof(bool), null, (CultureInfo)null));
        }
        #endregion
    }
}