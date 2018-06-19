// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableKeyValuePair.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializable key value pair.
    /// </summary>
#if NET || NETSTANDARD
    [DataContract, Serializable]
#endif
    public class SerializableKeyValuePair
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [DataMember]
        public object Key { get; set; }

        /// <summary>
        /// Gets or sets the type of the key.
        /// </summary>
        /// <value>The type of the key.</value>
        [ExcludeFromSerialization]
        [XmlIgnore]
        public Type KeyType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        [ExcludeFromSerialization]
        [XmlIgnore]
        public Type ValueType { get; set; }
    }
}
