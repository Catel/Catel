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
    using System.Linq;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Runtime.Serialization;
    using Data;
    using NUnit.Framework;

    public class XmlSerializerFacts
    {

#if NET
        [Serializable]
#endif
        public class XmlFamily : ModelBase
        {
            public XmlFamily()
            {
                Persons = new ObservableCollection<XmlPerson>();
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
            public void RespectsTheXmlAttributeAttributeOnRootElements()
            {
                var family = new XmlFamily();
                family.LastName = "van Horrik";
                family.Persons.Add(new XmlPerson
                {
                    FirstName = "Geert",
                    LastName = family.LastName,
                    Gender = Gender.Male 
                });

                var newFamily = SerializationTestHelper.SerializeAndDeserialize(family, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(family.LastName, newFamily.LastName);
                Assert.AreEqual(1, family.Persons.Count);

                var newPerson = newFamily.Persons.First();

                Assert.AreEqual(family.Persons[0].FirstName, newPerson.FirstName);
                Assert.AreEqual(family.Persons[0].LastName, newPerson.LastName);
                Assert.AreEqual(family.Persons[0].Gender, newPerson.Gender);
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