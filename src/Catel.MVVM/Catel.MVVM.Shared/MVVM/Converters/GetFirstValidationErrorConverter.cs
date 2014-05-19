// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFirstValidationErrorConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

namespace Catel.MVVM.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Converts a collection containing <see cref="ValidationError"/> objects to return the first error
    /// or an empty string in case there are no errors.
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof(ICollection<ValidationError>), typeof(string))]
#endif
    public class GetFirstValidationErrorConverter : ValueConverterBase
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
                return string.Empty;
            }

            var errorCollection = value as ICollection<ValidationError>;
            if (errorCollection == null)
            {
                return string.Empty;
            }

            var firstError = errorCollection.FirstOrDefault();
            if (firstError == null)
            {
                return string.Empty;
            }

            if (firstError.ErrorContent == null)
            {
                return string.Empty;
            }

            return firstError.ErrorContent.ToString();
        }
    }
}

#endif