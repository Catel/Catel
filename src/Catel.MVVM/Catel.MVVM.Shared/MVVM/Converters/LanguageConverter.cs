// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageConverter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Converters
{
    using System;
    using System.Globalization;
    using Converters;
    using IoC;
    using Services;

    /// <summary>
    /// Converts the value (the resource name) to a language string.
    /// </summary>
    public class LanguageConverter : ValueConverterBase<string>
    {
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageConverter"/> class.
        /// </summary>
        public LanguageConverter()
        {
            _languageService = ServiceLocator.Default.ResolveType<ILanguageService>();
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns>System.Object.</returns>
        protected override object Convert(string value, Type targetType, object parameter)
        {
            var translatedValue = string.Empty;

            var culture = parameter as CultureInfo;
            if (culture != null)
            {
                translatedValue = _languageService.GetString(value, culture);
            }
            else
            {
                translatedValue = _languageService.GetString(value);
            }

            return translatedValue;
        }
    }
}