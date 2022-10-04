namespace Catel.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Service to convert objects to strings and vice versa.
    /// </summary>
    public interface IObjectConverterService
    {
        /// <summary>
        /// Gets or sets the default culture to use for parsing.
        /// </summary>
        /// <value>The default culture.</value>
        CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Converts the specified object to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string value.</returns>
        string ConvertFromObjectToString(object? value);

        /// <summary>
        /// Converts the specified object to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The string value.</returns>
        string ConvertFromObjectToString(object? value, CultureInfo culture);

        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The object value.</returns>
        object ConvertFromStringToObject(string value, Type targetType);

        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The object value.</returns>
        object ConvertFromStringToObject(string value, Type targetType, CultureInfo culture);

        /// <summary>
        /// Converts the specified object to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The object value.</returns>
        object ConvertFromObjectToObject(object? value, Type targetType);
    }
}
