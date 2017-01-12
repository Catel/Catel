// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICustomJsonSerializable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Xml
{
    using System.Xml.Linq;

    /// <summary>
    /// Allows a type to implement their own (de)serialization mechanism.
    /// </summary>
    public interface ICustomXmlSerializable
    {
        /// <summary>
        /// Serializes the object to the specified xml element.
        /// </summary>
        /// <param name="xmlElement">The XML element.</param>
        void Serialize(XElement xmlElement);

        /// <summary>
        /// Serializes the object from the specified xml element.
        /// <para />
        /// Note that the object is always constructed by the serialization engine, the
        /// object itself must read the values from the <paramref name="xmlElement" />.
        /// </summary>
        /// <param name="xmlElement">The XML element.</param>
        void Deserialize(XElement xmlElement);
    }
}