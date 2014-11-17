// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyStringToVisibilityConverterTest.cs" company="Catel development team">
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

    [TestFixture]
    public class EmptyStringToVisibilityConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyString()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(string.Empty, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyString_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(string.Empty, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_EmptyString_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(string.Empty, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_String()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("filledstring", typeof (Visibility), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_String_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert("filledstring", typeof (Visibility), true, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_String_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("filledstring", typeof (Visibility), false, (CultureInfo)null));
        }

        [TestCase]
        public void ConvertBack()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(ConverterHelper.UnsetValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}