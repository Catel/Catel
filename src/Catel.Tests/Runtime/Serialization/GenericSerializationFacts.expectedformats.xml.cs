namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Catel.Runtime.Serialization;
    using Catel.Tests.Data;
    using Catel.Tests.Runtime.Serialization.TestModels;
    using NUnit.Framework;
    using static Catel.Tests.Runtime.Serialization.GenericSerializationFacts.CollectionSerializationFacts;

    public partial class GenericSerializationFacts
    {
        public partial class ExpectedFormats
        {
            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_GraphDepth_1_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(1);

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_GraphDepth_2_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(2);

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_GraphDepth_3_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(3);

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_Collection_Async()
            {
                var originalObject = new List<Country>
                {
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan" },
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt" }
                };

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_Array_Async()
            {
                var originalObject = new[]
                {
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan"},
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt"},
                };

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_Dictionary_Async()
            {
                var originalObject = new Dictionary<string, int>
                {
                    { "skip", 1 },
                    { "take", 2 },
                    { "some other string", 3 }
                };

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_ComplexHierarchyWithInheritance_Async()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_ComplexHierarchyNonCatel_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_SerializationWithPrivateMembers_Async()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                await TestXmlSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Xml_ModelsWithParsableObjectsAndSerializerModifierNotUsingParse_Async()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                var serializationManager = new SerializationManager();
                serializationManager.AddSerializerModifier<TestModelWithParsableMembersWithoutAttributes, TestModelWithParsableMembersNotUsingParseSerializerModifier>();

                await TestXmlSerializationWithExpectedFormatAsync(originalObject, serializationManager);
            }
        }
    }
}
