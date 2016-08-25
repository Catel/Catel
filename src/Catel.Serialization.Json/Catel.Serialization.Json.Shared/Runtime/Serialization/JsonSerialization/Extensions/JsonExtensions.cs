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
        [ObsoleteEx(ReplacementTypeOrMember = "ToJson(ModelBase, ISerializationConfiguration)",
            TreatAsErrorFromVersion = "4.5", RemoveInVersion = "5.0")]
        public static string ToJson(this ModelBase model)
        {
            return ToJson(model, null);
        }

        /// <summary>
        /// Converters the specified model to a json string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ToJson(this ModelBase model, ISerializationConfiguration configuration)
        {
            var jsonSerializer = new JsonSerializer(SerializationManager, TypeFactory.Default, ObjectAdapter);

            using (var stream = new MemoryStream())
            {
                jsonSerializer.Serialize(model, stream, configuration);

                stream.Position = 0L;

                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}