// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IXmlSerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    /// <summary>
    /// Interface for the xml serializer.
    /// </summary>
    public interface IXmlSerializer : ISerializer
    {
        /// <summary>
        /// Gets or sets the default fallback optimalization mode if it's not specified via <see cref="XmlSerializationConfiguration"/>.
        /// <para />
        /// The default value is <see cref="XmlSerializerOptimalizationMode.Performance"/>.
        /// </summary>
        /// <value>The optimalization mode.</value>
        XmlSerializerOptimalizationMode OptimalizationMode { get; set; } 
    }
}