// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;

    using Data;

    /// <summary>
    /// The serialization context used to serialize and deserialize models.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class SerializationContext<TContext> : ISerializationContext<TContext>
        where TContext : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext{TContext}"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is <c>null</c>.</exception>
        public SerializationContext(ModelBase model, TContext context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            Model = model;
            Context = context;
        }

        /// <summary>
        /// Gets the model that needs serialization or deserialization.
        /// </summary>
        /// <value>The model.</value>
        public ModelBase Model { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public TContext Context { get; private set; }
    }
}