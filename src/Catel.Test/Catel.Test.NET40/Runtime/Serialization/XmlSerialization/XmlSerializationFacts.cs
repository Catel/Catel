// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Catel.Test.Data;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class XmlSerializerFacts
    {
        [TestClass]
        public class BasicSerializationFacts
        {
            [TestMethod]
            public void XmlSerializationLevel1()
            {
                var originalObject = ModelBaseTestHelper.CreateIniEntryObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestMethod]
            public void XmlSerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestMethod]
            public void XmlSerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestMethod]
            public void XmlSerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestMethod]
            public void XmlSerializationWithXmlIgnore()
            {
                var obj = new ObjectWithXmlMappings();

                string xml = obj.ToXml().ToString();

                Assert.IsFalse(xml.Contains("IgnoredProperty"));
            }

            [TestMethod]
            public void XmlSerializationWithXmlMappings()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsWithXmlMappingsObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestMethod]
            public void XmlSerializationWithCustomTypes()
            {
                // Create object
                var originalObject = new ObjectWithCustomType();
                originalObject.FirstName = "Test";
                originalObject.Gender = Gender.Female;

                // Serialize and deserialize
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

#if NET
            [TestMethod]
            public void XmlSerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }
#endif

            [TestMethod]
            public void ReadXml()
            {
                // Should always return null
                var iniFile = ModelBaseTestHelper.CreateIniFileObject();
                Assert.AreEqual(null, ((IXmlSerializable)iniFile).GetSchema());
            }

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
            public void RespectsTheXmlIgnoreAttribute()
            {
                var person = new ModelBaseFacts.Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("MappedPerson");
                Assert.IsNotNull(personElement);

                Assert.IsNull(personElement.Element("FullName"));
            }

            [TestMethod]
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

            [TestMethod]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexHierarchy();

                var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, SerializationFactory.GetXmlSerializer());

                Assert.IsTrue(complexHierarchy == deserializedObject);
            }
        }

        [TestClass]
        public class AdvancedSerializationFacts
        {
            public abstract class AbstractBase : ModelBase
            {
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                /// <summary>
                /// Register the Name property so it is known in the class.
                /// </summary>
                public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
            }

            public class DerivedClass : AbstractBase
            {
            }

            public class ContainerClass : ModelBase
            {
                public ContainerClass()
                {
                    Items = new ObservableCollection<AbstractBase>();
                }

                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public ObservableCollection<AbstractBase> Items
                {
                    get { return GetValue<ObservableCollection<AbstractBase>>(ItemsProperty); }
                    set { SetValue(ItemsProperty, value); }
                }

                /// <summary>
                /// Register the name property so it is known in the class.
                /// </summary>
                public static readonly PropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<AbstractBase>), null);
            }

            [TestMethod]
            public void CorrectlySerializesCustomizedModels()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                testModel.SetValue(TestModel.ExcludedProtectedCatelPropertyProperty, "excluded");

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(null, clonedModel._excludedField);
                Assert.AreEqual("included", clonedModel._includedField);

                Assert.AreEqual(null, clonedModel.ExcludedRegularProperty);
                Assert.AreEqual("included", clonedModel.IncludedRegularProperty);

                Assert.AreEqual(null, clonedModel.ExcludedCatelProperty);
                Assert.AreEqual("included", clonedModel.IncludedCatelProperty);

                Assert.AreEqual(null, clonedModel.GetValue(TestModel.ExcludedProtectedCatelPropertyProperty.Name));
            }

            [TestMethod]
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

            [TestMethod]
            public void CorrectlyHandlesSerializationOfCollectionsWithAbstractClasses()
            {
                var collection = new ContainerClass();
                collection.Items.Add(new DerivedClass { Name = "item 1" });

                var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(collection, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(collection, clonedGraph);
            }

            [TestMethod]
            public void CorrectlyHandlesNullValues()
            {
                var testModel = new TestModel();

                testModel.IncludedCatelProperty = null;

                var editableObject = testModel as IEditableObject;
                editableObject.BeginEdit();

                testModel.IncludedCatelProperty = "included";

                editableObject.CancelEdit();

                Assert.IsNull(testModel.IncludedCatelProperty);
            }

            // Note: Disabled because it has too much impact on other tests
            //[TestMethod]
            //public void CorrectlyHandlesCustomizedValuesDuringSerialization()
            //{
            //    var testModel = new TestModel();
            //    testModel.IncludedCatelProperty = "BeforeSerializingMember";

            //    var xmlSerializer = ServiceLocator.Default.ResolveType<IXmlSerializer>();
            //    xmlSerializer.SerializingMember += (sender, e) =>
            //    {
            //        if (ReferenceEquals(e.SerializationContext.Model, testModel))
            //        {
            //            if (e.MemberValue.Name == "IncludedCatelProperty")
            //            {
            //                Assert.AreEqual("BeforeSerializingMember", e.MemberValue.Value);

            //                e.MemberValue.Value = "AfterSerializingMember";
            //            }
            //        }
            //    };

            //    xmlSerializer.SerializedMember += (sender, e) =>
            //    {
            //        if (ReferenceEquals(e.SerializationContext.Model, testModel))
            //        {
            //            if (e.MemberValue.Name == "IncludedCatelProperty")
            //            {
            //                Assert.AreEqual("AfterSerializingMember", e.MemberValue.Value);
            //            }
            //        }
            //    };

            //    xmlSerializer.DeserializedMember += (sender, e) =>
            //    {
            //        if (e.MemberValue.Name == "IncludedCatelProperty")
            //        {
            //            Assert.AreEqual("AfterSerializingMember", e.MemberValue.Value);

            //            e.MemberValue.Value = "AfterDeserializedMember";
            //        }
            //    };

            //    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetXmlSerializer());

            //    Assert.AreEqual("AfterDeserializedMember", clonedModel.IncludedCatelProperty);
            //}

            [TestMethod]
            public void CorrectlyHandlesSameInstancesInGraph()
            {
                var graph = SerializationTestHelper.CreateComplexCircularTestModelGraph();

                var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, SerializationFactory.GetXmlSerializer());

                Assert.IsNotNull(clonedGraph);
                Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel));
            }
        }

        [TestClass]
        public class TheWarmupMethod
        {
            [TestMethod]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new Type[] { typeof(CircularTestModel), typeof(TestModel) };

                var serializer = SerializationFactory.GetXmlSerializer();

                TimeMeasureHelper.MeasureAction(5, "Xml serializer warmup",
                    () => serializer.Warmup(typesToWarmup),
                    () =>
                    {
                        TypeCache.InitializeTypes(false);

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestMethod]
            public void WarmsUpAllTypes()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                TimeMeasureHelper.MeasureAction(5, "Xml serializer warmup",
                    () => serializer.Warmup(),
                    () =>
                    {
                        TypeCache.InitializeTypes(false);

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }
    }
}