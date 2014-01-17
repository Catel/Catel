// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToGrayscaleConverter.cs" company="Catel development team">
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
    /// Converts a boolean to a grayscale saturation value. If the input is <c>false</c>, this converter will
    /// return <c>0</c>, otherwise <c>1</c>.
    /// </summary>
#if NET
    [ValueConversion(typeof(bool), typeof(double))]
#endif
    public class BooleanToGrayscaleConverter : ValueConverterBase
    {
        /// <summary>
        /// Initialzies the <see cref="BooleanToGrayscaleConverter"/>.
        /// </summary>
        public BooleanToGrayscaleConverter()
        {
            FalseResult = 0d;
            TrueResult = 1d;
        }

        /// <summary>
        /// The value a input value of false will be converted to.
        /// <para />
        /// The default value is <c>0</c>.
        /// </summary>
        public double FalseResult { get; set; }

        /// <summary>
        /// The value a input value of true will be converted to.<br />
        /// <para />
        /// The default value is <c>1</c>.
        /// </summary>
        public double TrueResult { get; set; }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is bool))
            {
                return TrueResult;
            }

            return ((bool)value) ? TrueResult : FalseResult;
        }
    }
}