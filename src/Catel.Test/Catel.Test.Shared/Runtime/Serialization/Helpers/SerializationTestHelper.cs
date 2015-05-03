// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Binary;

    public static class SerializationTestHelper
    {
        /// <summary>
        /// Serializes and deserializes using the specified serializer.
        /// </summary>
        /// <typeparam name="TModel">The type of the T model.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>System.Object.</returns>
        public static TModel SerializeAndDeserialize<TModel>(TModel model, IModelBaseSerializer serializer)
            where TModel : ModelBase
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream);

                memoryStream.Position = 0L;

                if (!(serializer is BinarySerializer))
                {
                    var streamReader = new StreamReader(memoryStream);
                    var streamAsText = streamReader.ReadToEnd();

                    Console.WriteLine(streamAsText);

                    memoryStream.Position = 0L;
                }

                return (TModel)serializer.Deserialize(typeof (TModel), memoryStream);
            }
        }

        public static string ToXmlString(this ModelBase model)
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