// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    [TestFixture]
    public class BooleanToVisibilityConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_NonBoolean()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert("string", typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(true, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_ParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(true, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_True_ParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(true, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_False()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(false, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_FalseParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(false, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_FalseParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(false, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(false, converter.ConvertBack(null, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_Visible()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(true, converter.ConvertBack(Visibility.Visible, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_VisibleParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(false, converter.ConvertBack(Visibility.Visible, typeof (bool), true, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_VisibleParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(true, converter.ConvertBack(Visibility.Visible, typeof (bool), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_Collapsed()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(false, converter.ConvertBack(Visibility.Collapsed, typeof (bool), null, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_CollapsedParameterTrue()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(true, converter.ConvertBack(Visibility.Collapsed, typeof (bool), true, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack_CollapsedParameterFalse()
        {
            var converter = new BooleanToCollapsingVisibilityConverter();
            Assert.AreEqual(false, converter.ConvertBack(Visibility.Collapsed, typeof (bool), false, (CultureInfo)null));
        }
        #endregion
    }
}