// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETSTANDARD


namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Catel.Data;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    [ObsoleteEx(Message = "Binary serialization should no longer be used for security reasons, see https://github.com/Catel/Catel/issues/1216", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
    public class BinarySerializationContextInfo : SerializationContextInfoBase<BinarySerializationContextInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializationContextInfo" /> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="memberValues">The member values.</param>
        /// <param name="binaryFormatter">The binary formatter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationInfo" /> is <c>null</c>.</exception>
        public BinarySerializationContextInfo(SerializationInfo serializationInfo, List<MemberValue> memberValues = null, BinaryFormatter binaryFormatter = null)
        {
            Argument.IsNotNull("serializationInfo", serializationInfo);

            MemberValues = new List<MemberValue>();
            PropertyValues = new List<PropertyValue>();

            BinaryFormatter = binaryFormatter;
            SerializationInfo = serializationInfo;

            if (memberValues != null)
            {
                MemberValues = memberValues;
            }
        }

        /// <summary>
        /// Gets the binary formatter.
        /// </summary>
        /// <value>The binary formatter.</value>
        public BinaryFormatter BinaryFormatter { get; private set; }

        /// <summary>
        /// Gets the member values.
        /// </summary>
        /// <value>The member values.</value>
        public List<MemberValue> MemberValues { get; internal set; }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>The property values.</value>
        public List<PropertyValue> PropertyValues { get; internal set; }

        /// <summary>
        /// Gets the serialization info.
        /// </summary>
        /// <value>The serialization info.</value>
        public SerializationInfo SerializationInfo { get; private set; }
    }
}

#endif
