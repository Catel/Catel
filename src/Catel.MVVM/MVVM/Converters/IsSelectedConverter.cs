// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsSelectedConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;
    using Catel.Data;

    /// <summary>
    /// Converts a selected value to either true of false.
    /// </summary>
    /// <remarks>
    /// This converter is very usefull when a mutual exclusive selection must be made
    /// Original code found at http://geekswithblogs.net/claraoscura/archive/2008/10/17/125901.aspx
    /// </remarks>
#if NET || NETCORE
    [System.Windows.Data.ValueConversion(typeof(bool?), typeof(bool))]
#endif
    public class IsSelectedConverter : ValueConverterBase
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
            bool param = true;
            if (parameter is string)
            {
                bool.TryParse((string)parameter, out param);
            }

            return BoxingCache.GetBoxedValue((value is null) ? false : !((bool)value ^ param));
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
            bool param = true;
            if (parameter is string)
            {
                bool.TryParse((string)parameter, out param);
            }

            return BoxingCache.GetBoxedValue(!((bool)value ^ param));
        }
    }
}
