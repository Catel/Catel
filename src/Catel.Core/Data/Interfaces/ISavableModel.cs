namespace Catel.Data
{
    using System.IO;
    using Runtime.Serialization;

    /// <summary>
    /// ISavableDataObjectBase that defines the additional methods to save a <see cref="IModel" /> object.
    /// </summary>
    public interface ISavableModel : IModel
    {
        /// <summary>
        /// Saves the object to a stream using a specific formatting.
        /// </summary>
        /// <param name="stream">Stream that will contain the serialized data of this object.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="configuration">The configuration.</param>
        void Save(Stream stream, ISerializer serializer, ISerializationConfiguration? configuration = null);
    }
}
