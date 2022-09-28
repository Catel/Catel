namespace Catel
{
    using System.IO;
    using Data;
    using IoC;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Runtime.Serialization;
    using JsonSerializer = Runtime.Serialization.Json.JsonSerializer;

    /// <summary>
    /// Json extensions.
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly ISerializationManager SerializationManager = ServiceLocator.Default.ResolveType<ISerializationManager>();
        private static readonly Catel.Runtime.Serialization.IObjectAdapter ObjectAdapter = ServiceLocator.Default.ResolveType<Catel.Runtime.Serialization.IObjectAdapter>();

        /// <summary>
        /// Converters the specified model to a json string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ToJson(this ModelBase model, ISerializationConfiguration configuration = null)
        {
            var jsonSerializer = new JsonSerializer(SerializationManager, TypeFactory.Default, ObjectAdapter);

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
        public static JsonReader CreateReader(this JToken token, ISerializationConfiguration configuration)
        {
            var reader = token.CreateReader();
            reader.Culture = configuration.Culture;

            return reader;
        }
    }
}
