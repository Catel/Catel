// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public static class SerializationTestHelper
    {
        /// <summary>
        /// Serializes and deserializes using the specified serializer.
        /// </summary>
        /// <typeparam name="TModel">The type of the T model.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>System.Object.</returns>
        public static TModel SerializeAndDeserialize<TModel>(TModel model, IModelBaseSerializer serializer)
            where TModel : ModelBase
        {
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(model, memoryStream);

                memoryStream.Position = 0L;

                return (TModel)serializer.Deserialize(typeof (TModel), memoryStream);
            }
        }
    }
}