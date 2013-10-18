// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberSerializationEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Special member value serialization event args.
    /// </summary>
    public class MemberSerializationEventArgs : SerializationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberSerializationEventArgs" /> class.
        /// </summary>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="memberValue">The member value.</param>
        public MemberSerializationEventArgs(ISerializationContext serializationContext, MemberValue memberValue)
            : base(serializationContext)
        {
            MemberValue = memberValue;
            // Note: no check for null to improve performance
        }

        /// <summary>
        /// Gets the member value.
        /// </summary>
        /// <value>The member value.</value>
        public MemberValue MemberValue { get; private set; }
    }
}