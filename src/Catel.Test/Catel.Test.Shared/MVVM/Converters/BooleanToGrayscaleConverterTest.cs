﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToGrayscaleConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanToGrayscaleConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert(null, typeof (double), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_NonBooleanValue()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert("string", typeof (double), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(1d, converter.Convert(true, typeof (double), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(0d, converter.Convert(false, typeof (double), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new BooleanToGrayscaleConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}