namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Additional features for serializable objects.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Starts the serialization.
        /// </summary>
        void StartSerialization();

        /// <summary>
        /// Finishes the serialization.
        /// </summary>
        void FinishSerialization();

        /// <summary>
        /// Starts the deserialization.
        /// </summary>
        void StartDeserialization();

        /// <summary>
        /// Finishes the deserialization.
        /// </summary>
        void FinishDeserialization();
    }
}
