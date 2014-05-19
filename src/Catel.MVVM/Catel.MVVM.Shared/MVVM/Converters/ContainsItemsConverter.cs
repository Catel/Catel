// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainsItemsConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections;

    /// <summary>
    /// Converter that converts whether a collection contains items or not.
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof(IEnumerable), typeof(bool))]
#endif
    public class ContainsItemsConverter : ValueConverterBase
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
            if (value == null)
            {
                return false;
            }

            var collection = value as ICollection;
            if (collection != null)
            {
                return (collection.Count > 0);
            }

            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                foreach (object obj in enumerable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}