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
        public SerializationContextInfoBase()
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
