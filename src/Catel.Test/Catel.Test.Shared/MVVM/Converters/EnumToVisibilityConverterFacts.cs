// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumToVisibilityConverterFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    public class EnumToVisibilityConverterTest
    {
        public enum ConverterTestEnum
        {
            Value1,

            Value2,

            Value3
        }

        #region Methods
        [TestCase(ConverterTestEnum.Value1, "Value1|Value2", Visibility.Visible)]
        [TestCase(ConverterTestEnum.Value1, "Value2|Value3", Visibility.Collapsed)]
        [TestCase(ConverterTestEnum.Value1, "!Value1|Value2", Visibility.Collapsed)]
        [TestCase(ConverterTestEnum.Value1, "!Value2|Value3", Visibility.Visible)]
        public void Convert(object enumValue, string parameter, Visibility expectedVisibility)
        {
            var converter = new EnumToCollapsingVisibilityConverter();
            Assert.AreEqual(expectedVisibility, converter.Convert(enumValue, typeof(Visibility), parameter, (CultureInfo)null));
        }
        #endregion
    }
}