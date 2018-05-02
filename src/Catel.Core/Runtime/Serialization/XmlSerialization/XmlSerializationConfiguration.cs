
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Xml
{
    /// <summary>
    /// Serialization configuration with additional xml configuration.
    /// </summary>
    public class XmlSerializationConfiguration : SerializationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationConfiguration"/> class.
        /// </summary>
        public XmlSerializationConfiguration()
        {
            OptimalizationMode = XmlSerializerOptimalizationMode.Performance;
        }

        /// <summary>
        /// Gets or sets the optimalization mode.
        /// </summary>
        /// <value>
        /// The optimalization mode.
        /// </value>
        public XmlSerializerOptimalizationMode OptimalizationMode { get; set; }
    }
}