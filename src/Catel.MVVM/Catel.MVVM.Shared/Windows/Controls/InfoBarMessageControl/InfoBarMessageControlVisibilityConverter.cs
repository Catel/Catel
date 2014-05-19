// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoBarMessageControlVisibilityConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.Windows.Controls
{
    using System;
    using System.Windows;
    using MVVM.Converters;

    /// <summary>
    /// Converter for the <see cref="InfoBarMessageControl"/> to determine whether the control
    /// should be visible for the current mode and
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof(InfoBarMessageControlMode), typeof(Visibility), ParameterType = typeof(InfoBarMessageControlMode))]
#endif
    public class InfoBarMessageControlVisibilityConverter : IValueConverter
    {

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is not of type <see cref="InfoBarMessageControlMode"/>.</exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Argument.IsNotNull("value", value);
            Argument.IsOfType("value", value, typeof(InfoBarMessageControlMode));

            InfoBarMessageControlMode mode = (parameter is InfoBarMessageControlMode) ? (InfoBarMessageControlMode) parameter : InfoBarMessageControlMode.Inline;

            if (parameter is string)
            {
                Enum<InfoBarMessageControlMode>.TryParse((string)parameter, out mode);
            }

            return ((InfoBarMessageControlMode)value == mode) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ConverterHelper.UnsetValue;
        }
    }
}

#endif