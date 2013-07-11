// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Logging;

    using Runtime.Serialization;
    using Catel.Reflection;

#if !NET
    using System.Reflection;
#endif

#if NET35
    using System.Security.Permissions;
#elif NET40 || NET45
    using System.Security;
#endif

#if NET
    using System.Runtime.Serialization.Formatters.Binary;
#elif NETFX_CORE
    using Windows.Storage.Streams;
#elif PCL
    // Not supported in Portable Class Library
#else
    using System.IO.IsolatedStorage;
#endif

#if NET
    [System.Xml.Serialization.XmlSchemaProvider("GetModelBaseXmlSchema")]
#endif
    public partial class ModelBase
    {
        #region Fields
#if NET
        private bool _isDeserializing;
#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is deserialized.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler Deserialized;
        #endregion

        #region IXmlSerializable Members
#if NET
        /// <summary>
        /// Gets XML schema for this class.
        /// <para />
        /// Implemented to support WCF serialization for all types deriving from this type.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        /// <returns>System.Xml.XmlQualifiedName.</returns>
        public static XmlQualifiedName GetModelBaseXmlSchema(XmlSchemaSet schemaSet)
        {
            return XmlSchemaManager.GetXmlSchema(typeof(ModelBase), schemaSet);
        }
#endif

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            // As requested by the documentation, we return null
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            var type = GetType();

            var settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            var newReader = XmlReader.Create(reader, settings);

            //if (!string.Equals(type.Name, newReader.LocalName, StringComparison.OrdinalIgnoreCase))
            //{
            //    if (!newReader.Read())
            //    {
            //        return;
            //    }
            //}

            bool isAtXmlRoot = string.IsNullOrEmpty(newReader.LocalName) || string.Equals(newReader.LocalName, "xml", StringComparison.OrdinalIgnoreCase);
            if (isAtXmlRoot)
            {
                newReader.MoveToContent();
            }

            // Read attributes
            if (newReader.MoveToFirstAttribute())
            {
                do
                {
                    if (PropertyDataManager.IsXmlAttributeNameMappedToProperty(type, newReader.LocalName))
                    {
                        var propertyName = PropertyDataManager.MapXmlAttributeNameToPropertyName(type, reader.LocalName);
                        ReadValueFromXmlNode(newReader, propertyName);
                    }
                } while (newReader.MoveToNextAttribute());
            }

            // This might be the node itself or a wrapping node (in case of web services), 
            // so check if that is true and step into child element
            if (string.IsNullOrEmpty(newReader.LocalName) || !PropertyDataManager.IsXmlElementNameMappedToProperty(type, newReader.LocalName))
            {
                newReader.Read();
                newReader.MoveToElement();
            }

            while (newReader.NodeType != XmlNodeType.EndElement)
            {
                if (string.IsNullOrEmpty(reader.LocalName))
                {
                    Log.Debug("reader.LocalName is null or empty, trying to skip current node");
                    reader.Skip();

                    if (string.IsNullOrEmpty(reader.LocalName))
                    {
                        Log.Warning("reader.LocalName is null or empty, cannot read empty xml tag");
                        continue;
                    }
                }

                if (PropertyDataManager.IsXmlElementNameMappedToProperty(type, reader.LocalName))
                {
                    var propertyName = PropertyDataManager.MapXmlElementNameToPropertyName(type, reader.LocalName);
                    ReadValueFromXmlNode(newReader, propertyName);
                }
                else
                {
                    newReader.Skip();
                }

                while (newReader.NodeType == XmlNodeType.Whitespace)
                {
                    newReader.Skip();
                }
            }

            FinishDeserialization();
        }

        /// <summary>
        /// Reads the value from the XML node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="reader"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <remarks>This method does not check whether the property exists. This is the responsibility of the caller.</remarks>
        private void ReadValueFromXmlNode(XmlReader reader, string propertyName)
        {
            Argument.IsNotNull("reader", reader);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), propertyName);

            object value = null;

            switch (reader.NodeType)
            {
                case XmlNodeType.Attribute:
                    value = GetObjectFromXmlAttribute(reader, propertyData);
                    break;

                case XmlNodeType.Element:
                    value = GetObjectFromXmlElement(reader, propertyName);
                    break;

                default:
                    string error = string.Format("Xml node type '{0}' with local name '{1}' is not supported", reader.NodeType, ObjectToStringHelper.ToString(reader.LocalName));
                    Log.Error(error);
                    throw new NotSupportedException(error);
            }

            if (value != null)
            {
                SetValue(propertyData, value, false, false);
            }
        }

        /// <summary>
        /// Gets the object from XML attribute.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyData">The property data.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlAttribute(XmlReader reader, PropertyData propertyData)
        {
            reader.ReadAttributeValue();
            var value = reader.Value;

            try
            {
                return StringToObjectHelper.ToRightType(propertyData.Type, value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to convert '{0}' to type '{1}', returning null", value, propertyData.Type.FullName);
                return null;
            }
        }

        /// <summary>
        /// Gets the object from XML element.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyName">Name of the property as known in the class.</param>
        /// <returns>Object or <c>null</c>.</returns>
        private object GetObjectFromXmlElement(XmlReader reader, string propertyName)
        {
            object value = null;
            string xmlName = reader.LocalName;
            var type = GetType();

            if (IsPropertyRegistered(propertyName))
            {
                try
                {
                    var propertyData = GetPropertyData(propertyName);
                    var propertyTypeToDeserialize = propertyData.Type;

                    var serializer = SerializationHelper.GetDataContractSerializer(type, propertyTypeToDeserialize, xmlName);

                    string attributeValue = reader.GetAttribute("type", "http://catel.codeplex.com");
                    if (!string.IsNullOrEmpty(attributeValue))
                    {
                        Log.Debug("Property type for property '{0}' is '{1}' but found type info that it should be deserialized as '{2}'",
                            propertyData.Name, propertyData.Type.FullName, attributeValue);

                        var actualTypeToDeserialize = (from t in serializer.KnownTypes
                                                       where t.FullName == attributeValue
                                                       select t).FirstOrDefault();
                        if (actualTypeToDeserialize != null)
                        {
                            serializer = SerializationHelper.GetDataContractSerializer(type, actualTypeToDeserialize, xmlName);
                        }
                        else
                        {
                            Log.Warning("Could not find type '{0}', falling back to original type '{1}'", attributeValue, propertyData.Type.FullName);
                        }
                    }

                    value = serializer.ReadObject(reader, false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to deserialize property '{0}' stored as '{1}', using default value", propertyName, xmlName);
                }
            }

            return value;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var type = GetType();

            string ns1 = writer.LookupPrefix("http://catel.codeplex.com");
            if (ns1 == null)
            {
                writer.WriteAttributeString("xmlns", "ctl", null, "http://catel.codeplex.com");
            }

            // 1st, write all attributes
            WriteXmlAttributes(writer, type);

            // 2nd, write all elements
            WriteXmlElements(writer, type);
        }

        /// <summary>
        /// Writes the XML attributes to the xml writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="type">The type.</param>
        private void WriteXmlAttributes(XmlWriter writer, Type type)
        {
            IEnumerable<KeyValuePair<string, object>> propertiesAsAttributes;

            lock (_propertyValuesLock)
            {
                propertiesAsAttributes = (from propertyValue in _propertyBag.GetAllProperties()
                                          where PropertyDataManager.IsPropertyNameMappedToXmlAttribute(type, propertyValue.Key)
                                          select propertyValue);
            }

            foreach (var propertyAsAttribute in propertiesAsAttributes)
            {
                var attributeName = PropertyDataManager.MapPropertyNameToXmlAttributeName(type, propertyAsAttribute.Key);
                writer.WriteAttributeString(attributeName, propertyAsAttribute.Value.ToString());
            }
        }

        /// <summary>
        /// Writes the XML elements to the xml writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="type">The type.</param>
        private void WriteXmlElements(XmlWriter writer, Type type)
        {
            IEnumerable<KeyValuePair<string, object>> propertiesAsElements;

            lock (_propertyValuesLock)
            {
                propertiesAsElements = (from propertyValue in _propertyBag.GetAllProperties()
                                        where PropertyDataManager.IsPropertyNameMappedToXmlElement(type, propertyValue.Key)
                                        select propertyValue);
            }

            foreach (var propertyAsElement in propertiesAsElements)
            {
                var propertyInfo = GetPropertyInfo(propertyAsElement.Key);
                if (propertyInfo == null)
                {
                    continue;
                }

                if (propertyAsElement.Value == null)
                {
                    continue;
                }

                string serializationPropertyName = propertyAsElement.Key;
                serializationPropertyName = PropertyDataManager.MapPropertyNameToXmlElementName(type, serializationPropertyName);

                var propertyType = propertyInfo.PropertyType;
                var propertyTypeToSerialize = propertyAsElement.Value.GetType();

                var serializer = SerializationHelper.GetDataContractSerializer(GetType(), propertyTypeToSerialize, serializationPropertyName, propertyAsElement.Value);

                if (propertyType != propertyTypeToSerialize)
                {
                    Log.Debug("Property type for property '{0}' is '{1}' but registered as '{2}', adding type info for deserialization",
                              propertyInfo.Name, propertyTypeToSerialize.FullName, propertyType.FullName);

                    serializer.WriteStartObject(writer, propertyAsElement.Value);

                    writer.WriteAttributeString("ctl", "type", null, propertyTypeToSerialize.FullName);

                    serializer.WriteObjectContent(writer, propertyAsElement.Value);

                    serializer.WriteEndObject(writer);
                }
                else
                {
                    serializer.WriteObject(writer, propertyAsElement.Value);
                }
            }
        }
        #endregion

        #region Loading
#if NET
        /// <summary>
        /// Loads the object from a file using binary formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileName">Filename of the file that contains the serialized data of this object.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(string fileName, bool enableRedirects = false)
            where T : class
        {
            return Load<T>(fileName, SerializationMode.Binary, enableRedirects);
        }
#elif NETFX_CORE
        /// <summary>
        /// Loads the object from a file using xml formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(IRandomAccessStream fileStream, bool enableRedirects = false)
            where T : class
        {
            return Load<T>(fileStream, SerializationMode.Xml, enableRedirects);
        }
#elif PCL
        // Not supported in Portable Class Library
#else
        /// <summary>
        /// Loads the object from a file using xml formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(IsolatedStorageFileStream fileStream, bool enableRedirects = false)
            where T : class
        {
            return Load<T>(fileStream, SerializationMode.Xml, enableRedirects);
        }
#endif

#if NET
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileName">Filename of the file that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(string fileName, SerializationMode mode, bool enableRedirects = false)
            where T : class
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open))
            {
                return Load<T>(stream, mode, enableRedirects);
            }
        }
#elif NETFX_CORE
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(IRandomAccessStream fileStream, SerializationMode mode, bool enableRedirects = false)
            where T : class
        {
            return Load<T>((Stream)fileStream, mode, enableRedirects);
        }
#elif PCL
        // Not supported in Portable Class Library
#else
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(IsolatedStorageFileStream fileStream, SerializationMode mode, bool enableRedirects = false)
            where T : class
        {
            return Load<T>((Stream)fileStream, mode, enableRedirects);
        }
#endif

        /// <summary>
        /// Loads the object from an XmlDocument object.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        public static T Load<T>(XDocument xmlDocument)
            where T : class
        {
            using (var memoryStream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(memoryStream))
                {
                    xmlDocument.Save(writer);
                }

                memoryStream.Position = 0L;

                return Load<T>(memoryStream, SerializationMode.Xml, false);
            }
        }

        /// <summary>
        /// Loads the object from a stream.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="bytes">The byte array.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(byte[] bytes, bool enableRedirects = false)
            where T : class
        {
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(bytes, 0, bytes.Length);

                memoryStream.Position = 0L;

#if NET
                return Load<T>(memoryStream, SerializationMode.Binary, enableRedirects);
#else
                return Load<T>(memoryStream, SerializationMode.Xml, enableRedirects);
#endif
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(Stream stream, bool enableRedirects = false)
            where T : class
        {
#if NET
            return Load<T>(stream, SerializationMode.Binary, enableRedirects);
#else
            return Load<T>(stream, SerializationMode.Xml, enableRedirects);
#endif
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode"/> to use.</param>
        /// <param name="enableRedirects">if set to <c>true</c>, redirects will be enabled.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load<T>(Stream stream, SerializationMode mode, bool enableRedirects = false)
            where T : class
        {
            object result = null;

            Log.Debug("Loading object '{0}' as '{1}'", typeof(T).Name, mode);

            switch (mode)
            {
#if NET
                case SerializationMode.Binary:
                    try
                    {
                        BinaryFormatter binaryFormatter = SerializationHelper.GetBinarySerializer(enableRedirects);

                        Log.Debug("Resetting stream position to 0");

                        stream.Position = 0L;

                        Log.Debug("Resetted stream position to 0");

                        Log.Debug("Deserializing binary stream for type '{0}'", typeof(T).Name);

                        result = binaryFormatter.Deserialize(stream);

                        Log.Debug("Deserialized binary stream");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to deserialize the binary object");
                    }

                    break;
#endif

                case SerializationMode.Xml:
                    using (var xmlReader = XmlReader.Create(stream))
                    {
#if NET
                        result = Activator.CreateInstance(typeof(T), true);
#elif NETFX_CORE || PCL
                        result = Activator.CreateInstance(typeof(T), true);
#else
                        result = typeof(T).InvokeMember(null, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, new object[] { });
#endif

                        var xmlSerializable = (IXmlSerializable)result;
                        xmlSerializable.ReadXml(xmlReader);
                    }

                    break;
            }

            Log.Debug("Loaded object");

            var resultAsModelBase = result as ModelBase;
            if (resultAsModelBase != null)
            {
                resultAsModelBase.Mode = mode;
            }

            return (T)result;
        }

#if NET
        /// <summary>
        /// Retrieves the actual data from the serialization info.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to get the data from.</param>
        /// <remarks>
        /// This method is called from the OnDeserialized method, thus all child objects
        /// are serialized and available at the time this method is called.
        /// <para />
        /// Only use this method to support older serialization techniques. When using this class
        /// for new objects, all serialization is handled automatically.
        /// </remarks>
        protected virtual void GetDataFromSerializationInfo(SerializationInfo info)
        {
            // Not implemented by default
        }

        /// <summary>
        /// Retrieves the actual data from the serialization info for the properties registered
        /// on this object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to get the data from.</param>
        protected void GetDataFromSerializationInfoInternal(SerializationInfo info)
        {
            if (info == null)
            {
                Log.Warning("Serialization info is null, no data to retrieve");
                return;
            }

            if (!IsDeserializedDataAvailable)
            {
                return;
            }

            if (IsDeserialized)
            {
                Log.Debug("Object '{0}' is already deserialized", GetType().Name);
                return;
            }

            // Try to get new style
            var newStyleProperties = (List<PropertyValue>)SerializationHelper.GetObject(info, "Properties", typeof(List<PropertyValue>), null);
            if (newStyleProperties == null)
            {
                // Failed to get new style, use old and convert it to new style
                var oldStyleProperties = (List<KeyValuePair<string, object>>)SerializationHelper.GetObject(info, "Properties",
                    typeof(List<KeyValuePair<string, object>>), new List<KeyValuePair<string, object>>());
                newStyleProperties = oldStyleProperties.Select(oldStyleProperty => new PropertyValue(oldStyleProperty)).ToList();
            }

            try
            {
                foreach (var property in newStyleProperties)
                {
                    if (IsPropertyRegistered(property.Name))
                    {
                        if (property.Value != null)
                        {
                            var propertyValueAsIDeserializationCallback = property.Value as IDeserializationCallback;
                            if ((propertyValueAsIDeserializationCallback != null) && (property.Value is ICollection))
                            {
                                // Call it since collections need this call to contain valid items
                                propertyValueAsIDeserializationCallback.OnDeserialization(this);
                            }
                        }

                        var propertyData = GetPropertyData(property.Name);
                        var deserializedValue = property.Value;
                        var actualValue = GetPropertyValueForDeserialization(propertyData, deserializedValue);

                        SetValue(propertyData, actualValue, false, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deserializing object '{0}'", GetType().Name);
            }

            // Allow developers to support backwards compatibility
            GetDataFromSerializationInfo(info);

            FinishDeserialization();
        }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
#if NET35
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
#else
        [SecurityCritical]
#endif
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            List<PropertyValue> properties;

            lock (_propertyValuesLock)
            {
                properties = ConvertDictionaryToListAndExcludeNonSerializableObjects(_propertyBag.GetAllProperties());
            }

            info.AddValue("Properties", properties, properties.GetType());
        }

        /// <summary>
        /// Invoked when the deserialization of the object graph is complete.
        /// </summary>
        /// <param name="context">The <see cref="StreamingContext"/>..</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _isDeserializing = false;

            //Log.Debug("Received OnDeserialized event for '{0}'", GetType().Name);

            IsDeserializedDataAvailable = true;

            GetDataFromSerializationInfoInternal(_serializationInfo);
        }

        /// <summary>
        /// Invoked when the deserialization of the object graph is complete.
        /// </summary>
        /// <param name="sender">The object that has started deserializing.</param>
        /// <remarks>
        /// When this method is called from within this object, it is invoked from a
        /// static method so the sender will be <c>null</c>.
        /// </remarks>
        public void OnDeserialization(object sender)
        {
            // Prevent stack overflows in very rare cases where the OnDeserialization is called when it is not really needed
            if (_isDeserializing)
            {
                return;
            }

            _isDeserializing = true;

            //Log.Debug("Received OnDeserialization event for '{0}'", GetType().Name);

            try
            {
                lock (_propertyValuesLock)
                {
                    foreach (var property in _propertyBag.GetAllProperties())
                    {
                        CallOnDeserializationCallback(property.Value);

                        var collection = property.Value as ICollection;
                        if (collection != null)
                        {
                            foreach (object item in collection)
                            {
                                CallOnDeserializationCallback(item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.Warning("Failed to call IDeserializationCallback.OnDeserialization for child objects");
            }
        }

        /// <summary>
        /// Calls the <see cref="IDeserializationCallback.OnDeserialization"/> method on the object if possible.
        /// </summary>
        /// <param name="obj">The object that has finished deserializing.</param>
        private void CallOnDeserializationCallback(object obj)
        {
            if (obj == null)
            {
                return;
            }

            var objAsIDeserializationCallback = obj as IDeserializationCallback;
            if (objAsIDeserializationCallback != null)
            {
                objAsIDeserializationCallback.OnDeserialization(this);
            }
        }
#endif

#if !NET_
        /// <summary>
        /// Serializes all the properties that are serializable on this object.
        /// </summary>
        /// <returns>Byte array containing the serialized properties.</returns>
        internal byte[] SerializeProperties()
        {
            using (var stream = new MemoryStream())
            {
                // Xml backup, create serializer without using the cache since the dictionary is used for every object, and
                // we need a "this" object specific dictionary.

                List<PropertyValue> objectToSerialize = null;

                lock (_propertyValuesLock)
                {
                    objectToSerialize = ConvertDictionaryToListAndExcludeNonSerializableObjects(_propertyBag.GetAllProperties());
                }

                DataContractSerializer serializer = SerializationHelper.GetDataContractSerializer(GetType(), objectToSerialize.GetType(),
                    "internal", objectToSerialize, false);

                serializer.WriteObject(stream, objectToSerialize);

#if DEBUG
                stream.Position = 0L;
                var debugReader = new StreamReader(stream);
                string content = debugReader.ReadToEnd();

                System.Diagnostics.Debug.WriteLine(content);
#endif

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes all the properties that are serializable on this object.
        /// </summary>
        /// <param name="data">The data containing the serialized properties.</param>
        /// <remarks>
        /// This method first tries to deserialize using <see cref="ConformanceLevel.Document"/>. If that
        /// does not succeed, it tries <see cref="ConformanceLevel.Fragment"/>. If that does not succeed, 
        /// it finally tries <see cref="ConformanceLevel.Auto"/>. If that fails, <c>null</c> will be returned.
        /// </remarks>
        /// <returns>List of deserialized properties.</returns>
        internal List<PropertyValue> DeserializeProperties(byte[] data)
        {
            try
            {
                return DeserializeProperties(data, ConformanceLevel.Document);
            }
            catch (Exception)
            {
            }

            try
            {
                return DeserializeProperties(data, ConformanceLevel.Fragment);
            }
            catch (Exception)
            {
            }

            try
            {
                return DeserializeProperties(data, ConformanceLevel.Auto);
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Deserializes all the properties that are serializable on this object using the specified <see cref="ConformanceLevel"/>.
        /// </summary>
        /// <param name="data">The data containing the serialized properties.</param>
        /// <param name="conformanceLevel">The conformance level.</param>
        /// <returns>List of deserialized properties.</returns>
        private List<PropertyValue> DeserializeProperties(byte[] data, ConformanceLevel conformanceLevel)
        {
            using (var stream = new MemoryStream(data))
            {
#if DEBUG
                long initialStreamPos = stream.Position;
                var debugReader = new StreamReader(stream);
                string content = debugReader.ReadToEnd();
                stream.Position = initialStreamPos;

                System.Diagnostics.Debug.WriteLine(content);
#endif

                // Make sure to include all properties of the view model, the types must be known
                var additionalKnownTypes = new List<Type>();
                additionalKnownTypes.Add(GetType());
                foreach (var property in PropertyDataManager.GetProperties(GetType()))
                {
                    if (!additionalKnownTypes.Contains(property.Value.Type))
                    {
                        additionalKnownTypes.Add(property.Value.Type);
                    }
                }

                DataContractSerializer serializer = SerializationHelper.GetDataContractSerializer(GetType(), InternalSerializationType, "internal",
                    additionalKnownTypes, false);

                // Use a custom reader, required to succeed
                var settings = new XmlReaderSettings();
                settings.ConformanceLevel = conformanceLevel;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;

                var reader = XmlReader.Create(stream, settings);
                return (List<PropertyValue>)serializer.ReadObject(reader, false);
            }
        }
#endif

        /// <summary>
        /// Converts a dictionary to a list for serialization purposes.
        /// </summary>
        /// <param name="dictionary">Dictionary to convert.</param>
        /// <param name="propertiesToIgnore">The properties to ignore.</param>
        /// <returns>List that contains all the values of the dictionary.</returns>
        /// <remarks>
        /// This method is required because Dictionary can't be serialized.
        /// </remarks>
        internal List<PropertyValue> ConvertDictionaryToListAndExcludeNonSerializableObjects(Dictionary<string, object> dictionary, params string[] propertiesToIgnore)
        {
            var propertiesToIgnoreHashSet = new HashSet<string>(propertiesToIgnore);

            var listToSerialize = new List<PropertyValue>();

            foreach (var dictionaryItem in dictionary)
            {
                var propertyData = GetPropertyData(dictionaryItem.Key);
                if (!propertyData.IsSerializable)
                {
                    Log.Warning("Property '{0}' is not serializable, so will be excluded from the serialization", propertyData.Name);
                    continue;
                }

                if (!propertyData.IncludeInSerialization)
                {
                    Log.Debug("Property '{0}' is flagged to be excluded from serialization", propertyData.Name);
                    continue;
                }

                if (propertiesToIgnoreHashSet.Contains(propertyData.Name))
                {
                    Log.Info("Property '{0}' is being ignored for serialization", propertyData.Name);
                    continue;
                }

#if NET
                var collection = dictionaryItem.Value as ICollection;
                if (collection != null)
                {
                    bool validCollection = true;

                    foreach (var item in collection)
                    {
                        if ((item != null) && (item.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length == 0))
                        {
                            validCollection = false;
                            break;
                        }
                    }

                    if (!validCollection)
                    {
                        Log.Debug("Property '{0}' is a collection containing non-serializable objects, so will be excluded from serialization", propertyData.Name);
                        continue;
                    }
                }
#endif

                Log.Debug("Adding property '{0}' to list of objects to serialize", propertyData.Name);

                var actualValue = dictionaryItem.Value;
                var serializingValue = GetPropertyValueForSerialization(propertyData, actualValue);

                listToSerialize.Add(new PropertyValue(propertyData.Name, serializingValue));
            }

            return listToSerialize;
        }

        /// <summary>
        /// Converts a list to a dictionary for serialization purposes.
        /// </summary>
        /// <param name="list">List to convert.</param>
        /// <returns>
        /// Dictionary that contains all the values of the list.
        /// </returns>
        internal Dictionary<string, object> ConvertListToDictionary(IEnumerable<PropertyValue> list)
        {
            return ConvertListToDictionary(GetType(), list);
        }

        /// <summary>
        /// Converts a list to a dictionary for serialization purposes.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="list">List to convert.</param>
        /// <returns>
        /// Dictionary that contains all the values of the list.
        /// </returns>
        internal static Dictionary<string, object> ConvertListToDictionary(Type type, IEnumerable<PropertyValue> list)
        {
            var result = new Dictionary<string, object>();

            foreach (var propertyValue in list)
            {
                if (IsPropertyRegistered(type, propertyValue.Name))
                {
#if NET
                    if (propertyValue.Value != null)
                    {
                        var propertyValueAsIDeserializationCallback = propertyValue.Value as IDeserializationCallback;
                        if ((propertyValueAsIDeserializationCallback != null) && (propertyValue.Value is ICollection))
                        {
                            // We are allowed to pass null, see remarks in OnDeserialization defined in this class
                            propertyValueAsIDeserializationCallback.OnDeserialization(null);
                        }
                    }
#endif

                    // Store the value (since deserialized values always override default values)
                    result[propertyValue.Name] = propertyValue.Value;
                }
            }

            return result;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the property value to use during serialization.
        /// <para />
        /// This method allows the customization of property value serialization.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="propertyValue">The actual value of the property.</param>
        /// <returns>The value to serialize.</returns>
        protected virtual object GetPropertyValueForSerialization(PropertyData property, object propertyValue)
        {
            return propertyValue;
        }

        /// <summary>
        /// Gets the property value to use during deserialization.
        /// <para />
        /// This method allows the customization of the property value deserialization.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="serializedValue">The value that was serialized.</param>
        /// <returns>The value to deserialize.</returns>
        protected virtual object GetPropertyValueForDeserialization(PropertyData property, object serializedValue)
        {
            return serializedValue;
        }
        #endregion
    }
}