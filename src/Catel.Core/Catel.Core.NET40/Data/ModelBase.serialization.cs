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
        #region Events
        /// <summary>
        /// Occurs when the object is deserialized.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler Deserialized;
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
        protected internal virtual object GetPropertyValueForSerialization(PropertyData property, object propertyValue)
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
        protected internal virtual object GetPropertyValueForDeserialization(PropertyData property, object serializedValue)
        {
            return serializedValue;
        }
        #endregion
    }
}