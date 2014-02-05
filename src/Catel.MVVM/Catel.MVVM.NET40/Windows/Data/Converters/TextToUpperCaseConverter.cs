// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToUpperCaseConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Data.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Converts string values to upper case.
    /// </summary>
    public class TextToUpperCaseConverter : ValueConverterBase
    {
        #region Methods
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
#if NETFX_CORE
                return stringValue.ToUpper();
#else
                return stringValue.ToUpper(CurrentCulture ?? CultureInfo.CurrentCulture);
#endif
            }

            return value;
        }
        #endregion
    }
}