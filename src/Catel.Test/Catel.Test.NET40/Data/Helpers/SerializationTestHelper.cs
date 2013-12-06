// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Data
{
    using System;
    using System.IO;

    using Catel.Data;

    public static class SerializationTestHelper
    {
        /// <summary>
        /// Serializes and deserializes an object using the specified mode. Finally, it will check whether the original object is equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testObject">The test object.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The deserialized object.</returns>
        public static T SerializeAndDeserializeObject<T>(T testObject, SerializationMode mode)
            where T : SavableModelBase<T>
        {
            using (var memoryStream = new MemoryStream())
            {
                testObject.Save(memoryStream, mode);

                memoryStream.Position = 0L;

                return ModelBase.Load<T>(memoryStream, mode);
            }
        }
    }
}