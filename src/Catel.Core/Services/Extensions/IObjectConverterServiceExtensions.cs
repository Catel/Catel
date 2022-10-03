namespace Catel.Services
{
    /// <summary>
    /// Extension methods for <see cref="IObjectConverterService"/>.
    /// </summary>
    public static class IObjectConverterServiceExtensions
    {
        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="value">The value.</param>
        /// <returns>The object value.</returns>
        public static T ConvertFromStringToObject<T>(this IObjectConverterService service, string value)
        {
            return (T) service.ConvertFromStringToObject(value, typeof (T));
        }

        /// <summary>
        /// Converts the specified object to an object.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="value">The value.</param>
        /// <returns>The object value.</returns>
        public static T ConvertFromObjectToObject<T>(this IObjectConverterService service, object value)
        {
            return (T)service.ConvertFromObjectToObject(value, typeof(T));
        }
    }
}
