// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISavableDataObjectBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System.IO;
    using Runtime.Serialization;

#if NETFX_CORE
    using Windows.Storage.Streams;
#endif

    /// <summary>
    /// ISavableDataObjectBase that defines the additional methods to save a <see cref="IModel" /> object.
    /// </summary>
    public interface ISavableModel : IModel
    {
        #region Methods
#if NET || XAMARIN
        // No overloads required
#elif NETFX_CORE
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        void Save(IRandomAccessStream fileStream, ISerializer serializer, ISerializationConfiguration configuration = null);
#elif PCL
        // Not supported in Portable Class Library
#else
        /// <summary>
        /// Saves the object to an isolated storage file stream using the default formatting.
        /// </summary>
        /// <param name="fileStream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        void Save(IsolatedStorageFileStream fileStream, ISerializer serializer, ISerializationConfiguration configuration = null);
#endif

        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        void Save(Stream stream, ISerializer serializer, ISerializationConfiguration configuration = null);
        #endregion
    }
}