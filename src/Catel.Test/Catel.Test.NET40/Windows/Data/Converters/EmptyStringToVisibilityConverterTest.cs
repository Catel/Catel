// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyStringToVisibilityConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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

    [TestClass]
    public class EmptyStringToVisibilityConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(null, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_EmptyString()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(string.Empty, typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_EmptyString_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert(string.Empty, typeof (Visibility), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_EmptyString_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert(string.Empty, typeof (Visibility), false, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_String()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("filledstring", typeof (Visibility), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_String_ParameterTrue()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Collapsed, converter.Convert("filledstring", typeof (Visibility), true, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_String_ParameterFalse()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(Visibility.Visible, converter.Convert("filledstring", typeof (Visibility), false, (CultureInfo)null));
        }

        [TestMethod]
        public void ConvertBack()
        {
            var converter = new EmptyStringToCollapsingVisibilityConverter();
            Assert.AreEqual(ConverterHelper.DoNothingBindingValue, converter.ConvertBack(null, typeof (object), null, (CultureInfo)null));
        }
        #endregion
    }
}