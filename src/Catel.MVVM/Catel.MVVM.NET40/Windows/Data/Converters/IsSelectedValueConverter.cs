// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsSelectedValueConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data.Converters
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Converts a selected value to either true of false.
    /// </summary>
    /// <remarks>
    /// This converter is very usefull when a mutual exclusive selection must be made
    /// Original code found at http://geekswithblogs.net/claraoscura/archive/2008/10/17/125901.aspx
    /// </remarks>
#if NET
    [ValueConversion(typeof(int?), typeof(bool))]
#endif
    public class IsSelectedValueConverter : ValueConverterBase
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
            int param;
            if (parameter is int)
            {
                param = (int)parameter;
            }
            else if (parameter is string)
            {
                param = int.Parse((string)parameter);
            }
            else
            {
                return false;
            }

            return (value == null) ? false : (int)value == param;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the source object.</returns>
        /// <remarks>
        /// By default, this method returns <see cref="ConverterHelper.DoNothingBindingValue"/>. This method only has
        /// to be overridden when it is actually used.
        /// </remarks>
        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (!(value is bool))
            {
                return ConverterHelper.DoNothingBindingValue;
            }

            int param;
            if (parameter is int)
            {
                param = (int)parameter;
            }
            else if (parameter is string)
            {
                param = int.Parse((string)parameter);
            }
            else
            {
                return ConverterHelper.DoNothingBindingValue;
            }

            if ((bool)value)
            {
                return param;
            }

            return ConverterHelper.DoNothingBindingValue;
        }
    }
}
