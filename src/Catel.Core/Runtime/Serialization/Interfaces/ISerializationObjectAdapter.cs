﻿namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Adapter to interact with objects.
    /// </summary>
    public interface ISerializationObjectAdapter
    {
        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="modelInfo">The model information.</param>
        /// <returns>MemberValue.</returns>
        MemberValue? GetMemberValue(object model, string memberName, SerializationModelInfo modelInfo);

        /// <summary>
        /// Sets the member value.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="member">The member.</param>
        /// <param name="modelInfo">The model information.</param>
        void SetMemberValue(object model, MemberValue member, SerializationModelInfo modelInfo);
    }
}
