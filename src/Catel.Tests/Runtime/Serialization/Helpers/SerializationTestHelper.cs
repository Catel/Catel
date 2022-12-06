namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using System.Diagnostics;
    using Catel.IoC;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;

    public static class SerializationTestHelper
    {
        public static IXmlSerializer GetXmlSerializer(ISerializationManager serializationManager = null)
        {
            if (serializationManager is null)
            {
                serializationManager = new SerializationManager();
            }

            var serializer = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<XmlSerializer>(serializationManager);
            return serializer;
        }

        public static IJsonSerializer GetJsonSerializer(ISerializationManager serializationManager = null)
        {
            if (serializationManager is null)
            {
                serializationManager = new SerializationManager();
            }

            var serializer = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<JsonSerializer>(serializationManager);
            return serializer;
        }

        /// <summary>
        /// Serializes and deserializes using the specified serializer.
        /// </summary>
        /// <typeparam name="TModel">The type of the T model.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.Object.
        /// </returns>
        public static TModel SerializeAndDeserialize<TModel>(TModel model, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream, configuration);

                memoryStream.Position = 0L;

                if (Debugger.IsAttached)
                {
#pragma warning disable IDISP001 // Dispose created.
                    var streamReader = new StreamReader(memoryStream);
#pragma warning restore IDISP001 // Dispose created.
                    var streamAsText = streamReader.ReadToEnd();

                    Console.WriteLine(streamAsText);

                    memoryStream.Position = 0L;
                }

                // Note: we use model.GetType to always ensure the correct type (even if 'object' type is specified)
                return (TModel)serializer.Deserialize(model.GetType(), memoryStream, configuration);
            }
        }

        public static string ToXmlString(this object model)
        {
            ArgumentNullException.ThrowIfNull(model);

            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = SerializationFactory.GetXmlSerializer();
                xmlSerializer.Serialize(model, memoryStream, null);

                memoryStream.Position = 0L;
                using (var xmlReader = XmlReader.Create(memoryStream))
                {
                    return XDocument.Load(xmlReader).ToString();
                }
            }
        }

        public static T FromXmlString<T>(this string xml)
            where T : ModelBase
        {
            Argument.IsNotNullOrWhitespace(() => xml);

            var xmlDocument = XDocument.Parse(xml);

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream))
                {
                    xmlDocument.Save(xmlWriter);
                }

                memoryStream.Position = 0L;

                var xmlSerializer = SerializationFactory.GetXmlSerializer();
                return (T)xmlSerializer.Deserialize(typeof(T), memoryStream, null);
            }
        }

        /// <summary>
        /// Creates a complex circular test model graph.
        /// </summary>
        /// <returns>A graph of circular test models.</returns>
        public static CircularTestModel CreateComplexCircularTestModelGraph()
        {
            var graph = new CircularTestModel();

            var innerElement = new CircularTestModel();
            innerElement.CircularModel = graph;

            graph.CircularModel = innerElement;

            return graph;
        }
    }
}
