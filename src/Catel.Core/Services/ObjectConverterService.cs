namespace Catel.Services
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Service to convert objects to strings and vice versa.
    /// </summary>
    public class ObjectConverterService : IObjectConverterService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectConverterService"/> class.
        /// </summary>
        public ObjectConverterService()
        {
            DefaultCulture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the default culture.
        /// </summary>
        /// <value>The default culture.</value>
        public CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Converts the specified object to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string value.</returns>
        public virtual string ConvertFromObjectToString(object? value)
        {
            return ConvertFromObjectToString(value, DefaultCulture);
        }

        /// <summary>
        /// Converts from object to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string ConvertFromObjectToString(object? value, CultureInfo culture)
        {
            return ObjectToStringHelper.ToString(value, culture);
        }

        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The object value.</returns>
        public virtual object ConvertFromStringToObject(string value, Type targetType)
        {
            return ConvertFromStringToObject(value, targetType, DefaultCulture);
        }

        /// <summary>
        /// Converts from string to object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertFromStringToObject(string value, Type targetType, CultureInfo culture)
        {
            return StringToObjectHelper.ToRightType(targetType, value, culture);
        }

        /// <summary>
        /// Converts the specified object to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The object value.</returns>
        public virtual object ConvertFromObjectToObject(object? value, Type targetType)
        {
            var stringValue = ConvertFromObjectToString(value);
            return ConvertFromStringToObject(stringValue, targetType);
        }
    }
}
