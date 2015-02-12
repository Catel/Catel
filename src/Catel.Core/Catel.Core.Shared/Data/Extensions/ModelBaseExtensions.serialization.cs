// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.IO;
    using System.Xml.Serialization;
    using Catel.Runtime.Serialization;

    public static partial class ModelBaseExtensions
    {
        /// <summary>
        /// Saves as XML.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public static void SaveAsXml(this ModelBase model, Stream stream)
        {
            Save(model, stream, SerializationFactory.GetXmlSerializer());
        }

        /// <summary>
        /// Saves the specified model to the stream using the serializer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="serializer">The serializer.</param>
        public static void Save(this ModelBase model, Stream stream, IModelBaseSerializer serializer)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("serializer", serializer);

            serializer.Serialize(model, stream);
        }

#if NET
        /// <summary>
        /// Saves the specified model to the file as xml.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="filePath">The file path.</param>
        public static void SaveAsXml(this ModelBase model, string filePath)
        {
            Save(model, filePath, SerializationFactory.GetXmlSerializer());
        }

        /// <summary>
        /// Saves the specified model to the file using the specified serializer.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="serializer">The serializer.</param>
        public static void Save(this ModelBase model, string filePath, IModelBaseSerializer serializer)
        {
            Argument.IsNotNullOrWhitespace("filePath", filePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                Save(model, fileStream, serializer);
            }
        }
#endif
    }
}