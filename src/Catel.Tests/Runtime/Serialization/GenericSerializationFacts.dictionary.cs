// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using NUnit.Framework;
    using TestModels;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class TheKeyValuePairSerializerModifier
        {
            [TestCase]
            public void SerializesAndDeserializesKeyValuePairs()
            {
                var originalObject = new TestModelWithKeyValuePair();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.AreEqual(originalObject.KeyValuePair, clonedObject.KeyValuePair, description);
                    Assert.AreEqual(originalObject.KeyValuePairAsObject, clonedObject.KeyValuePairAsObject, description);
                });
            }
        }
    }
}