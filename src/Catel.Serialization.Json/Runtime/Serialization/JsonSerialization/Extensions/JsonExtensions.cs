namespace Catel
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Runtime.Serialization;

    /// <summary>
    /// Json extensions.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Creates a json reader with the right configuration.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The json reader.
        /// </returns>
        public static JsonReader CreateReader(this JToken token, ISerializationConfiguration? configuration)
        {
            var reader = token.CreateReader();
            reader.Culture = configuration?.Culture ?? CultureInfo.InvariantCulture;

            return reader;
        }
    }
}
