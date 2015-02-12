// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AreEqualMultiValueConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.MVVM.Converters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Converts a comparison of 2 bindings to a boolean whether the 
    /// objects are equal or not.
    /// </summary>
    [ValueConversion(typeof(object), typeof(object))]
    public class AreEqualMultiValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts the comparison of 2 values to a boolean.
        /// </summary>
        /// <param name="values">Values to convert. Only 2 values are supported.</param>
        /// <param name="targetType">Not supported.</param>
        /// <param name="parameter">Not supported.</param>
        /// <param name="culture">Not supported.</param>
        /// <returns>True if the values are equal, otherwise false.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return false;
            }

            object value1 = values[0];
            object value2 = values[1];

            if ((value1 == null) && (value2 == null))
            {
                return true;
            }

            if ((value1 == null) || (value2 == null))
            {
                return false;
            }

            return value1.Equals(value2);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">Not supported.</param>
        /// <param name="targetTypes">Not supported.</param>
        /// <param name="parameter">Not supported.</param>
        /// <param name="culture">Not supported.</param>
        /// <returns>Not supported.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not supported (and IMultiValueConverter must return null if no conversion is supported)
            return null;
        }
    }
}

#endif