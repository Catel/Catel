// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceToCollapsingVisibilityConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    ///This is a test class for ReferenceToCollapsingVisibilityConverterTest and is intended
    ///to contain all ReferenceToCollapsingVisibilityConverterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class ReferenceToCollapsingVisibilityConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_Null_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(null, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_Null_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ValueType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ValueType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(1, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ValueType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(1, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ReferenceType()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(new object(), typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ReferenceType_ParameterTrue()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(new object(), typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_ReferenceType_ParameterFalse()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(new object(), typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new ReferenceToCollapsingVisibilityConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}