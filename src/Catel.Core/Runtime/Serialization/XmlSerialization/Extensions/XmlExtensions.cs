namespace Catel
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization.Xml;
    using Runtime.Serialization;

    /// <summary>
    /// XML extensions.
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Serializes the object to and xml object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        ///   <see cref="XDocument" /> containing the serialized data.
        /// </returns>
        public static XDocument ToXml(this IModel model, ISerializer serializer, ISerializationConfiguration? configuration = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream, configuration);

                memoryStream.Position = 0L;

                using (var xmlReader = XmlReader.Create(memoryStream))
                {
                    return XDocument.Load(xmlReader);
                }
            }
        }


        /// <summary>
        /// Converters the specified model to an xml string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="xmlSerializer">The xml serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ToXmlString(this ModelBase model, IXmlSerializer xmlSerializer, ISerializationConfiguration? configuration = null)
        {
            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(model, stream, configuration);

                stream.Position = 0L;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
