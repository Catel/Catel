// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System.IO;
    using System.Text;
    using Data;
    using IoC;
    using Runtime.Serialization;
    using Runtime.Serialization.Json;

    /// <summary>
    /// Json extensions.
    /// </summary>
    public static class JsonExtensions
    {
        private static readonly ISerializationManager SerializationManager = ServiceLocator.Default.ResolveType<ISerializationManager>();
        private static readonly IObjectAdapter ObjectAdapter = ServiceLocator.Default.ResolveType<IObjectAdapter>();

        /// <summary>
        /// Converters the specified model to a json string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
        public static string ToJson(this ModelBase model)
        {
            var jsonSerializer = new JsonSerializer(SerializationManager, TypeFactory.Default, ObjectAdapter);

            using (var stream = new MemoryStream())
            {
                jsonSerializer.Serialize(model, stream);

                stream.Position = 0L;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}