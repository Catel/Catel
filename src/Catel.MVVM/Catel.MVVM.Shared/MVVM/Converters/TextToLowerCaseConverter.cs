// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToLowerCaseConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Converters
{
    using System;
    using System.Globalization;
    using Caching;

    /// <summary>
    /// Converts string values to lower case.
    /// </summary>
    public class TextToLowerCaseConverter : ValueConverterBase
    {
        /// <summary>
        /// The cache storage.
        /// </summary>
        private readonly ICacheStorage<string, string> _cacheStorage = new CacheStorage<string, string>();

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
                value = _cacheStorage.GetFromCacheOrFetch(stringValue, () =>
                {
#if NETFX_CORE || XAMARIN_FORMS
                    return stringValue.ToLower();
#else
                    return stringValue.ToLower(CurrentCulture ?? CultureInfo.CurrentCulture);
#endif
                });
            }

            return value;
        }
    }
}