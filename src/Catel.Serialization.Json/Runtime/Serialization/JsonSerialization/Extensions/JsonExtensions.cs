namespace Catel
{
    using System.Globalization;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Runtime.Serialization;

    /// <summary>
    /// Json extensions.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Converters the specified model to a json string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ToJson(this ModelBase model, IJsonSerializer jsonSerializer, ISerializationConfiguration? configuration = null)
        {
            using (var stream = new MemoryStream())
            {
                jsonSerializer.Serialize(model, stream, configuration);

                stream.Position = 0L;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

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
