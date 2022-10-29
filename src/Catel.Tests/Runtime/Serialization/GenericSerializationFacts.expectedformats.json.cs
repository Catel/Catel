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
            public async Task Json_GraphDepth_1_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(1);

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_GraphDepth_2_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(2);

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_GraphDepth_3_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(3);

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_Collection_Async()
            {
                var originalObject = new List<Country>();
                originalObject.Add(new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan" });
                originalObject.Add(new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt" });

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_Array_Async()
            {
                var originalObject = new[]
                {
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan"},
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt"},
                };

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_Dictionary_Async()
            {
                var originalObject = new Dictionary<string, int>();
                originalObject.Add("skip", 1);
                originalObject.Add("take", 2);
                originalObject.Add("some other string", 3);

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_ComplexHierarchyWithInheritance_Async()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_ComplexHierarchyNonCatel_Async()
            {
                var originalObject = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_SerializationWithPrivateMembers_Async()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                await TestJsonSerializationWithExpectedFormatAsync(originalObject);
            }

            [Test]
            [MethodImpl(MethodImplOptions.NoInlining)]
            public async Task Json_ModelsWithParsableObjectsAndSerializerModifierNotUsingParse_Async()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                var serializationManager = new SerializationManager();
                serializationManager.AddSerializerModifier<TestModelWithParsableMembersWithoutAttributes, TestModelWithParsableMembersNotUsingParseSerializerModifier>();

                await TestJsonSerializationWithExpectedFormatAsync(originalObject, serializationManager);
            }
        }
    }
}
