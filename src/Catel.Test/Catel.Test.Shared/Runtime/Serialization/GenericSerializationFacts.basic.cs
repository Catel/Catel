// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
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

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                }, false);
            }

            [TestCase]
            public void SerializationWithCustomTypes()
            {
                var originalObject = new ObjectWithCustomType();
                originalObject.FirstName = "Test";
                originalObject.Gender = Gender.Female;

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

#if NET
            [TestCase]
            public void SerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }
#endif

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexHierarchy();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer);

                    Assert.IsTrue(complexHierarchy == deserializedObject, description);
                });
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

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void SerializeWithIFieldSerializable()
            {
                var originalObject = new NonCatelTestModelWithIFieldSerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.IsTrue(originalObject.GetViaInterface, description);
                    Assert.IsTrue(clonedObject.SetViaInterface, description);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void SerializeWithIPropertySerializable()
            {
                var originalObject = new NonCatelTestModelWithIPropertySerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.IsTrue(originalObject.GetViaInterface, description);
                    Assert.IsTrue(clonedObject.SetViaInterface, description);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer);

                    Assert.AreEqual(complexHierarchy.LastName, deserializedObject.LastName, description);
                    Assert.AreEqual(complexHierarchy.Persons.Count, deserializedObject.Persons.Count, description);

                    for (int i = 0; i < deserializedObject.Persons.Count; i++)
                    {
                        var expectedPerson = complexHierarchy.Persons[i];
                        var actualPerson = deserializedObject.Persons[i];

                        Assert.AreEqual(expectedPerson.Gender, actualPerson.Gender, description);
                        Assert.AreEqual(expectedPerson.FirstName, actualPerson.FirstName, description);
                        Assert.AreEqual(expectedPerson.LastName, actualPerson.LastName, description);
                    }
                });
            }
        }

        [TestFixture]
        public class GenericBasicSerializationFacts
        {
            [TestCase]
            public void SerializesModelsWithParsableObjects()
            {
                var originalObject = new TestModelWithParsableMembers();
                originalObject.Vector = new Vector(1, 2, 3);

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject.Vector.X, clonedObject.Vector.X, description);
                    Assert.AreEqual(originalObject.Vector.Y, clonedObject.Vector.Y, description);
                    Assert.AreEqual(originalObject.Vector.Z, clonedObject.Vector.Z, description);
                });
            }
        }
    }
}