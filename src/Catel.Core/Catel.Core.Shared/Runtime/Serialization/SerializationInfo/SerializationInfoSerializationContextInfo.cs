// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// Class containing all information about the serialization info (.NET only) serialization context.
    /// </summary>
    public class SerializationInfoSerializationContextInfo : ISerializationContextInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationInfoSerializationContextInfo"/> class.
        /// </summary>
        public SerializationInfoSerializationContextInfo()
        {
            MemberValues = new List<MemberValue>();
            PropertyValues = new List<PropertyValue>();
        }

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationInfoSerializationContextInfo" /> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="memberValues">The member values.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationInfo" /> is <c>null</c>.</exception>
        public SerializationInfoSerializationContextInfo(SerializationInfo serializationInfo, List<MemberValue> memberValues = null)
            : this()
        {
            Argument.IsNotNull("serializationInfo", serializationInfo);

            SerializationInfo = serializationInfo;

            if (memberValues != null)
            { 
                MemberValues = memberValues;
            }
        }
#endif

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

#if NET
        /// <summary>
        /// Gets the serialization info.
        /// </summary>
        /// <value>The serialization info.</value>
        public SerializationInfo SerializationInfo { get; private set; }
#endif
    }
}