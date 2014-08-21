// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlatformsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Converters
{
    using System;

    /// <summary>
    /// Returns a boolean whether the currently executing platform is available.
    /// </summary>
    public class PlatformToBooleanConverter : ValueConverterBase
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>System.Object.</returns>
        protected override object Convert(object value, System.Type targetType, object parameter)
        {
            var stringValue = ObjectToStringHelper.ToString(value);

            var isSupported = false;

            KnownPlatforms platform = KnownPlatforms.Unknown;
            if (Enum<KnownPlatforms>.TryParse(stringValue, out platform))
            {
                isSupported = Platforms.IsPlatformSupported(platform);
            }

            bool invert = ConverterHelper.ShouldInvert(parameter);
            if (invert)
            {
                isSupported = !isSupported;
            }

            return isSupported;
        }
    }
}
