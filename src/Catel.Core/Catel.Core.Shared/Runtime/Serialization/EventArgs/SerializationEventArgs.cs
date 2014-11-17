// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// The serialization event args.
    /// </summary>
    public class SerializationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationEventArgs"/> class.
        /// </summary>
        /// <param name="serializationContext">The serialization context.</param>
        public SerializationEventArgs(ISerializationContext serializationContext)
        {
            // Note: no check for null to improve performance
            SerializationContext = serializationContext;
        }

        /// <summary>
        /// Gets the serialization context.
        /// </summary>
        /// <value>The serialization context.</value>
        public ISerializationContext SerializationContext { get; private set; }
    }
}