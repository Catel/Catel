namespace Catel.Data
{
    using System.IO;
    using Runtime.Serialization;

    public static partial class IModelExtensions
    {
        /// <summary>
        /// Serializes the object to a byte array.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Byte array containing the serialized data.
        /// </returns>
        public static byte[] ToByteArray(this IModel model, ISerializer serializer, ISerializationConfiguration? configuration = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream, configuration);

                memoryStream.Position = 0L;

                return memoryStream.ToArray();
            }
        }
    }
}
