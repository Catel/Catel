// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    using Newtonsoft.Json;

#if NET
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
#endif

    using NUnit.Framework;

#if !NETFX_CORE
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
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

        [XmlRoot("MappedPerson")]
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

        [TestFixture]
        public class TheJsonSerialization
        {
#if NET
            [Serializable]
#endif
            [JsonObject(MemberSerialization.OptIn)]
            public class JsonInnerModel : ModelBase
            {
                /// <summary>
                /// Initializes a new object from scratch.
                /// </summary>
                public JsonInnerModel()
                {
                }

#if NET
                /// <summary>
                /// Initializes a new object based on <see cref="SerializationInfo"/>.
                /// </summary>
                /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
                /// <param name="context"><see cref="StreamingContext"/>.</param>
                protected JsonInnerModel(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
#endif

                #region Properties
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                [JsonProperty("name")]
                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                /// <summary>
                /// Register the Place property so it is known in the class.
                /// </summary>
                public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
                #endregion
            }

#if NET
            [Serializable]
#endif
            [JsonObject(MemberSerialization.OptIn)]
            public class JsonExampleModel : ModelBase
            {
                /// <summary>
                /// Initializes a new object from scratch.
                /// </summary>
                public JsonExampleModel()
                {
                }

#if NET
                /// <summary>
                /// Initializes a new object based on <see cref="SerializationInfo"/>.
                /// </summary>
                /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
                /// <param name="context"><see cref="StreamingContext"/>.</param>
                protected JsonExampleModel(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
#endif
                #region Properties
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                [JsonProperty("name")]
                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                /// <summary>
                /// Register the Place property so it is known in the class.
                /// </summary>
                public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

                /// <summary>
                /// Gets or sets the modules.
                /// </summary>
                [JsonProperty("modules")]
                public ObservableCollection<JsonInnerModel> Modules
                {
                    get { return GetValue<ObservableCollection<JsonInnerModel>>(ModulesProperty); }
                    set { SetValue(ModulesProperty, value); }
                }

                /// <summary>
                /// Register the Modules property so it is known in the class.
                /// </summary>
                public static readonly PropertyData ModulesProperty = RegisterProperty("Modules", typeof(ObservableCollection<JsonInnerModel>), () => new ObservableCollection<JsonInnerModel>());
                #endregion
            }

            [TestCase]
            public void CanSerializeToJson()
            {
                var model = new JsonExampleModel();
                model.Name = "Geert";
                for (int i = 0; i < 3; i++)
                {
                    model.Modules.Add(new JsonInnerModel { Name = "Name " + (i + 1) });
                }

                var json = JsonConvert.SerializeObject(model);

                Assert.AreEqual("{\"name\":\"Geert\",\"modules\":[{\"name\":\"Name 1\"},{\"name\":\"Name 2\"},{\"name\":\"Name 3\"}]}", json);
            }

            [TestCase]
            public void CanDeserializeFromJson()
            {
                const string json = "{ \"name\": \"Geert\" }";

                var model = JsonConvert.DeserializeObject<JsonExampleModel>(json);

                Assert.AreEqual("Geert", model.Name);
            }
        }

#if NET
        [TestFixture]
        public class TheBinarySerializationWithCircularReferencesIssue
        {
            [TestCase]
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