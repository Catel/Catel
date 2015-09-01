// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET


namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    public class BinarySerializationContextInfo : SerializationInfoSerializationContextInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializationContextInfo" /> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="memberValues">The member values.</param>
        /// <param name="binaryFormatter">The binary formatter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationInfo" /> is <c>null</c>.</exception>
        public BinarySerializationContextInfo(SerializationInfo serializationInfo, List<MemberValue> memberValues = null, BinaryFormatter binaryFormatter = null)
            : base(serializationInfo, memberValues)
        {
            BinaryFormatter = binaryFormatter;
        }

        /// <summary>
        /// Gets the binary formatter.
        /// </summary>
        /// <value>The binary formatter.</value>
        public BinaryFormatter BinaryFormatter { get; private set; }
    }
}

#endif