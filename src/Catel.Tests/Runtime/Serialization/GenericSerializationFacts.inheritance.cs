namespace Catel.Tests.Runtime.Serialization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Runtime.Serialization;
    using Catel.Tests.Runtime.Serialization.TestModels;
    using NUnit.Framework;
    using VerifyNUnit;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class FailedTestFacts
        {
            [Test]
            public async Task DictionaryWithItemsListSerializationTestAsync()
            {
                var serializableData = new SerializableData
                {
                    Items = new List<DataItem>
                    {
                        new()
                        {
                            Name = "Part of list"
                        }
                    },

                    Roots = new Dictionary<string, DataItem>
                    {
                        {
                            "Key", new DataItem
                            {
                                Name = "Part of dictionary"
                            }
                        }
                    }
                };

                var result = SerializationTestHelper.SerializeAndDeserialize(serializableData, SerializationFactory.GetXmlSerializer());

                await Verifier.Verify(result);
            }

            [Explicit, Test]
            public async Task SerializeInheritedFromModelBaseAsync()
            {
                var inheritedFromModelBase = new InheritedFromModelBase
                {
                    Name = "Inherited"
                };

                var inheritedFromModelBaseCopy = SerializationTestHelper.SerializeAndDeserialize(inheritedFromModelBase, SerializationFactory.GetXmlSerializer());


                var notInheritedFromModelBase = new NotInheritedFromModelBase
                {
                    Name = "NotInherited"
                };

                var notInheritedFromModelBaseCopy = SerializationTestHelper.SerializeAndDeserialize(notInheritedFromModelBase, SerializationFactory.GetXmlSerializer());

                Assert.IsNotNull(inheritedFromModelBaseCopy.Name);
                Assert.IsNotNull(notInheritedFromModelBaseCopy.Name);
            }

            [Explicit, Test, Timeout(5000)]
            public async Task HierarchyOfModelBaseObjectsTestAsync()
            {
                var itemD = new DataItemD
                {
                    Name = "Item D"
                };

                var itemR = new DataItemR
                {
                    Name = "Data item R",
                    Parts = new List<DataItemRPart>
                    {
                        new()
                        {
                            Item = itemD,
                            Name = "Parts"
                        }
                    },
                };

                var itemV = new DataItemV
                {
                    First = itemR,
                    Second = itemD
                };

                var data = new ContentData
                {
                    DataItems = new List<IDataItem>
                    {
                        itemV,
                        itemR,
                        itemD
                    },

                    Roots = new Dictionary<string, IDataItem>
                    {
                        { "Key", itemV }
                    }
                };

                var xmlSerializer = SerializationFactory.GetXmlSerializer();
                var dataCopy = SerializationTestHelper.SerializeAndDeserialize(data, SerializationFactory.GetXmlSerializer());

                await Verifier.Verify(dataCopy);
            }
        }
    }
}
