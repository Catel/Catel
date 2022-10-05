namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Fast serialization interaction. By default the serialization engine uses reflection to get and set values. To improve
    /// performance, once can implement this interface.
    /// </summary>
    public interface IFieldSerializable
    {
        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is retrieved successfully; otherwise, <c>false</c>.</returns>
        bool GetFieldValue<T>(string fieldName, ref T value);

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is set successfully; otherwise, <c>false</c>.</returns>
        bool SetFieldValue<T>(string fieldName, T value);
    }
}
