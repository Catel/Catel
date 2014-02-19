// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorToBrushConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;

#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Data;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Data;
    using System.Windows.Media;
#endif

    /// <summary>
    /// ColorToBrushConverter
    /// </summary>
#if NET
    [ValueConversion(typeof (Color), typeof (Brush))]
#endif
    public class ColorToBrushConverter : ValueConverterBase
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            Brush brush = null;
            if (value is Color)
            {
                brush = new SolidColorBrush((Color)value);
            }

            return brush;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <remarks>
        /// By default, this method returns <see cref="ConverterHelper.UnsetBindingValue"/>. This method only has
        /// to be overridden when it is actually used.
        /// </remarks>
        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            var color = Colors.Black;
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                color = brush.Color;
            }

            return color;
        }
    }
}