// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorToBrushConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.MVVM.Converters
{
    using System;

#if NETFX_CORE
    using global::Windows.UI;
    using global::Windows.UI.Xaml.Media;
#else
    using System.Windows.Media;
#endif

    /// <summary>
    /// ColorToBrushConverter
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof(Color), typeof(Brush))]
#endif
    public class ColorToBrushConverter : ValueConverterBase<Color, Brush>
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(Color value, Type targetType, object parameter)
        {
            return new SolidColorBrush(value);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <remarks>
        /// By default, this method returns <see cref="ConverterHelper.UnsetValue"/>. This method only has
        /// to be overridden when it is actually used.
        /// </remarks>
        protected override object ConvertBack(Brush value, Type targetType, object parameter)
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

#endif