// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceToBooleanConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
    /// Implementation of class ReferenceToBooleanConverter
    /// </summary>
#if NET
    [ValueConversion(typeof (object), typeof (bool))]
#endif
    public class ReferenceToBooleanConverter : ValueConverterBase
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
            bool isNull = value == null;
            bool invert = ConverterHelper.ShouldInvert(parameter);

            return invert ? isNull : !isNull;
        }
    }
}