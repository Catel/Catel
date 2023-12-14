namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Enum representing all member group.
    /// </summary>
    public enum SerializationMemberGroup
    {
        /// <summary>
        /// The member is a catel property.
        /// </summary>
        CatelProperty,

        /// <summary>
        /// The member is a regular property.
        /// </summary>
        RegularProperty,

        /// <summary>
        /// The member is a field.
        /// </summary>
        Field,

        /// <summary>
        /// The member is a simple root object, must be handled by external serializer immediately.
        /// </summary>
        SimpleRootObject,

        /// <summary>
        /// The member is a collection.
        /// </summary>
        Collection,

        /// <summary>
        /// The member is a dictionary.
        /// </summary>
        Dictionary
    }
}
