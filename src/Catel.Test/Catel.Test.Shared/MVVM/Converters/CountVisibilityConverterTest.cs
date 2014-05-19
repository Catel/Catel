// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountVisibilityConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Collections.Generic;
    using System.Globalization;
    using Catel.MVVM.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using global::Windows.UI.Xaml;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Windows;
#endif

    [TestClass]
    public class CountVisibilityConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_EmptyList()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(new List<int>(), typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_FilledList()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(new List<int>(new[] {1, 2, 3}), typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_EmptyString()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(string.Empty, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_String()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("filledstring", typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Long_0()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(0L, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Long_1()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1L, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Int_0()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(0, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Int_1()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Short_0()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert((short) 0, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Short_1()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert((short) 1, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new CountCollapsedConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}