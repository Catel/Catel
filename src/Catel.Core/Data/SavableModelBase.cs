// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SavableModelBaseBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Logging;
    using Catel.Runtime.Serialization;

#if NET || NETSTANDARD
    using System.Runtime.Serialization;
#elif NETFX_CORE
    using Windows.Storage.Streams;
#else
    using System.IO.IsolatedStorage;
#endif

    /// <summary>
    /// Abstract class that makes the <see cref="ModelBase" /> serializable.
    /// </summary>
    /// <typeparam name="T">Type that the class should hold (same as the defined type).</typeparam>
#if NET || NETSTANDARD
    [Serializable]
#endif
    public abstract class SavableModelBase<T> : ModelBase, ISavableModel
        where T : class
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
#if NET || NETSTANDARD
        [field: NonSerialized]
#endif
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SavableModelBase{T}"/> class.
        /// </summary>
        protected SavableModelBase()
        {
        }

#if NET || NETSTANDARD
        /// <summary>
        /// Initializes a new instance of the <see cref="SavableModelBase{T}"/> class.
        /// </summary>
        /// <param name="info">SerializationInfo object, null if this is the first time construction.</param>
        /// <param name="context">StreamingContext object, simple pass a default new StreamingContext() if this is the first time construction.</param>
        /// <remarks>
        /// Call this method, even when constructing the object for the first time (thus not deserializing).
        /// </remarks>
        protected SavableModelBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        #endregion

        #region Methods
        #region Saving
#if NET || NETSTANDARD || XAMARIN
        // No overloads required
#elif NETFX_CORE
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(IRandomAccessStream fileStream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Save(fileStream.AsStream(), null);
        }
#else
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(IsolatedStorageFileStream fileStream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Save((Stream)fileStream, null);
        }
#endif

        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(Stream stream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("serializer", serializer);

            serializer.Serialize(this, stream, configuration);

            this.ClearIsDirtyOnAllChilds();
        }
        #endregion

        #region Loading
#if NETFX_CORE
        /// <summary>
        /// Loads the object from a file using a specific formatting.
        /// </summary>
        /// <param name="fileStream">File stream of the file that contains the serialized data of this object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.</returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load(IRandomAccessStream fileStream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            return Load((Stream)fileStream, serializer, configuration);
        }
#endif

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static T Load(Stream stream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            return (T)Load(typeof(T), stream, serializer, configuration);
        }

        /// <summary>
        /// Loads the object from a stream using a specific formatting.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="stream">Stream that contains the serialized data of this object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// Deserialized instance of the object. If the deserialization fails, <c>null</c> is returned.
        /// </returns>
        /// <remarks>
        /// When enableRedirects is enabled, loading will take more time. Only set
        /// the parameter to <c>true</c> when the deserialization without redirects fails.
        /// </remarks>
        public static IModel Load(Type type, Stream stream, ISerializer serializer, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("serializer", serializer);

            var result = serializer.Deserialize(type, stream, configuration);
            return result as IModel;
        }
        #endregion
        #endregion
    }
}
