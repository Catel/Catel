// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorToBrushConverterTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.Converters
{
    using System.Globalization;
    using Catel.MVVM.Converters;

    using NUnit.Framework;

#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Media;
#endif

    [TestFixture]
    public class ColorToBrushConverterTest
    {
        #region Methods
        [TestCase]
        public void Convert_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(null, converter.Convert(null, typeof (Brush), null, (CultureInfo)null));
        }

        [TestCase]
        public void Convert_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Black).ToString(), converter.Convert(Colors.Black, typeof (Brush), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void Convert_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(new SolidColorBrush(Colors.Green).ToString(), converter.Convert(Colors.Green, typeof (Brush), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Null()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(null, typeof (Color), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Black()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Black.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Black), typeof (Color), null, (CultureInfo)null).ToString());
        }

        [TestCase]
        public void ConvertBack_Green()
        {
            var converter = new ColorToBrushConverter();
            Assert.AreEqual(Colors.Green.ToString(), converter.ConvertBack(new SolidColorBrush(Colors.Green), typeof (Color), null, (CultureInfo)null).ToString());
        }
        #endregion
    }
}