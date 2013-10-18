// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Threading;
    using Catel.Scoping;
    using Data;

    /// <summary>
    /// The serialization context used to serialize and deserialize models.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public class SerializationContext<TContext> : ISerializationContext<TContext>
        where TContext : class
    {
        private ScopeManager<ReferenceManager> _referenceManagerScopeManager; 

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext{TContext}" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        public SerializationContext(ModelBase model, TContext context, SerializationContextMode contextMode)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            Model = model;
            ModelType = model.GetType();
            Context = context;
            ContextMode = contextMode;

            var scopeName = SerializationContextHelper.GetSerializationReferenceManagerScopeName();
            _referenceManagerScopeManager = ScopeManager<ReferenceManager>.GetScopeManager(scopeName);

            ReferenceManager = _referenceManagerScopeManager.ScopeObject;
        }

        /// <summary>
        /// Gets the model that needs serialization or deserialization.
        /// </summary>
        /// <value>The model.</value>
        public ModelBase Model { get; private set; }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the context mode.
        /// </summary>
        /// <value>The context mode.</value>
        public SerializationContextMode ContextMode { get; private set; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public TContext Context { get; private set; }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public ReferenceManager ReferenceManager { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_referenceManagerScopeManager != null)
            {
                _referenceManagerScopeManager.Dispose();
                _referenceManagerScopeManager = null;
            }
        }
    }
}