// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsSelectedValueConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    ///This is a test class for IsSelectedValueConverterTest and is intended
    ///to contain all IsSelectedValueConverterTest Unit Tests
    ///</summary>
    [TestClass]
    public class IsSelectedValueConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_1()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_1_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), 0, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_1_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), "0", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_1_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), 1, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_1_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), "1", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_0()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_0_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(0, typeof (bool), 0, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_0_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(0, typeof (bool), "0", (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_0_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), 1, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_0_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), "1", (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (int), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(true, typeof (int), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(0, converter.ConvertBack(true, typeof (int), 0, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(0, converter.ConvertBack(true, typeof (int), "0", (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(1, converter.ConvertBack(true, typeof (int), 1, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_True_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(1, converter.ConvertBack(true, typeof (int), "1", (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), 0, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), "0", (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), 1, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack_False_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), "1", (CultureInfo)null));
        }
        #endregion
    }
}