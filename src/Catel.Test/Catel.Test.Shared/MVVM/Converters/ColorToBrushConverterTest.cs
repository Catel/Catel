// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorToBrushConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Media;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Windows.Media;
#endif

    [TestClass]
    public class ColorToBrushConverterTest
    {
        #region Methods
        [TestMethod]
        public void Convert_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(null, converter.Convert(null, typeof (Brush), null, (CultureInfo)null));
        }

        [TestMethod]
        public void Convert_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Black).ToString(), converter.Convert(Colors.Black, typeof (Brush), null, (CultureInfo)null).ToString());
        }

        [TestMethod]
        public void Convert_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Green).ToString(), converter.Convert(Colors.Green, typeof (Brush), null, (CultureInfo)null).ToString());
        }

        [TestMethod]
        public void ConvertBack_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(null, typeof (Color), null, (CultureInfo)null).ToString());
        }

        [TestMethod]
        public void ConvertBack_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Black), typeof (Color), null, (CultureInfo)null).ToString());
        }

        [TestMethod]
        public void ConvertBack_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Green.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Green), typeof (Color), null, (CultureInfo)null).ToString());
        }
        #endregion
    }
}