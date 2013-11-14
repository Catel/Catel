// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.Xml.Linq;

    /// <summary>
    /// Possible xml serializer optimalization modes
    /// </summary>
    public enum XmlSerializerOptimalizationMode
    {
        /// <summary>
        /// If pretty xml is required (for display reasons), pick this one.
        /// </summary>
        PrettyXml,

        /// <summary>
        /// If duplicate namespaces are irrelevant, pick this for speed.
        /// </summary>
        Performance
    }

    /// <summary>
    /// Interface for the xml serializer.
    /// </summary>
    public interface IXmlSerializer : IModelBaseSerializer<XmlSerializationContextInfo>
    {
        /// <summary>
        /// Gets or sets the optimalization mode.
        /// <para />
        /// The default value is <see cref="XmlSerializerOptimalizationMode.Performance"/>.
        /// </summary>
        /// <value>The optimalization mode.</value>
        XmlSerializerOptimalizationMode OptimalizationMode { get; set; } 
    }
}