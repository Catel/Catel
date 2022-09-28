namespace Catel.MVVM.Converters
{
    using System;
    using System.Globalization;
    using Caching;

    /// <summary>
    /// Converts string values to upper case.
    /// </summary>
    public class TextToUpperCaseConverter : ValueConverterBase
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
            if (stringValue is not null)
            {
                return _cacheStorage.GetFromCacheOrFetch(stringValue, () =>
                {
                    return stringValue.ToUpper(CurrentCulture ?? CultureInfo.CurrentCulture);
                });
            }

            return value;
        }
    }
}
