namespace Catel.Tests.Runtime.Serialization
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestModels;

    public class XmlSerializerFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
            [TestCase]
            public void XmlSerializationWithXmlIgnore()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var obj = new ObjectWithXmlMappings();

                    var xml = obj.ToXml(serializer).ToString();

                    Assert.That(xml.Contains("IgnoredProperty"), Is.False);
                }
            }

            [TestCase]
            public void XmlSerializationWithXmlMappings()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var originalObject = ModelBaseTestHelper.CreateComputerSettingsWithXmlMappingsObject();
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.That(clonedObject, Is.EqualTo(originalObject));
                }
            }

            [TestCase]
            public void ReadXml()
            {
                // Should always return null
                var iniFile = ModelBaseTestHelper.CreateIniFileObject();
                Assert.That(((IXmlSerializable)iniFile).GetSchema(), Is.EqualTo(null));
            }

            [TestCase]
            public void RespectsTheXmlRootAndXmlElementAttribute()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                    var xmlDocument = person.ToXml(serializer);

                    var personElement = xmlDocument.Element("MappedPerson");
                    Assert.That(personElement, Is.Not.Null);

                    var firstNameElement = personElement.Element("NameFirst");
                    Assert.That(firstNameElement, Is.Not.Null);
                    Assert.That(firstNameElement.Value, Is.EqualTo("Geert"));

                    var middleNameElement = personElement.Element("NameMiddle");
                    Assert.That(middleNameElement, Is.Not.Null);
                    Assert.That(middleNameElement.Value, Is.EqualTo("van"));

                    var lastNameElement = personElement.Element("NameLast");
                    Assert.That(lastNameElement, Is.Not.Null);
                    Assert.That(lastNameElement.Value, Is.EqualTo("Horrik"));

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream))
                        {
                            streamWriter.Write(xmlDocument.ToString());
                            streamWriter.Flush();

                            memoryStream.Position = 0L;

                            var deserializedPerson = serializer.Deserialize<ModelBaseFacts.Person>(memoryStream);

                            Assert.That(deserializedPerson.FirstName, Is.EqualTo("Geert"));
                            Assert.That(deserializedPerson.MiddleName, Is.EqualTo("van"));
                            Assert.That(deserializedPerson.LastName, Is.EqualTo("Horrik"));
                        }
                    }
                }
            }

            [TestCase]
            public void RespectsTheXmlAttributeAttribute()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                    var xmlDocument = person.ToXml(serializer);

                    var personElement = xmlDocument.Element("MappedPerson");
                    Assert.That(personElement, Is.Not.Null);

                    var ageAttribute = personElement.Attribute("FutureAge");
                    Assert.That(ageAttribute, Is.Not.Null);
                    Assert.That(ageAttribute.Value, Is.EqualTo("42"));

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream))
                        {
                            streamWriter.Write(xmlDocument.ToString());
                            streamWriter.Flush();

                            memoryStream.Position = 0L;

                            var deserializedPerson = serializer.Deserialize<ModelBaseFacts.Person>(memoryStream);

                            Assert.That(deserializedPerson.Age, Is.EqualTo(42));
                        }
                    }
                }
            }

            [TestCase(XmlSerializerOptimalizationMode.PrettyXml)]
            //[TestCase(XmlSerializerOptimalizationMode.PrettyXmlAgressive)]
            [TestCase(XmlSerializerOptimalizationMode.Performance)]
            public void RespectsTheXmlAttributeAttributeOnRootElements(XmlSerializerOptimalizationMode mode)
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var family = new XmlFamily();
                    family.LastName = "van Horrik";
                    family.Persons.Add(new XmlPerson
                    {
                        FirstName = "Geert",
                        LastName = family.LastName,
                        Gender = Gender.Male
                    });

                    var newFamily = SerializationTestHelper.SerializeAndDeserialize(family, serializer,
                        new XmlSerializationConfiguration
                        {
                        });

                    Assert.That(newFamily.LastName, Is.EqualTo(family.LastName));
                    Assert.That(newFamily.Persons.Count, Is.EqualTo(1));

                    var newPerson = newFamily.Persons.First();

                    Assert.That(newPerson.FirstName, Is.EqualTo(family.Persons[0].FirstName));
                    Assert.That(newPerson.LastName, Is.EqualTo(family.Persons[0].LastName));
                    Assert.That(newPerson.Gender, Is.EqualTo(family.Persons[0].Gender));
                }
            }

            [TestCase(XmlSerializerOptimalizationMode.PrettyXml)]
            //[TestCase(XmlSerializerOptimalizationMode.PrettyXmlAgressive)]
            [TestCase(XmlSerializerOptimalizationMode.Performance)]
            public void SerializesModelsWithOnlyAttributes(XmlSerializerOptimalizationMode mode)
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var family = new XmlFamily();
                    family.LastName = "van Horrik";
                    family.ModelsWithAttributesOnly.Add(new XmlModelWithAttributesOnly
                    {
                        FirstName = "Geert",
                    });

                    var newFamily = SerializationTestHelper.SerializeAndDeserialize(family, serializer,
                        new XmlSerializationConfiguration
                        {
                            // No longer using optimization mode, but keep this test alive
                        });

                    Assert.That(newFamily.LastName, Is.EqualTo(family.LastName));
                    Assert.That(newFamily.ModelsWithAttributesOnly.Count, Is.EqualTo(1));

                    var newModelWithAttributesOnly = newFamily.ModelsWithAttributesOnly.First();

                    Assert.That(newModelWithAttributesOnly.FirstName, Is.EqualTo(family.ModelsWithAttributesOnly[0].FirstName));
                }
            }

            [TestCase]
            public void RespectsTheXmlIgnoreAttribute()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                    var xmlDocument = person.ToXml(serializer);

                    var personElement = xmlDocument.Element("MappedPerson");
                    Assert.That(personElement, Is.Not.Null);

                    Assert.That(personElement.Element("FullName"), Is.Null);
                }
            }

            [TestCase]
            public void SupportsNestedHierarchySerialization()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var root = new ModelBaseFacts.Group()
                    {
                        Name = "myRoot"
                    };

                    var child = new ModelBaseFacts.Group()
                    {
                        Name = "myChild"
                    };

                    root.Items = new ObservableCollection<ModelBaseFacts.Item>();
                    root.Items.Add(child);

                    var newRoot = SerializationTestHelper.SerializeAndDeserialize(root, serializer);
                    Assert.That(newRoot, Is.Not.Null);
                    Assert.That(newRoot.Name, Is.EqualTo("myRoot"));
                    Assert.That(newRoot.Items.Count, Is.EqualTo(1));
                    Assert.That(newRoot.Items[0].Name, Is.EqualTo("myChild"));
                }
            }
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
            [TestCase]
            public void SerializesAbstractBaseCollections()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var collection = new ObservableCollection<AbstractBase>();

                    collection.Add(new Derived1
                    {
                        Name = "1"
                    });

                    collection.Add(new Derived2
                    {
                        Name = "2"
                    });

                    collection.Add(new Derived1
                    {
                        Name = "3"
                    });

                    collection.Add(new Derived1
                    {
                        Name = "4"
                    });

                    collection.Add(new Derived2
                    {
                        Name = "5"
                    });

                    var clonedCollection = SerializationTestHelper.SerializeAndDeserialize(collection, serializer, null);

                    Assert.That(clonedCollection.Count, Is.EqualTo(collection.Count));
                    Assert.That(((Derived1)clonedCollection[0]).Name, Is.EqualTo(((Derived1)collection[0]).Name));
                    Assert.That(((Derived2)clonedCollection[1]).Name, Is.EqualTo(((Derived2)collection[1]).Name));
                    Assert.That(((Derived1)clonedCollection[2]).Name, Is.EqualTo(((Derived1)collection[2]).Name));
                    Assert.That(((Derived1)clonedCollection[3]).Name, Is.EqualTo(((Derived1)collection[3]).Name));
                    Assert.That(((Derived2)clonedCollection[4]).Name, Is.EqualTo(((Derived2)collection[4]).Name));
                }
            }

            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomXmlSerializable_Simple()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var model = new CustomXmlSerializationModel
                    {
                        FirstName = "Geert"
                    };

                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                    // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                    Assert.That(model.IsCustomSerialized, Is.True);
                    Assert.That(clonedModel.IsCustomDeserialized, Is.True);

                    Assert.That(clonedModel.FirstName, Is.EqualTo(model.FirstName));
                }
            }

            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomXmlSerializable_Nested()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var model = new CustomXmlSerializationModelWithNesting
                    {
                        Name = "Test model with nesting",
                        NestedModel = new CustomXmlSerializationModel
                        {
                            FirstName = "Geert"
                        }
                    };

                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                    Assert.That(clonedModel.NestedModel, Is.Not.Null);

                    // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                    Assert.That(model.NestedModel.IsCustomSerialized, Is.True);
                    Assert.That(clonedModel.NestedModel.IsCustomDeserialized, Is.True);

                    Assert.That(clonedModel.Name, Is.EqualTo(model.Name));
                    Assert.That(clonedModel.NestedModel.FirstName, Is.EqualTo(model.NestedModel.FirstName));
                }
            }


            [TestCase]
            public void CorrectlySerializesToXmlString()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var serializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var testModel = new TestModel(serializer);

                    testModel._excludedField = "excluded";
                    testModel._includedField = "included";

                    testModel.ExcludedRegularProperty = "excluded";
                    testModel.IncludedRegularProperty = "included";

                    testModel.ExcludedCatelProperty = "excluded";
                    testModel.IncludedCatelProperty = "included";

                    var xml = testModel.ToXmlString(serializer);

                    Assert.That(xml.Contains("Excluded"), Is.False);
                }
            }
        }
    }
}
