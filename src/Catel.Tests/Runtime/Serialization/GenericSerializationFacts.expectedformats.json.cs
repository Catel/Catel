namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using ApprovalTests;
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
            public void Json_GraphDepth_1()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(1);

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_GraphDepth_2()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(2);

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_GraphDepth_3()
            {
                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(3);

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_Collection()
            {
                var originalObject = new List<Country>();
                originalObject.Add(new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan" });
                originalObject.Add(new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt" });

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_Array()
            {
                var originalObject = new[]
                {
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425ae"), IsoCode = "AF", Description = "Afghanistan"},
                    new Country { Id = Guid.Parse("19721f5a-b406-4079-89b3-1011005425af"), IsoCode = "AG", Description = "Agypt"},
                };

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_Dictionary()
            {
                var originalObject = new Dictionary<string, int>();
                originalObject.Add("skip", 1);
                originalObject.Add("take", 2);
                originalObject.Add("some other string", 3);

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_ComplexHierarchyWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_ComplexHierarchyNonCatel()
            {
                var originalObject = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_SerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                TestJsonSerializationWithExpectedFormat(originalObject);
            }

            [Test]
            public void Json_ModelsWithParsableObjectsAndSerializerModifierNotUsingParse()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                var serializationManager = new SerializationManager();
                serializationManager.AddSerializerModifier<TestModelWithParsableMembersWithoutAttributes, TestModelWithParsableMembersNotUsingParseSerializerModifier>();

                TestJsonSerializationWithExpectedFormat(originalObject, serializationManager);
            }
        }
    }
}
