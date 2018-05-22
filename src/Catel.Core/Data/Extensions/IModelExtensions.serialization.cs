// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelExtensions.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Runtime.Serialization;

    public static partial class IModelExtensions
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
        public static XDocument ToXml(this IModel model, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("serializer", serializer);

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
        /// Serializes the object to a byte array.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Byte array containing the serialized data.
        /// </returns>
        public static byte[] ToByteArray(this IModel model, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("serializer", serializer);

            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream, configuration);

                memoryStream.Position = 0L;

                return memoryStream.ToArray();
            }
        }
    }
}