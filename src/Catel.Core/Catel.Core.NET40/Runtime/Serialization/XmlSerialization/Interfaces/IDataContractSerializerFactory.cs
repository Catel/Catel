// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContractFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface defining a factory to create <see cref="DataContractSerializer"/> objects for specific types.
    /// </summary>
    public interface IDataContractSerializerFactory
    {
        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        /// <param name="serializingObject">The object to create the serializer for. When the object is not <c>null</c>, the types that are
        /// a child object of this object are added to the known types of the serializer.</param>
        /// <param name="additionalKnownTypes">A list of additional types to add to the known types.</param>
        /// <returns><see cref="DataContractSerializer" /> for the given type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName" /> is <c>null</c> or whitespace.</exception>
        [ObsoleteEx(RemoveInVersion = "4.5", TreatAsErrorFromVersion = "4.2",
            Replacement = "IDataContractSerializerFactory.GetDataContractSerializer(serializingType, typeToSerialize, xmlName, rootNamespace, additionalKnownTypes)")]
        DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, string rootNamespace = null, object serializingObject = null, List<Type> additionalKnownTypes = null);

        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        /// <param name="additionalKnownTypes">A list of additional types to add to the known types.</param>
        /// <returns><see cref="DataContractSerializer" /> for the given type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName" /> is <c>null</c> or whitespace.</exception>
        DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, string rootNamespace = null, List<Type> additionalKnownTypes = null);

#if NET
        /// <summary>
        /// Gets or sets the <see cref="IDataContractSurrogate"/> passed in constructor to <see cref="DataContractSerializer"/>.
        /// <para />
        /// The default value is <null/>.
        /// </summary>
        /// <value>The <see cref="IDataContractSurrogate"/>.</value>
        IDataContractSurrogate DataContractSurrogate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataContractResolver"/> passed in constructor to <see cref="DataContractSerializer"/>.
        /// <para />
        /// The default value is <null/>.
        /// </summary>
        /// <value>The <see cref="DataContractResolver"/>.</value>
        DataContractResolver DataContractResolver { get; set; }
#endif

    }
}