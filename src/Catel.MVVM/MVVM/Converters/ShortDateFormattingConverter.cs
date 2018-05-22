// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortDateFormattingConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Converters
{
    using System;
    using System.Globalization;

    /// <summary>
    /// ShortDateFormattingConverter
    /// </summary>
#if NET
    [System.Windows.Data.ValueConversion(typeof(DateTime), typeof(string))]
#endif
    public class ShortDateFormattingConverter : FormattingConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortDateFormattingConverter"/> class.
        /// </summary>
        public ShortDateFormattingConverter()
            : base("d")
        {
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
            DateTime dateTimeValue;
            bool parsed = DateTime.TryParse(value as string, CurrentCulture, DateTimeStyles.None, out dateTimeValue);

            return parsed ? dateTimeValue : ConverterHelper.UnsetValue;
        }
    }
}