// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiplyConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;

    /// <summary>
    /// Calculate the product of given value and factor in parameter.
    /// </summary>
#if NET || NETCORE
    [System.Windows.Data.ValueConversion(typeof(int), typeof(int))]
#endif
    public class MultiplyConverter : ValueConverterBase
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
            double typedValue = 0d;

            if (value is int)
            {
                typedValue = System.Convert.ToDouble((int)value);
            }
            else if (value is double)
            {
                typedValue = (double)value;
            }

            if (typedValue == 0d)
            {
                return 0d;
            }

            double factor;
            if (!double.TryParse(parameter as string, out factor))
            {
                return 0d;
            }

            return typedValue * factor;
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
        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            double typedValue = 0d;

            if (value is int)
            {
                typedValue = System.Convert.ToDouble((int)value);
            }
            else if (value is double)
            {
                typedValue = (double)value;
            }

            if (typedValue == 0d)
            {
                return 0d;
            }

            double factor;
            if (!double.TryParse(parameter as string, out factor))
            {
                return 0d;
            }

            return typedValue / factor;
        }
    }
}
