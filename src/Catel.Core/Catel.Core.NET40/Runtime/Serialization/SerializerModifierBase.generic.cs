// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerModifier.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using Catel.Data;

    /// <summary>
    /// Allows modifications for a specific model for every supported serializer.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public abstract class SerializerModifierBase<TModel> : SerializerModifierBase
        where TModel : IModel
    {
        /// <summary>
        /// Called when the object is about to be serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnSerializing(ISerializationContext context, TModel model)
        {
            
        }

        /// <summary>
        /// Called when the object is serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnSerialized(ISerializationContext context, TModel model)
        {

        }

        /// <summary>
        /// Called when the object is about to be deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnDeserializing(ISerializationContext context, TModel model)
        {

        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public virtual void OnDeserialized(ISerializationContext context, TModel model)
        {

        }
    }
}