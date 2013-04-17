// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsSelectedConverterTest.cs" company="Catel development team">
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

    /// <summary>
    ///This is a test class for IsSelectedConverterTest and is intended
    ///to contain all IsSelectedConverterTest Unit Tests
    ///</summary>
    [TestClass]
    public class IsSelectedConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_True()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_True_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof (bool), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_True_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.Convert(true, typeof (bool), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_False()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_False_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof (bool), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_False_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.Convert(false, typeof (bool), true, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof (bool?), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof (bool?), false, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(true, converter.ConvertBack(true, typeof (bool?), true, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof (bool?), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_ParameterFalse()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof (bool?), false, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_ParameterTrue()
        {
            var converter = new IsSelectedConverter();
            Assert.AreEqual(false, converter.ConvertBack(false, typeof (bool?), true, (CultureInfo)null));
        }
        #endregion
    }
}