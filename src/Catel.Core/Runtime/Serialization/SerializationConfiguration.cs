// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System.Globalization;

    /// <summary>
    /// Serialization configuration.
    /// </summary>
    public class SerializationConfiguration : ISerializationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationConfiguration"/> class.
        /// </summary>
        public SerializationConfiguration()
        {
            Culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the culture used for serialization.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; set; }
    }
}