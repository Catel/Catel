namespace Catel.Tests.Data
{
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Serialization;
    using Catel.Data;
    using Newtonsoft.Json;
    using System;
    using NUnit.Framework;

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
            public static readonly IPropertyData NameProperty = RegisterProperty<string>("Name", "MyName");
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
            public static readonly IPropertyData ItemsProperty = RegisterProperty<ObservableCollection<Item>>("Items");
            #endregion

            #region Methods
            #endregion
        }

        [XmlRoot("MappedPerson")]
        public class Person : ComparableModelBase
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
            public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName");

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
            public static readonly IPropertyData MiddleNameProperty = RegisterProperty<string>("MiddleName");

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
            public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("LastName");

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
            public static readonly IPropertyData AgeProperty = RegisterProperty<int>("Age");

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
        [Serializable]
        public partial class Customer : ComparableModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new object from scratch.
            /// </summary>
            public Customer()
            {
            }
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
            public static readonly IPropertyData IdProperty = RegisterProperty<int>("Id");

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
            public static readonly IPropertyData NameProperty = RegisterProperty<string>("Name");

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
            public static readonly IPropertyData ProjectsProperty = RegisterProperty<ObservableCollection<Project>>("Projects", () => new ObservableCollection<Project>());
            #endregion
        }
        #endregion

        #region Nested type: Project
        /// <summary>
        /// Project
        /// </summary>
        [Serializable]
        public class Project : ComparableModelBase
        {
            #region Constructors
            /// <summary>
            /// Initializes a new object from scratch.
            /// </summary>
            public Project()
            {
            }
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
            public static readonly IPropertyData IdProperty = RegisterProperty("Id", 0);

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
            public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);

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
            public static readonly IPropertyData CustomerIdProperty = RegisterProperty("CustomerId", 0);

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
            public static readonly IPropertyData CustomerProperty = RegisterProperty<Customer>("Customer");
            #endregion
        }
        #endregion

        [TestFixture]
        public class TheJsonSerialization
        {
            [Serializable]
            [JsonObject(MemberSerialization.OptIn)]
            public class JsonInnerModel : ModelBase
            {
                /// <summary>
                /// Initializes a new object from scratch.
                /// </summary>
                public JsonInnerModel()
                {
                }


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
                public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);
                #endregion
            }

            [Serializable]
            [JsonObject(MemberSerialization.OptIn)]
            public class JsonExampleModel : ModelBase
            {
                /// <summary>
                /// Initializes a new object from scratch.
                /// </summary>
                public JsonExampleModel()
                {
                }

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
                public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);

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
                public static readonly IPropertyData ModulesProperty = RegisterProperty<ObservableCollection<JsonInnerModel>>("Modules", () => new ObservableCollection<JsonInnerModel>());
                #endregion
            }

            [TestCase]
            public void CanSerializeToJson()
            {
                var model = new JsonExampleModel();
                model.Name = "Geert";
                for (int i = 0; i < 3; i++)
                {
                    model.Modules.Add(new JsonInnerModel { Name = "Name " + BoxingCache.GetBoxedValue(i + 1) });
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
    }
}
