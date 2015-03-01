// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifier.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using Catel.Data;

    /// <summary>
    /// Allows modifications for a specific model for every supported serializer.
    /// </summary>
    public abstract class SerializerModifierBase : ISerializerModifier
    {
        /// <summary>
        /// Determines whether the specified member should be ignored.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the property should be ignored, <c>false</c> otherwise.</returns>
        public virtual bool ShouldIgnoreMember(ISerializationContext context, IModel model, MemberValue memberValue)
        {
            return false;
        }

        /// <summary>
        /// Called when the object is about to be serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnSerializing(ISerializationContext context, IModel model)
        {
        }

        /// <summary>
        /// Allows the customization of the provided <see cref="MemberValue"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        public virtual void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Called when the object is serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnSerialized(ISerializationContext context, IModel model)
        {
        }

        /// <summary>
        /// Called when the object is about to be deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnDeserializing(ISerializationContext context, IModel model)
        {
        }

        /// <summary>
        /// Allows the customization of the provided <see cref="MemberValue"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The final <see cref="MemberValue"/> that will be deserialized.</returns>
        public virtual void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnDeserialized(ISerializationContext context, IModel model)
        {
        }
    }
}