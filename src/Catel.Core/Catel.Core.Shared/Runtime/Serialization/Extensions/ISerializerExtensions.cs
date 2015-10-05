// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.IO;

    /// <summary>
    /// ISerializer extensions.
    /// </summary>
    public static class ISerializerExtensions
    {
        /// <summary>
        /// Deserializes the specified stream into the model.
        /// </summary>
        /// <typeparam name="TModel">The type of the t model.</typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized model.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializer"/> is <c>null</c>.</exception>
        public static TModel Deserialize<TModel>(this ISerializer serializer, Stream stream)
        {
            Argument.IsNotNull(() => serializer);

            var model = serializer.Deserialize(typeof (TModel), stream);
            return (TModel) model;
        }
    }
}