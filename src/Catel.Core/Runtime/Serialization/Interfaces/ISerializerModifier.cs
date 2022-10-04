namespace Catel.Runtime.Serialization
{
    using Catel.Data;

    /// <summary>
    /// Allows modifications for a specific model for every supported serializer.
    /// </summary>
    public interface ISerializerModifier
    {
        /// <summary>
        /// Returns whether the serializer should serialize this model as a collection.
        /// </summary>
        /// <returns><c>true</c> if the model should be serialized as a collection, <c>false</c> if not. Return <c>null</c> if the serializer should decide automatically.</returns>
        bool? ShouldSerializeAsCollection();

        /// <summary>
        /// Returns whether the serializer should serialize this model as a dictionary.
        /// </summary>
        /// <returns><c>true</c> if the model should be serialized as a dictionary, <c>false</c> if not. Return <c>null</c> if the serializer should decide automatically.</returns>
        bool? ShouldSerializeAsDictionary();

        /// <summary>
        /// Returns whether the serializer should serialize the member using <c>ToString(IFormatProvider)</c> and <c>Parse(string, IFormatProvider)</c>.
        /// </summary>
        /// <returns><c>true</c> if the member should be serialized using parse, <c>false</c> if not. Return <c>null</c> if the serializer should decide automatically.</returns>
        bool? ShouldSerializeMemberUsingParse(MemberValue memberValue);

        /// <summary>
        /// Returns whether the serializer should serialize the enum member using <c>ToString()</c>.
        /// </summary>
        /// <param name="memberValue"></param>
        /// <returns></returns>
        bool? ShouldSerializeEnumMemberUsingToString(MemberValue memberValue);

        /// <summary>
        /// Determines whether the specified member should be ignored.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the property should be ignored, <c>false</c> otherwise.</returns>
        bool ShouldIgnoreMember(ISerializationContext context, object model, MemberValue memberValue);

        /// <summary>
        /// Called when the object is about to be serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        void OnSerializing(ISerializationContext context, object model);

        /// <summary>
        /// Allows the customization of the provided <see cref="MemberValue"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        void SerializeMember(ISerializationContext context, MemberValue memberValue);

        /// <summary>
        /// Called when the object is serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        void OnSerialized(ISerializationContext context, object model);

        /// <summary>
        /// Called when the object is about to be deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        void OnDeserializing(ISerializationContext context, object model);

        /// <summary>
        /// Allows the customization of the provided <see cref="MemberValue"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        void DeserializeMember(ISerializationContext context, MemberValue memberValue);

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        void OnDeserialized(ISerializationContext context, object model);
    }
}
