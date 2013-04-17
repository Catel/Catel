// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceToCollapsingVisibilityConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Windows.Data.Converters
{
    using System.Globalization;
    using Catel.Windows.Data.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using global::Windows.UI.Xaml;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Windows;
#endif

    /// <summary>
    ///This is a test class for ReferenceToCollapsingVisibilityConverterTest and is intended
    ///to contain all ReferenceToCollapsingVisibilityConverterTest Unit Tests
    ///</summary>
    [TestClass]
    public class ReferenceToCollapsingVisibilityConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Null_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(null, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Null_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(1, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ValueType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(new object(), typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(new object(), typeof (Visibility), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_ReferenceType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(new object(), typeof (Visibility), false, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(ConverterHelper.DoNothingBindingValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}