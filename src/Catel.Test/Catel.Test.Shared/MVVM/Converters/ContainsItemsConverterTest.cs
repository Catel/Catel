// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainsItemsConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

    [TestFixture]
    public class ContainsItemsConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyList()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert(new List<int>(), typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_FilledList()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(true, converter.Convert(new List<int>(new[] {1, 2}), typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyArray()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert(new int[] {}, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_FilledArray()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(true, converter.Convert(new[] {1, 2}, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyString()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(false, converter.Convert("", typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_FilledString()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(true, converter.Convert("filledstring", typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new ContainsItemsConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}