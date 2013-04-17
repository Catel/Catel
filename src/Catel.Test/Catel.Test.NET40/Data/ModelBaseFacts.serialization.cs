// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseBaseFacts.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Catel.Data;
    using Catel.Logging;

#if NET
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
#endif

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public partial class ModelBaseFacts
    {
        public class Item : SavableModelBase<Item>
        {
            #region Fields
            #endregion

            #region Constructors
            /// <summary> 
            /// Initializes a new object from scratch.
            /// </summary> 
            public Item()
            { }
            #endregion

            #region Properties
            /// <summary> 
            /// Gets or sets the name.
            /// </summary> 
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            /// <summary> 
            /// Register the Name property so it is known in the class.
            /// </summary> 
            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), "MyName");
            #endregion

            #region Methods
            #endregion
        }

        public class Group : Item
        {
            #region Fields
            #endregion

            #region Constructors
            /// <summary> 
            /// Initializes a new object from scratch.
            /// </summary> 
            public Group()
            { }
            #endregion

            #region Properties
            /// <summary> 
            /// Gets or sets the name.
            /// </summary> 
            public ObservableCollection<Item> Items
            {
                get { return GetValue<ObservableCollection<Item>>(ItemsProperty); }
                set { SetValue(ItemsProperty, value); }
            }

            /// <summary> 
            /// Register the Name property so it is known in the class.
            /// </summary> 
            public static readonly PropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<Item>), null);
            #endregion

            #region Methods
            #endregion
        }

        public class Person : SavableModelBase<Person>
        {
            public Person()
            {

            }

            public Person(string firstName, string middleName, string lastName, int age)
            {
                FirstName = firstName;
                MiddleName = middleName;
                LastName = lastName;
                Age = age;
            }

            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            [XmlElement(ElementName = "NameFirst")]
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            /// <summary>
            /// Register the FirstName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));

            /// <summary>
            /// Gets or sets the middle name.
            /// </summary>
            [XmlElement(ElementName = "NameMiddle")]
            public string MiddleName
            {
                get { return GetValue<string>(MiddleNameProperty); }
                set { SetValue(MiddleNameProperty, value); }
            }

            /// <summary>
            /// Register the MiddleName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof(string));

            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            [XmlElement(ElementName = "NameLast")]
            public string LastName
            {
                get { return GetValue<string>(LastNameProperty); }
                set { SetValue(LastNameProperty, value); }
            }

            /// <summary>
            /// Register the LastName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string));

            /// <summary>
            /// Gets or sets the age.
            /// </summary>
            [XmlAttribute(AttributeName = "FutureAge")]
            public int Age
            {
                get { return GetValue<int>(AgeProperty); }
                set { SetValue(AgeProperty, value); }
            }

            /// <summary>
            /// Register the Age property so it is known in the class.
            /// </summary>
            public static readonly PropertyData AgeProperty = RegisterProperty("Age", typeof(int));

            [XmlIgnore]
            public string FullName
            {
                get { return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName); }
            }
        }

        #region Nested type: Customer
        /// <summary>
        /// Customer
        /// </summary>
#if NET
        [Serializable]
#endif
        public partial class Customer : ModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new object from scratch.
            /// </summary>
            public Customer()
            {
            }

#if NET
            /// <summary>
            /// Initializes a new object based on <see cref="SerializationInfo"/>.
            /// </summary>
            /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
            /// <param name="context"><see cref="StreamingContext"/>.</param>
            protected Customer(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the Id.
            /// </summary>
            /// <value>
            /// The Id.
            /// </value>
            public virtual int Id
            {
                get { return GetValue<int>(IdProperty); }
                set { SetValue(IdProperty, value); }
            }

            /// <summary>
            /// Register the ID property so it is known in the class.
            /// </summary>
            public static readonly PropertyData IdProperty = RegisterProperty("Id", typeof(int), null);

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public virtual string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            /// <summary>
            /// Register the Name property so it is known in the class.
            /// </summary>
            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

            /// <summary>
            /// Gets or sets the projects.
            /// </summary>
            /// <value>
            /// The projects.
            /// </value>
            public virtual ObservableCollection<Project> Projects
            {
                get { return GetValue<ObservableCollection<Project>>(ProjectsProperty); }
                set { SetValue(ProjectsProperty, value); }
            }

            /// <summary>
            /// Register the Projects property so it is known in the class.
            /// </summary>
            public static readonly PropertyData ProjectsProperty = RegisterProperty("Projects", typeof(ObservableCollection<Project>),
                () => new ObservableCollection<Project>());
            #endregion
        }
        #endregion

        #region Nested type: Project
        /// <summary>
        /// Project
        /// </summary>
#if NET
        [Serializable]
#endif
        public class Project : ModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new object from scratch.
            /// </summary>
            public Project()
            {
            }

#if NET
            /// <summary>
            /// Initializes a new object based on <see cref="SerializationInfo"/>.
            /// </summary>
            /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
            /// <param name="context"><see cref="StreamingContext"/>.</param>
            protected Project(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#endif
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the Id.
            /// </summary>
            /// <value>
            /// The Id.
            /// </value>
            public virtual int Id
            {
                get { return GetValue<int>(IdProperty); }
                set { SetValue(IdProperty, value); }
            }

            /// <summary>
            /// Register the ID property so it is known in the class.
            /// </summary>
            public static readonly PropertyData IdProperty = RegisterProperty("Id", typeof(int), null);

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public virtual string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            /// <summary>
            /// Register the Name property so it is known in the class.
            /// </summary>
            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

            /// <summary>
            /// Gets or sets the CustomerId.
            /// </summary>
            /// <value>
            /// The CustomerId.
            /// </value>
            public virtual int CustomerId
            {
                get { return GetValue<int>(CustomerIdProperty); }
                set { SetValue(CustomerIdProperty, value); }
            }

            /// <summary>
            /// Register the CustomerId property so it is known in the class.
            /// </summary>
            public static readonly PropertyData CustomerIdProperty = RegisterProperty("CustomerId", typeof(int), null);

            /// <summary>
            /// Gets or sets the customer.
            /// </summary>
            /// <value>
            /// The customer.
            /// </value>
            [XmlIgnore]
            public virtual Customer Customer
            {
                get { return GetValue<Customer>(CustomerProperty); }
                set { SetValue(CustomerProperty, value); }
            }

            /// <summary>
            /// Register the Customer property so it is known in the class.
            /// </summary>
            public static readonly PropertyData CustomerProperty = RegisterProperty("Customer", typeof(Customer), null);
            #endregion
        }
        #endregion

        // Disabled because Fody weaver is disabled
//#if NET
//        [TestClass]
//        public class TheGetXmlSchema
//        {
//            [TestMethod]
//            public void GetsWeavedAndCallsXmlSchemaManager()
//            {
//                Type type = typeof (Group);

//                var xmlSchemaProviderAttribute = (from attribute in type.GetCustomAttributes(typeof (XmlSchemaProviderAttribute), false)
//                                                  select attribute as XmlSchemaProviderAttribute).FirstOrDefault();
//                Assert.IsNotNull(xmlSchemaProviderAttribute);

//                var methodName = xmlSchemaProviderAttribute.MethodName;
//                var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);

//                Assert.IsNotNull(method);

//                var xmlSchemaSet = new XmlSchemaSet();
//                var xmlResult = (XmlQualifiedName)method.Invoke(null, new object[] { xmlSchemaSet });
//                Assert.IsNotNull(xmlResult);
//            }
//        }
//#endif

        [TestClass]
        public class TheXmlSerialization
        {
            [TestMethod]
            public void RespectsTheXmlElementAttribute()
            {
                var person = new Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("Person");
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

                var deserializedPerson = Person.Load(xmlDocument);
                Assert.AreEqual("Geert", deserializedPerson.FirstName);
                Assert.AreEqual("van", deserializedPerson.MiddleName);
                Assert.AreEqual("Horrik", deserializedPerson.LastName);
            }

            [TestMethod]
            public void RespectsTheXmlAttributeAttribute()
            {
                var person = new Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("Person");
                Assert.IsNotNull(personElement);

                var ageAttribute = personElement.Attribute("FutureAge");
                Assert.IsNotNull(ageAttribute);
                Assert.AreEqual("42", ageAttribute.Value);

                var deserializedPerson = Person.Load(xmlDocument);
                Assert.AreEqual(42, deserializedPerson.Age);
            }

            [TestMethod]
            public void RespectsTheXmlIgnoreAttribute()
            {
                var person = new Person("Geert", "van", "Horrik", 42);
                var xmlDocument = person.ToXml();

                var personElement = xmlDocument.Element("Person");
                Assert.IsNotNull(personElement);

                Assert.IsNull(personElement.Element("FullName"));
            }

            [TestMethod]
            public void SupportsNestedHierarchySerialization()
            {
                LogManager.RegisterDebugListener();

                var root = new Group()
                {
                    Name = "myRoot"
                };

                var child = new Group()
                {
                    Name = "myChild"
                };

                root.Items = new ObservableCollection<Item>();
                root.Items.Add(child);

                var xmlDocument = root.ToXml();

                var newRoot = Group.Load<Group>(xmlDocument);
                Assert.IsNotNull(newRoot);
                Assert.AreEqual("myRoot", newRoot.Name);
                Assert.AreEqual(1, newRoot.Items.Count);
                Assert.AreEqual("myChild", newRoot.Items[0].Name);
            }
        }

#if NET
        [TestClass]
        public class TheBinarySerializationWithCustomSerialization
        {
            [Serializable]
            public class BinarySerializableObjectWithCustomCode : ModelBase
            {
                public BinarySerializableObjectWithCustomCode() { }

                public BinarySerializableObjectWithCustomCode(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }

                protected override object GetPropertyValueForSerialization(PropertyData property, object propertyValue)
                {
                    if (property == OnlySerializedDifferentlyProperty)
                    {
                        return string.Format("{0}_CUSTOMSERIALIZATION", propertyValue);
                    }

                    if (property == CustomSerializationProperty)
                    {
                        return string.Format("{0}_CUSTOMSERIALIZATION", propertyValue);
                    }

                    return propertyValue;
                }

                protected override object GetPropertyValueForDeserialization(PropertyData property, object serializedValue)
                {
                    if (property == OnlyDeserializedDifferentlyProperty)
                    {
                        return string.Format("{0}_CUSTOMDESERIALIZATION", serializedValue);
                    }

                    if (property == CustomSerializationProperty)
                    {
                        return ((string)serializedValue).Replace("_CUSTOMSERIALIZATION", string.Empty);
                    }

                    return serializedValue;
                }

                public string OnlySerializedDifferently
                {
                    get { return GetValue<string>(OnlySerializedDifferentlyProperty); }
                    set { SetValue(OnlySerializedDifferentlyProperty, value); }
                }

                public static readonly PropertyData OnlySerializedDifferentlyProperty = RegisterProperty("OnlySerializedDifferently", typeof(string), string.Empty);

                public string OnlyDeserializedDifferently
                {
                    get { return GetValue<string>(OnlyDeserializedDifferentlyProperty); }
                    set { SetValue(OnlyDeserializedDifferentlyProperty, value); }
                }

                public static readonly PropertyData OnlyDeserializedDifferentlyProperty = RegisterProperty("OnlyDeserializedDifferently", typeof(string), string.Empty);

                public string CustomSerialization
                {
                    get { return GetValue<string>(CustomSerializationProperty); }
                    set { SetValue(CustomSerializationProperty, value); }
                }

                public static readonly PropertyData CustomSerializationProperty = RegisterProperty("CustomSerialization", typeof(string), string.Empty);
            }

            [TestMethod]
            public void SupportsCustomSerializationOnly()
            {
                var model = new BinarySerializableObjectWithCustomCode();
                model.OnlySerializedDifferently = "mytest";

                BinarySerializableObjectWithCustomCode restoredModel = null;

                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, model);

                    memoryStream.Position = 0L;

                    restoredModel = (BinarySerializableObjectWithCustomCode)binaryFormatter.Deserialize(memoryStream);
                }

                Assert.AreEqual("mytest_CUSTOMSERIALIZATION", restoredModel.OnlySerializedDifferently);
            }

            [TestMethod]
            public void SupportsCustomDeserializationOnly()
            {
                var model = new BinarySerializableObjectWithCustomCode();
                model.OnlyDeserializedDifferently = "mytest";

                BinarySerializableObjectWithCustomCode restoredModel = null;

                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, model);

                    memoryStream.Position = 0L;

                    restoredModel = (BinarySerializableObjectWithCustomCode)binaryFormatter.Deserialize(memoryStream);
                }

                Assert.AreEqual("mytest_CUSTOMDESERIALIZATION", restoredModel.OnlyDeserializedDifferently);
            }

            [TestMethod]
            public void SupportsFullCustomSerialization()
            {
                var model = new BinarySerializableObjectWithCustomCode();
                model.CustomSerialization = "mytest";

                BinarySerializableObjectWithCustomCode restoredModel = null;

                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, model);

                    memoryStream.Position = 0L;

                    restoredModel = (BinarySerializableObjectWithCustomCode)binaryFormatter.Deserialize(memoryStream);
                }

                Assert.AreEqual("mytest", restoredModel.CustomSerialization);
            }
        }

        [TestClass]
        public class TheBinarySerializationWithCircularReferencesIssue
        {
            [TestMethod]
            public void CanSerializeAndDeserializeObjects()
            {
                var customer = new Customer();
                customer.Id = 1;
                customer.Name = "John Doe";

                var catelProject = new Project();
                catelProject.Id = 1;
                catelProject.Name = "Catel";
                catelProject.Customer = customer;

                Project restoredProject = null;

                var binaryFormatter = new BinaryFormatter();
                using (var memoryStream = new MemoryStream())
                {
                    binaryFormatter.Serialize(memoryStream, catelProject);

                    memoryStream.Position = 0L;

                    restoredProject = (Project)binaryFormatter.Deserialize(memoryStream);
                }

                var projectAsEditableObject = catelProject as IEditableObject;
                projectAsEditableObject.BeginEdit();

                projectAsEditableObject.BeginEdit();

                Assert.AreEqual(catelProject, restoredProject);
            }
        }
#endif
    }
}