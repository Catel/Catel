namespace Catel.Data
{
    using System;
    using System.IO;
    using Logging;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// Abstract class that makes the <see cref="ModelBase" /> serializable.
    /// </summary>
    /// <typeparam name="T">Type that the class should hold (same as the defined type).</typeparam>
    [Serializable]
    public abstract class SavableModelBase<T> : ModelBase, ISavableModel
        where T : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="SavableModelBase{T}"/> class.
        /// </summary>
        protected SavableModelBase()
        {
        }

        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        public void Save(Stream stream, ISerializer serializer, ISerializationConfiguration? configuration = null)
        {
            serializer.Serialize(this, stream, configuration);

            this.ClearIsDirtyOnAllChildren();
        }

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
        public static T? Load(Stream stream, ISerializer serializer, ISerializationConfiguration? configuration = null)
        {
            return (T?)Load(typeof(T), stream, serializer, configuration);
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
        public static IModel? Load(Type type, Stream stream, ISerializer serializer, ISerializationConfiguration? configuration = null)
        {
            var result = serializer.Deserialize(type, stream, configuration);
            return (IModel?)result;
        }
    }
}
