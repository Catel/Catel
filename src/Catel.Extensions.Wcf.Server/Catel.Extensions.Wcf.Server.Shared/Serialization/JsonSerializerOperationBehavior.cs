﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerOperationBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel.Description;
    using System.Xml;

    /// <summary>
    /// The json serializer operation behavior.
    /// </summary>
    public class JsonSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializerOperationBehavior"/> class.
        /// </summary>
        /// <param name="operation">An <see cref="T:System.ServiceModel.Description.OperationDescription" /> that represents the operation.</param>
        public JsonSerializerOperationBehavior(OperationDescription operation) : base(operation)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer" /> for serialization and deserialization processes.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> to create the serializer for.</param>
        /// <param name="name">The name of the generated type.</param>
        /// <param name="ns">The namespace of the generated type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Type" /> that contains known types.</param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer" /> class.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            Argument.IsNotNull("type", type);

            return new DataContractJsonSerializer(type);
        }

        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer" /> for serialization and deserialization processes with an <see cref="T:System.Xml.XmlDictionaryString" /> that contains the namespace.
        /// </summary>
        /// <param name="type">The type to serialize or deserialize.</param>
        /// <param name="name">The name of the serialized type.</param>
        /// <param name="ns">An <see cref="T:System.Xml.XmlDictionaryString" /> that contains the namespace of the serialized type.</param>
        /// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IList`1" /> of <see cref="T:System.Type" /> that contains known types.</param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer" /> class.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            Argument.IsNotNull("type", type);

            return new DataContractJsonSerializer(type);
        }
        #endregion
    }
}