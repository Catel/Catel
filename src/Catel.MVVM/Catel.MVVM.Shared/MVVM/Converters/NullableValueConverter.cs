// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableValueConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;

    /// <summary>
    /// Converts a value to a representive value for nullable.
    /// </summary>
    /// <remarks>Resolves problem with databinding with nullables. When textbox hasn't a value then null is expected as return value.</remarks>
#if NET
    [System.Windows.Data.ValueConversion(typeof(object), typeof(object))]
#endif
    public class NullableValueConverter : ValueConverterBase
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
            // not the interesting part for us, use default conversion
            return System.Convert.ChangeType(value, targetType, CurrentCulture);
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
            object result = null;
            string str = value as string;
            if (value != null && (str == null || !string.IsNullOrEmpty(str.Trim())))
            {
                Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                try
                {
                    result = System.Convert.ChangeType(value, underlyingType, CurrentCulture);
                }
                catch
                {
                    // ignore exceptions
                }
            }

            return result;
        }
    }
}