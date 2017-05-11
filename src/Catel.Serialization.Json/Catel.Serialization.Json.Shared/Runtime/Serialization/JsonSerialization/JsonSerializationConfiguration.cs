// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializationConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization.Json
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Serialization configuration with additional json configuration.
    /// </summary>
    public class JsonSerializationConfiguration : SerializationConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the use bson instead of json.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bson should be used instead of json; otherwise, <c>false</c>.
        /// </value>
        public bool UseBson { get; set; }

        /// <summary>
        /// Gets or sets the kind of the date time.
        /// </summary>
        /// <value>
        /// The kind of the date time.
        /// </value>
        public DateTimeKind DateTimeKind { get; set; }

        /// <summary>
        /// Gets or sets the date parse handling.
        /// </summary>
        /// <value>
        /// The date parse handling.
        /// </value>
        public DateParseHandling DateParseHandling { get; set; }

        /// <summary>
        /// Gets or sets the date time zone handling.
        /// </summary>
        /// <value>
        /// The date time zone handling.
        /// </value>
        public DateTimeZoneHandling DateTimeZoneHandling { get; set; }

        /// <summary>
        /// Gets or sets the json formatting.
        /// </summary>
        /// <value>
        /// The json formatting.
        /// </value>
        public Formatting Formatting { get; set; }
    }
}