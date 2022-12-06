namespace Catel.Tests.Runtime.Serialization
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;
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

        public class DictionarySerialization
        {
            [Test]
            public void SerializeDictionaryWithPocoClassAsKey()
            {
                var originalObject = new FailToSerialize();
                originalObject.Lookup.Add(new PocoKeyClass { X = 1.0, Y = 2.0, Z = 3.0 }, "test me");
                Assert.AreEqual(1, originalObject.Lookup.Count);

                TestSerializationOnXmlSerializer((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.IsNotNull(clonedObject);
                    Assert.AreEqual(1, clonedObject.Lookup.Count);
                });
            }

            [KnownType(typeof(PocoKeyClass))]
            public class FailToSerialize : ModelBase
            {
                [IncludeInSerialization]
                public Dictionary<PocoKeyClass, string> Lookup { get; set; } = new Dictionary<PocoKeyClass, string>();
            }

            public class PocoKeyClass
            {
                [IncludeInSerialization]
                public double X { get; set; }

                [IncludeInSerialization]
                public double Y { get; set; }

                [IncludeInSerialization]
                public double Z { get; set; }
            }
        }
    }
}
