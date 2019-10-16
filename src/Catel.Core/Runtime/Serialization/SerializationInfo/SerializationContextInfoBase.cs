namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Base class for serialization context info implementations.
    /// </summary>
    /// <typeparam name="TSerializationContextInfo">The type of the serialization context information.</typeparam>
    /// <seealso cref="Catel.Runtime.Serialization.ISerializationContextInfo" />
    /// <seealso cref="Catel.Runtime.Serialization.ISerializationContextContainer" />
    public abstract class SerializationContextInfoBase<TSerializationContextInfo> : ISerializationContextInfo, ISerializationContextContainer
        where TSerializationContextInfo : class, ISerializationContextInfo
    {
        protected SerializationContextInfoBase()
        {
        }

        /// <summary>
        /// Sets the related serialization context.
        /// </summary>
        /// <typeparam name="T">The serialization context info type.</typeparam>
        /// <param name="serializationContext">The serialization context.</param>
        void ISerializationContextContainer.SetSerializationContext<T>(ISerializationContext<T> serializationContext)
        {
            var context = (ISerializationContext<TSerializationContextInfo>)serializationContext;
            if (context != null)
            {
                Context = context;
                OnContextUpdated(context);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the context should auto generate graph ids for new object instances.
        /// </summary>
        /// <param name="context">The current serialization context.</param>
        /// <returns><c>true</c> if graph ids should automatically be generated, <c>false</c> if they should be registered manually.</returns>
        public virtual bool ShouldAutoGenerateGraphIds(ISerializationContext context)
        {
            return context.ContextMode == SerializationContextMode.Serialization;
        }

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        /// <value>
        /// The parent context.
        /// </value>
        public ISerializationContext<TSerializationContextInfo> Context { get; private set; }

        /// <summary>
        /// Called when the <see cref="Context"/> is updated to a value other than <c>null</c>.
        /// </summary>
        protected virtual void OnContextUpdated(ISerializationContext<TSerializationContextInfo> context)
        {
        }
    }
}
