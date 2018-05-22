// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationScope.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// The serialization scope.
    /// </summary>
    public class SerializationScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationScope"/> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="configuration">The configuration.</param>
        public SerializationScope(ISerializer serializer, ISerializationConfiguration configuration)
        {
            Argument.IsNotNull(() => serializer);

            Serializer = serializer;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <value>
        /// The serializer.
        /// </value>
        public ISerializer Serializer { get; private set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ISerializationConfiguration Configuration { get; set; }
    }
}