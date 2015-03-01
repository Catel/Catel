// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiplyConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    /// <summary>
    ///This is a test class for MultiplyConverterTest and is intended
    ///to contain all MultiplyConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class MultiplyConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(0d, converter.Convert(null, typeof (int), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_0_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(0d, converter.Convert(0, typeof (int), "5", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_5_Parameter0()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(0d, converter.Convert(5, typeof (int), "0", (CultureInfo)null));
        }

        [TestCase]
        public void Convert_2_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(10d, converter.Convert(2, typeof (int), "5", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_0_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(double.PositiveInfinity, converter.ConvertBack(5, typeof (int), "0", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_5_Parameter0()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(0d, converter.ConvertBack(0, typeof (int), "5", (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_10_Parameter5()
        {
            var converter = new MultiplyConverter();
            Assert.AreEqual(2d, converter.ConvertBack(10, typeof (int), "5", (CultureInfo)null));
        }
        #endregion
    }
}