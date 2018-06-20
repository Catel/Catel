namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Supports setting the related serialization context.
    /// </summary>
    public interface ISerializationContextContainer
    {
        /// <summary>
        /// Sets the related serialization context.
        /// </summary>
        /// <typeparam name="T">The serialization context info type.</typeparam>
        /// <param name="serializationContext">The serialization context.</param>
        void SetSerializationContext<T>(ISerializationContext<T> serializationContext)
            where T : class, ISerializationContextInfo;
    }
}
