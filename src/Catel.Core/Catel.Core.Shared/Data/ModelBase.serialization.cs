// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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

    public partial class ModelBase
    {
        #region Fields
#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<EventArgs> _deserialized;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is deserialized.
        /// </summary>
        event EventHandler<EventArgs> IModelSerialization.Deserialized
        {
            add { _deserialized += value; }
            remove { _deserialized -= value; }
        }
        #endregion

        #region Loading
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
                using (var writer = XmlWriter.Create(memoryStream))
                {
                    xmlDocument.Save(writer);
                }

                memoryStream.Position = 0L;

                return Load<T>(memoryStream, SerializationMode.Xml);
            }
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <typeparam name="T">Type of the object that should be loaded.</typeparam>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.</remarks>
        public static T Load<T>(Stream stream, SerializationMode mode)
            where T : class
        {
            return (T)Load(typeof(T), stream, mode);
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="mode"><see cref="SerializationMode" /> to use.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.</remarks>
        public static IModel Load(Type type, Stream stream, SerializationMode mode)
        {
            Argument.IsNotNull("type", type);

            object result = null;

            Log.Debug("Loading object '{0}' as '{1}'", type.Name, mode);

            switch (mode)
            {
#if NET
                case SerializationMode.Binary:
                    try
                    {
                        var binarySerializer = SerializationFactory.GetBinarySerializer();
                        result = binarySerializer.Deserialize(type, stream);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to deserialize the binary object");
                    }

                    break;
#endif

                case SerializationMode.Xml:
                    try
                    {
                        var xmlSerializer = SerializationFactory.GetXmlSerializer();
                        result = xmlSerializer.Deserialize(type, stream);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to deserialize the binary object");
                    }
                    break;
            }

            Log.Debug("Loaded object");

            var resultAsModelBase = result as ISavableModel;
            if (resultAsModelBase != null)
            {
                resultAsModelBase.Mode = mode;
            }

            return result as IModel;
        }
        #endregion
    }
}