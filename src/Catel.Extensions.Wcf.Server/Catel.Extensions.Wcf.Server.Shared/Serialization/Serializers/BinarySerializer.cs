// --------------------------------------------------------------------------------------------------------------------
//  <copyright file="BinarySerializer.cs" Company = "Catel development team">
//    Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
//  </copyright> 
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Catel.IO;

namespace Catel.ServiceModel.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class BinarySerializer : XmlObjectSerializer
    {
        #region Constants

        /// <summary>
        /// The local name
        /// </summary>
        private const string LocalName = "BinaryEncodedObject";

        #endregion

        #region Fields

        /// <summary>
        /// The type
        /// </summary>
        private readonly Type _type;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinarySerializer" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public BinarySerializer(Type type)
        {
            Argument.IsNotNull("type", type);

            _type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a value that specifies whether the <see cref="T:System.Xml.XmlDictionaryReader" /> is positioned over an XML element that can be read.
        /// </summary>
        /// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML stream or document.</param>
        /// <returns>
        /// true if the reader can read the data; otherwise, false.
        /// </returns>
        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            Argument.IsNotNull("reader", reader);

            return reader.LocalName == LocalName;
        }

        /// <summary>
        /// Reads the XML stream or document with an <see cref="T:System.Xml.XmlDictionaryReader" /> and returns the deserialized object; it also enables you to specify whether the serializer can read the data before attempting to read it.
        /// </summary>
        /// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML document.</param>
        /// <param name="verifyObjectName">true to check whether the enclosing XML element name and namespace correspond to the root name and root namespace; otherwise, false to skip the verification.</param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            Argument.IsNotNull("reader", reader);

            var memoryStream = new MemoryStream(reader.ReadElementContentAsBase64());
            return BinarySerializerHelper.DiscoverAndDeSerialize(memoryStream, _type);
        }

        /// <summary>
        /// Writes the end of the object data as a closing XML element to the XML document or stream with an <see cref="T:System.Xml.XmlDictionaryWriter" />.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document or stream.</param>
        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            Argument.IsNotNull("writer", writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes only the content of the object to the XML document or stream using the specified <see cref="T:System.Xml.XmlDictionaryWriter" />.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document or stream.</param>
        /// <param name="graph">The object that contains the content to write.</param>
        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            Argument.IsNotNull("writer", writer);

            var memoryStream = new MemoryStream();
            BinarySerializerHelper.DiscoverAndSerialize(memoryStream, graph, _type);
            var bytes = memoryStream.ToByteArray();
            writer.WriteBase64(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the start of the object's data as an opening XML element using the specified <see cref="T:System.Xml.XmlDictionaryWriter" />.
        /// </summary>
        /// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document.</param>
        /// <param name="graph">The object to serialize.</param>
        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            Argument.IsNotNull("writer", writer);

            writer.WriteStartElement(LocalName);
        }

        #endregion
    }
}