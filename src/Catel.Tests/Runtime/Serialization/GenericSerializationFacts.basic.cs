namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Data;
    using NUnit.Framework;
    using TestModels;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class CatelModelBasicSerializationFacts
        {
            [TestCase]
            public void SerializationLevel1()
            {
                var originalObject = ModelBaseTestHelper.CreateIniEntryObject();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                });
            }

            [TestCase]
            public void SerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                });
            }

            [TestCase]
            public void SerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                });
            }

            [TestCase]
            public void SerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                }, false);
            }

            [TestCase]
            public void SerializationWithCustomTypes()
            {
                var originalObject = new ObjectWithCustomType();
                originalObject.FirstName = "Test";
                originalObject.Gender = Gender.Female;

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                });
            }

            [TestCase]
            public void SerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject, Is.EqualTo(originalObject), description);
                });
            }

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexHierarchy();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                //TestSerializationOnJsonSerializer((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer, config);

                    Assert.That(complexHierarchy, Is.EqualTo(deserializedObject), description);
                });
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void CorrectlyCallsOnSerializedForAllModelsInGraph(int depth)
            {
                // Written for https://github.com/Catel/Catel/issues/1194

                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(depth);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                //TestSerializationOnXmlSerializer((serializer, config, description) =>
                //TestSerializationOnJsonSerializer((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    AssertSerializationCalls(originalObject,
                        serializer.GetType().Name,
                        new[]
                        {
                            nameof(ComputerSettings._onSerializingCalls),
                            nameof(ComputerSettings._onSerializedCalls)
                        });
                });
            }

            [TestCase(1)]
            [TestCase(2)]
            [TestCase(3)]
            public void CorrectlyCallsOnDeserializedForAllModelsInGraph(int depth)
            {
                // Written for https://github.com/Catel/Catel/issues/1194

                var originalObject = ComplexSerializationHierarchy.CreateObjectGraphWithDepth(depth);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    AssertSerializationCalls(clonedObject,
                        serializer.GetType().Name,
                        new[]
                        {
                            nameof(ComputerSettings._onDeserializingCalls),
                            nameof(ComputerSettings._onDeserializedCalls)
                        });
                });
            }

            private static void AssertSerializationCalls(object obj, string serializationMode, string[] fieldNames)
            {
                foreach (var fieldName in fieldNames)
                {
                    var field = obj.GetType().GetFieldEx(fieldName);
                    var fieldValue = (int)field.GetValue(obj);

                    Assert.That(fieldValue, Is.EqualTo(1), $"{obj.GetType().Name}.{fieldName} should have been 1 for serialization mode {serializationMode}");
                }

                var methodInfo = obj.GetType().GetMethodEx("ClearSerializationCounters");
                methodInfo.Invoke(obj, Array.Empty<object>());

                if (obj is ComputerSettings computerSettings)
                {
                    foreach (var iniFile in computerSettings.IniFileCollection)
                    {
                        AssertSerializationCalls(iniFile, serializationMode, fieldNames);
                    }

                    return;
                }
                else if (obj is IniFile iniFile)
                {
                    foreach (var iniEntry in iniFile.IniEntryCollection)
                    {
                        AssertSerializationCalls(iniEntry, serializationMode, fieldNames);
                    }

                    return;
                }
                else if (obj is IniEntry iniEntry)
                {
                    // No need to check children
                    return;
                }

                Assert.Fail($"Unsupported model '{obj.GetType()}'");
            }
        }

        [TestFixture]
        public class NonCatelModelBasicSerializationFacts
        {
            [TestCase]
            public void SerializeSimpleModels()
            {
                var originalObject = new NonCatelTestModel();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject.FirstName, Is.EqualTo(originalObject.FirstName), description);
                    Assert.That(clonedObject.LastName, Is.EqualTo(originalObject.LastName), description);
                });
            }

            [TestCase]
            public void SerializeWithIFieldSerializable()
            {
                var originalObject = new NonCatelTestModelWithIFieldSerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(originalObject.GetViaInterface, Is.True, description);
                    Assert.That(clonedObject.SetViaInterface, Is.True, description);

                    Assert.That(clonedObject.FirstName, Is.EqualTo(originalObject.FirstName), description);
                    Assert.That(clonedObject.LastName, Is.EqualTo(originalObject.LastName), description);
                });
            }

            [TestCase]
            public void SerializeWithIPropertySerializable()
            {
                var originalObject = new NonCatelTestModelWithIPropertySerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(originalObject.GetViaInterface, Is.True, description);
                    Assert.That(clonedObject.SetViaInterface, Is.True, description);

                    Assert.That(clonedObject.FirstName, Is.EqualTo(originalObject.FirstName), description);
                    Assert.That(clonedObject.LastName, Is.EqualTo(originalObject.LastName), description);
                });
            }

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer, config);

                    Assert.That(deserializedObject.LastName, Is.EqualTo(complexHierarchy.LastName), description);
                    Assert.That(deserializedObject.Persons.Count, Is.EqualTo(complexHierarchy.Persons.Count), description);

                    for (int i = 0; i < deserializedObject.Persons.Count; i++)
                    {
                        var expectedPerson = complexHierarchy.Persons[i];
                        var actualPerson = deserializedObject.Persons[i];

                        Assert.That(actualPerson.Gender, Is.EqualTo(expectedPerson.Gender), description);
                        Assert.That(actualPerson.FirstName, Is.EqualTo(expectedPerson.FirstName), description);
                        Assert.That(actualPerson.LastName, Is.EqualTo(expectedPerson.LastName), description);
                    }
                });
            }
        }

        [TestFixture]
        public class GenericBasicSerializationFacts
        {
            [TestCase]
            public void SerializesModelsWithParsableObjectsWithAttributes()
            {
                var originalObject = new TestModelWithParsableMembersWithAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject.Vector.UsedParse, Is.True);
                    Assert.That(clonedObject.Vector.X, Is.EqualTo(originalObject.Vector.X), description);
                    Assert.That(clonedObject.Vector.Y, Is.EqualTo(originalObject.Vector.Y), description);
                    Assert.That(clonedObject.Vector.Z, Is.EqualTo(originalObject.Vector.Z), description);
                });
            }

            [TestCase]
            public void SerializesModelsWithParsableObjectsWithoutAttributes()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject.Vector.UsedParse, Is.False);
                    Assert.That(clonedObject.Vector.X, Is.EqualTo(originalObject.Vector.X), description);
                    Assert.That(clonedObject.Vector.Y, Is.EqualTo(originalObject.Vector.Y), description);
                    Assert.That(clonedObject.Vector.Z, Is.EqualTo(originalObject.Vector.Z), description);
                });
            }

            [TestCase]
            public void SerializesModelsWithParsableObjectsWithoutAttributesAndSerializerModifierUsingParse()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                var serializationManager = new SerializationManager();
                serializationManager.AddSerializerModifier<TestModelWithParsableMembersWithoutAttributes, TestModelWithParsableMembersUsingParseSerializerModifier>();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject.Vector.UsedParse, Is.True);
                    Assert.That(clonedObject.Vector.X, Is.EqualTo(originalObject.Vector.X), description);
                    Assert.That(clonedObject.Vector.Y, Is.EqualTo(originalObject.Vector.Y), description);
                    Assert.That(clonedObject.Vector.Z, Is.EqualTo(originalObject.Vector.Z), description);
                }, serializationManager: serializationManager);
            }

            [TestCase]
            public void SerializesModelsWithParsableObjectsAndSerializerModifierNotUsingParse()
            {
                var originalObject = new TestModelWithParsableMembersWithoutAttributes();
                originalObject.Vector = new Vector(1, 2, 3);

                var serializationManager = new SerializationManager();
                serializationManager.AddSerializerModifier<TestModelWithParsableMembersWithoutAttributes, TestModelWithParsableMembersNotUsingParseSerializerModifier>();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer, config);

                    Assert.That(clonedObject.Vector.UsedParse, Is.False);
                    Assert.That(clonedObject.Vector.X, Is.EqualTo(originalObject.Vector.X), description);
                    Assert.That(clonedObject.Vector.Y, Is.EqualTo(originalObject.Vector.Y), description);
                    Assert.That(clonedObject.Vector.Z, Is.EqualTo(originalObject.Vector.Z), description);
                }, serializationManager: serializationManager);
            }
        }
    }
}
