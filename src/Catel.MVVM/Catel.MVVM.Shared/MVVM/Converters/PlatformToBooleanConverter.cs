﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlatformsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
            var parameterAsString = ObjectToStringHelper.ToString(parameter);

            var supportedPlatforms = parameterAsString.Split(new[] { '|' });

            foreach (var supportedPlatform in supportedPlatforms)
            {
                KnownPlatforms platform = KnownPlatforms.Unknown;
                if (Enum<KnownPlatforms>.TryParse(supportedPlatform, out platform))
                {
                    if (Platforms.IsPlatformSupported(platform))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
