// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using NUnit.Framework;
    using TestModels;

    public class XmlSerializerFacts
    {

#if NET
        [Serializable]
#endif
        public class XmlModelWithAttributesOnly : ModelBase
        {
            [XmlAttribute]
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
        }

#if NET
        [Serializable]
#endif
        public class XmlFamily : ModelBase
        {
            public XmlFamily()
            {
                Persons = new ObservableCollection<XmlPerson>();
                ModelsWithAttributesOnly = new ObservableCollection<XmlModelWithAttributesOnly>();
            }

            public string LastName
            {
                get { return GetValue<string>(LastNameProperty); }
                set { SetValue(LastNameProperty, value); }
            }

            public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);


            public ObservableCollection<XmlPerson> Persons
            {
                get { return GetValue<ObservableCollection<XmlPerson>>(PersonsProperty); }
                private set { SetValue(PersonsProperty, value); }
            }

            public static readonly PropertyData PersonsProperty = RegisterProperty("Persons", typeof(ObservableCollection<XmlPerson>), null);


            public ObservableCollection<XmlModelWithAttributesOnly> ModelsWithAttributesOnly
            {
                get { return GetValue<ObservableCollection<XmlModelWithAttributesOnly>>(ModelsWithAttributesOnlyProperty); }
                set { SetValue(ModelsWithAttributesOnlyProperty, value); }
            }

            public static readonly PropertyData ModelsWithAttributesOnlyProperty = RegisterProperty("ModelsWithAttributesOnly", typeof(ObservableCollection<XmlModelWithAttributesOnly>), null);
        }

#if NET
        [Serializable]
#endif
        public class XmlPerson : ModelBase
        {
            public Gender Gender
            {
                get { return GetValue<Gender>(GenderProperty); }
                set { SetValue(GenderProperty, value); }
            }

            public static readonly PropertyData GenderProperty = RegisterProperty("Gender", typeof(Gender), Data.Gender.Female);


            [XmlAttribute]
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);


            [XmlAttribute]
            public string LastName
            {
                get { return GetValue<string>(LastNameProperty); }
                set { SetValue(LastNameProperty, value); }
            }

            public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);
        }

        [TestFixture]
        public class BasicSerializationFacts
        {
            [TestCase]
            public void XmlSerializationWithXmlIgnore()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                var obj = new ObjectWithXmlMappings();

                var xml = obj.ToXml(serializer).ToString();

                Assert.IsFalse(xml.Contains("IgnoredProperty"));
            }

            [TestCase]
            public void XmlSerializationWithXmlMappings()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsWithXmlMappingsObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestCase]
            public void ReadXml()
            {
                // Should always return null
                var iniFile = ModelBaseTestHelper.CreateIniFileObject();
                Assert.AreEqual(null, ((IXmlSerializable)iniFile).GetSchema());
            }

            [TestCase]
            public void RespectsTheXmlRootAndXmlElementAttribute()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml(serializer);

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                var firstNameElement = personElement.Element("NameFirst");
                Assert.IsNotNull(firstNameElement);
                Assert.AreEqual("Geert", firstNameElement.Value);

                var middleNameElement = personElement.Element("NameMiddle");
                Assert.IsNotNull(middleNameElement);
                Assert.AreEqual("van", middleNameElement.Value);

                var lastNameElement = personElement.Element("NameLast");
                Assert.IsNotNull(lastNameElement);
                Assert.AreEqual("Horrik", lastNameElement.Value);

                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write(xmlDocument.ToString());
                        streamWriter.Flush();

                        memoryStream.Position = 0L;

                        var deserializedPerson = serializer.Deserialize<ModelBaseFacts.Person>(memoryStream);

                        Assert.AreEqual("Geert", deserializedPerson.FirstName);
                        Assert.AreEqual("van", deserializedPerson.MiddleName);
                        Assert.AreEqual("Horrik", deserializedPerson.LastName);
                    }
                }
            }

            [TestCase]
            public void RespectsTheXmlAttributeAttribute()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml(serializer);

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                var ageAttribute = personElement.Attribute("FutureAge");
                Assert.IsNotNull(ageAttribute);
                Assert.AreEqual("42", ageAttribute.Value);

                using (var memoryStream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(memoryStream))
                    {
                        streamWriter.Write(xmlDocument.ToString());
                        streamWriter.Flush();

                        memoryStream.Position = 0L;

                        var deserializedPerson = serializer.Deserialize<ModelBaseFacts.Person>(memoryStream);

                        Assert.AreEqual(42, deserializedPerson.Age);
                    }
                }
            }

            [TestCase(XmlSerializerOptimalizationMode.PrettyXml)]
            //[TestCase(XmlSerializerOptimalizationMode.PrettyXmlAgressive)]
            [TestCase(XmlSerializerOptimalizationMode.Performance)]
            public void RespectsTheXmlAttributeAttributeOnRootElements(XmlSerializerOptimalizationMode mode)
            {
                var family = new XmlFamily();
                family.LastName = "van Horrik";
                family.Persons.Add(new XmlPerson
                {
                    FirstName = "Geert",
                    LastName = family.LastName,
                    Gender = Gender.Male
                });

                var newFamily = SerializationTestHelper.SerializeAndDeserialize(family, SerializationTestHelper.GetXmlSerializer(),
                    new XmlSerializationConfiguration
                    {
                        OptimalizationMode = mode
                    });

                Assert.AreEqual(family.LastName, newFamily.LastName);
                Assert.AreEqual(1, newFamily.Persons.Count);

                var newPerson = newFamily.Persons.First();

                Assert.AreEqual(family.Persons[0].FirstName, newPerson.FirstName);
                Assert.AreEqual(family.Persons[0].LastName, newPerson.LastName);
                Assert.AreEqual(family.Persons[0].Gender, newPerson.Gender);
            }

            [TestCase(XmlSerializerOptimalizationMode.PrettyXml)]
            //[TestCase(XmlSerializerOptimalizationMode.PrettyXmlAgressive)]
            [TestCase(XmlSerializerOptimalizationMode.Performance)]
            public void SerializesModelsWithOnlyAttributes(XmlSerializerOptimalizationMode mode)
            {
                var family = new XmlFamily();
                family.LastName = "van Horrik";
                family.ModelsWithAttributesOnly.Add(new XmlModelWithAttributesOnly
                {
                    FirstName = "Geert",
                });

                var newFamily = SerializationTestHelper.SerializeAndDeserialize(family, SerializationTestHelper.GetXmlSerializer(),
                    new XmlSerializationConfiguration
                    {
                        OptimalizationMode = mode
                    });

                Assert.AreEqual(family.LastName, newFamily.LastName);
                Assert.AreEqual(1, newFamily.ModelsWithAttributesOnly.Count);

                var newModelWithAttributesOnly = newFamily.ModelsWithAttributesOnly.First();

                Assert.AreEqual(family.ModelsWithAttributesOnly[0].FirstName, newModelWithAttributesOnly.FirstName);
            }

            [TestCase]
            public void RespectsTheXmlIgnoreAttribute()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml(serializer);

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                Assert.IsNull(personElement.Element("FullName"));
            }

            [TestCase]
            public void SupportsNestedHierarchySerialization()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

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
                Assert.IsNotNull(newRoot);
                Assert.AreEqual("myRoot", newRoot.Name);
                Assert.AreEqual(1, newRoot.Items.Count);
                Assert.AreEqual("myChild", newRoot.Items[0].Name);
            }
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomXmlSerializable_Simple()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IXmlSerializer>();

                var model = new CustomXmlSerializationModel
                {
                    FirstName = "Geert"
                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.IsTrue(model.IsCustomSerialized);
                Assert.IsTrue(clonedModel.IsCustomDeserialized);

                Assert.AreEqual(model.FirstName, clonedModel.FirstName);
            }

            [TestCase]
            public void CorrectlySerializesObjectsImplementingICustomXmlSerializable_Nested()
            {
                var serviceLocator = ServiceLocator.Default;
                var serializer = serviceLocator.ResolveType<IXmlSerializer>();

                var model = new CustomXmlSerializationModelWithNesting
                {
                    Name = "Test model with nesting",
                    NestedModel = new CustomXmlSerializationModel
                    {
                        FirstName = "Geert"
                    }
                };

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, null);

                Assert.IsNotNull(clonedModel.NestedModel);

                // Note: yes, the *model* is serialized, the *clonedModel* is deserialized
                Assert.IsTrue(model.NestedModel.IsCustomSerialized);
                Assert.IsTrue(clonedModel.NestedModel.IsCustomDeserialized);

                Assert.AreEqual(model.Name, clonedModel.Name);
                Assert.AreEqual(model.NestedModel.FirstName, clonedModel.NestedModel.FirstName);
            }


            [TestCase]
            public void CorrectlySerializesToXmlString()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var xml = testModel.ToXmlString();

                Assert.IsFalse(xml.Contains("Excluded"));
            }
        }
    }
}