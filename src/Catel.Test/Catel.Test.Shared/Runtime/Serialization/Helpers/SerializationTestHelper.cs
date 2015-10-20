// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Binary;
    using System.Diagnostics;
    using Catel.IoC;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;

    public static class SerializationTestHelper
    {
        private static readonly Dictionary<XmlSerializerOptimalizationMode, IXmlSerializer> _xmlSerializers = new Dictionary<XmlSerializerOptimalizationMode, IXmlSerializer>(); 

        static SerializationTestHelper()
        {
            var typeFactory = TypeFactory.Default;

            foreach (var value in Enum<XmlSerializerOptimalizationMode>.GetValues())
            {
                var xmlSerializer = typeFactory.CreateInstance<XmlSerializer>();
                xmlSerializer.OptimalizationMode = value;

                _xmlSerializers[value] = xmlSerializer;
            }
        }

        public static IXmlSerializer GetXmlSerializer(XmlSerializerOptimalizationMode optimalizationMode)
        {
            return _xmlSerializers[optimalizationMode];
        }

        public static IBinarySerializer GetBinarySerializer()
        {
            return SerializationFactory.GetBinarySerializer();
        }

        public static IJsonSerializer GetJsonSerializer()
        {
            return new JsonSerializer(new SerializationManager(), TypeFactory.Default, new ObjectAdapter());
        }

        /// <summary>
        /// Serializes and deserializes using the specified serializer.
        /// </summary>
        /// <typeparam name="TModel">The type of the T model.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>System.Object.</returns>
        public static TModel SerializeAndDeserialize<TModel>(TModel model, ISerializer serializer)
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream);

                memoryStream.Position = 0L;

                if (Debugger.IsAttached)
                {
                    if (!(serializer is BinarySerializer))
                    {
                        var streamReader = new StreamReader(memoryStream);
                        var streamAsText = streamReader.ReadToEnd();

                        Console.WriteLine(streamAsText);

                        memoryStream.Position = 0L;
                    }
                }

                return (TModel)serializer.Deserialize(typeof(TModel), memoryStream);
            }
        }

        public static string ToXmlString(this object model)
        {
            Argument.IsNotNull(() => model);

            using (var memoryStream = new MemoryStream())
            {
                var xmlSerializer = SerializationFactory.GetXmlSerializer();
                xmlSerializer.Serialize(model, memoryStream);

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
                return (T)xmlSerializer.Deserialize(typeof(T), memoryStream);
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