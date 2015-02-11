// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsSelectedValueConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for IsSelectedValueConverterTest and is intended
    ///to contain all IsSelectedValueConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class IsSelectedValueConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_1()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_1_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), 0, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_1_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(1, typeof (bool), "0", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_1_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), 1, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_1_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(1, typeof (bool), "1", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(0, typeof (bool), 0, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(true, converter.Convert(0, typeof (bool), "0", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), 1, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(false, converter.Convert(0, typeof (bool), "1", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (int), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(true, typeof (int), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(0, converter.ConvertBack(true, typeof (int), 0, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(0, converter.ConvertBack(true, typeof (int), "0", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(1, converter.ConvertBack(true, typeof (int), 1, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_True_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(1, converter.ConvertBack(true, typeof (int), "1", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_Parameter0AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), 0, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_Parameter0AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), "0", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_Parameter1AsValue()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), 1, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_False_Parameter1AsString()
        {
            var converter = new IsSelectedValueConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(false, typeof (int), "1", (CultureInfo)null));
        }
        #endregion
    }
}