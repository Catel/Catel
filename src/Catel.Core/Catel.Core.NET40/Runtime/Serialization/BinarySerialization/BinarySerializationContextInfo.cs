// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializationContextInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Catel.Data;

    /// <summary>
    /// Class containing all information about the binary serialization context.
    /// </summary>
    public class BinarySerializationContextInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializationContextInfo" /> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationInfo" /> is <c>null</c>.</exception>
        public BinarySerializationContextInfo(SerializationInfo serializationInfo)
        {
            Argument.IsNotNull("serializationInfo", serializationInfo);

            SerializationInfo = serializationInfo;
            PropertyValues = new List<PropertyValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializationContextInfo" /> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="binaryFormatter">The binary formatter.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationInfo" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="binaryFormatter" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyValues" /> is <c>null</c>.</exception>
        public BinarySerializationContextInfo(SerializationInfo serializationInfo, BinaryFormatter binaryFormatter, List<PropertyValue> propertyValues)
        {
            Argument.IsNotNull("serializationInfo", serializationInfo);
            Argument.IsNotNull("binaryFormatter", binaryFormatter);
            Argument.IsNotNull("propertyValues", propertyValues);

            SerializationInfo = serializationInfo;
            BinaryFormatter = binaryFormatter;
            PropertyValues = propertyValues;
        }

        /// <summary>
        /// Gets the serialization info.
        /// </summary>
        /// <value>The serialization info.</value>
        public SerializationInfo SerializationInfo { get; private set; }

        /// <summary>
        /// Gets the binary formatter.
        /// </summary>
        /// <value>The binary formatter.</value>
        public BinaryFormatter BinaryFormatter { get; private set; }

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>The property values.</value>
        public List<PropertyValue> PropertyValues { get; internal set; }
    }
}