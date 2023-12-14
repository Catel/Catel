namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// The mode in which a context is being used.
    /// </summary>
    public enum SerializationContextMode 
    {
        /// <summary>
        /// The context is being used for serialization.
        /// </summary>
        Serialization,

        /// <summary>
        /// The context is being used for deserialization.
        /// </summary>
        Deserialization
    }
}
