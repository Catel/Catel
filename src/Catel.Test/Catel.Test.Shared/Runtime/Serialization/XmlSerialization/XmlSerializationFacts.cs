// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using Catel.Logging;
    using Catel.Runtime.Serialization;
    using Data;
    using NUnit.Framework;

    public class XmlSerializerFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
            [TestCase]
            public void XmlSerializationWithXmlIgnore()
            {
                var obj = new ObjectWithXmlMappings();

                var xml = obj.ToXml().ToString();

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
                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

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

                var deserializedPerson = ModelBaseFacts.Person.Load(xmlDocument);
                Assert.AreEqual("Geert", deserializedPerson.FirstName);
                Assert.AreEqual("van", deserializedPerson.MiddleName);
                Assert.AreEqual("Horrik", deserializedPerson.LastName);
            }

            [TestCase]
            public void RespectsTheXmlAttributeAttribute()
            {
                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                var ageAttribute = personElement.Attribute("FutureAge");
                Assert.IsNotNull(ageAttribute);
                Assert.AreEqual("42", ageAttribute.Value);

                var deserializedPerson = ModelBaseFacts.Person.Load(xmlDocument);
                Assert.AreEqual(42, deserializedPerson.Age);
            }

            [TestCase]
            public void RespectsTheXmlIgnoreAttribute()
            {
                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                Assert.IsNull(personElement.Element("FullName"));
            }

            [TestCase]
            public void SupportsNestedHierarchySerialization()
            {
                LogManager.AddDebugListener();

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

                var xmlDocument = root.ToXml();

                var newRoot = ModelBaseFacts.Group.Load<ModelBaseFacts.Group>(xmlDocument);
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