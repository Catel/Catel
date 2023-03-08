namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Catel.Tests.Data;
    using Catel.Tests.Runtime.Serialization.TestModels;
    using NUnit.Framework;

    public class DataContractSerializerFactoryFacts
    {
        private class DataContractSerializerFactoryWrapper : DataContractSerializerFactory
        {
            public string CreateCacheKeyWrapper(Type serializingType, Type typeToSerialize, string xmlName, IReadOnlyCollection<Type>? additionalKnownTypes)
            {
                return base.CreateCacheKey(serializingType, typeToSerialize, xmlName, additionalKnownTypes);
            }
        }

        [TestFixture]
        public class The_CreateCacheKey_Method
        {
            [TestCase]
            public void Creates_Unique_Cache_Keys()
            {
                var factory = new DataContractSerializerFactoryWrapper();

                var keys = new List<string>();

                // 2 different additional types, used to go wrong
                keys.Add(factory.CreateCacheKeyWrapper(typeof(List<SerializableKeyValuePair>), typeof(SerializableKeyValuePair), "Items", new[]
                {
                    typeof(DataItem),
                    typeof(string)
                }));

                keys.Add(factory.CreateCacheKeyWrapper(typeof(List<SerializableKeyValuePair>), typeof(SerializableKeyValuePair), "Items", new[]
                {
                    typeof(IniEntry),
                    typeof(string)
                }));

                Assert.AreEqual(keys, keys.Distinct());
            }
        }
    }
}
