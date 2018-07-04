namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Serialization context scope managing all the objects required for a context.
    /// </summary>
    public class SerializationContextScope<TSerializationContextInfo>
        where TSerializationContextInfo : class, ISerializationContextInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContextScope{TSerializationContextInfo}"/> class.
        /// </summary>
        public SerializationContextScope()
        {
            ReferenceManager = new ReferenceManager();
            TypeStack = new Stack<Type>();
            Contexts = new Stack<ISerializationContext<TSerializationContextInfo>>();
        }

        /// <summary>
        /// Gets the reference manager.
        /// </summary>
        /// <value>
        /// The reference manager.
        /// </value>
        public ReferenceManager ReferenceManager { get; private set; }

        /// <summary>
        /// Gets the type stack.
        /// </summary>
        /// <value>
        /// The type stack.
        /// </value>
        public Stack<Type> TypeStack { get; private set; }

        /// <summary>
        /// Gets the contexts.
        /// </summary>
        /// <value>
        /// The contexts.
        /// </value>
        public Stack<ISerializationContext<TSerializationContextInfo>> Contexts { get; private set; }
    }
}
