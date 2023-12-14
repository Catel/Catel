namespace Catel.Tests.Data
{
    using System;
    using System.Linq;
    using Catel.Data;

    using NUnit.Framework;

    public class PropertyDataManagerFacts
    {
        #region Nested type: SupportsGenericClasses
        [TestFixture]
        public class SupportsGenericClasses
        {
            #region Methods
            [TestCase]
            public void ReturnsNoPropertiesForOpenGenericTypes()
            {
                var propertyDataManager = new PropertyDataManager();
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof(GenericClass<>));

                Assert.That(catelTypeInfo.GetCatelProperties().Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsPropertiesForClosedGenericTypes()
            {
                var propertyDataManager = new PropertyDataManager();
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof(GenericClass<int>));

                Assert.That(catelTypeInfo.GetCatelProperties().Count, Is.Not.EqualTo(0));
            }
            #endregion

            #region Nested type: GenericClass
            public class GenericClass<T> : ModelBase
            {
                #region Constants
                /// <summary>
                /// Register the FirstName property so it is known in the class.
                /// </summary>
                public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
                #endregion

                #region Properties
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }
                #endregion
            }
            #endregion
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Registers the property with the property data manager. This is a shortcut method so the PropertyData doesn't have
        /// to be declared every time.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyDataManager">The property data manager.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        private static void RegisterProperty<T>(PropertyDataManager propertyDataManager, string name, T defaultValue)
        {
            propertyDataManager.RegisterProperty(typeof(PropertyDataManagerFacts), name,
                new PropertyData<T>(name, defaultValue, null, false, false, false, false, false));
        }
        #endregion

        #region Nested type: TheGetPropertiesMethod
        [TestFixture]
        public class TheGetCatelTypeInfoMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var propertyDataManager = new PropertyDataManager();
                Assert.Throws<ArgumentNullException>(() => propertyDataManager.GetCatelTypeInfo(null));
            }

            [TestCase]
            public void ReturnsRightPropertiesByType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                RegisterProperty(propertyDataManager, "objectProperty", new object());
                RegisterProperty(propertyDataManager, "intProperty", 1);

                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof(PropertyDataManagerFacts));
                var properties = catelTypeInfo.GetCatelProperties();
                var keys = properties.Keys.ToArray();
                Assert.That(properties.Count, Is.EqualTo(3));
                Assert.That(keys[0], Is.EqualTo("stringProperty"));
                Assert.That(keys[1], Is.EqualTo("objectProperty"));
                Assert.That(keys[2], Is.EqualTo("intProperty"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetPropertyDataMethod
        [TestFixture]
        public class TheGetPropertyDataMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentNullException>(() => propertyDataManager.GetPropertyData(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), null));
                Assert.Throws<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ThrowsPropertyNotRegisteredExceptionForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                Assert.Throws<PropertyNotRegisteredException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), "stringProperty"));
            }

            [TestCase]
            public void ReturnsPropertyDataForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                var propertyData = propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), "stringProperty");
                Assert.That(propertyData, Is.Not.Null);
                Assert.That(propertyData.Name, Is.EqualTo("stringProperty"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsPropertyRegisteredMethod
        [TestFixture]
        public class TheIsPropertyRegisteredMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentNullException>(() => propertyDataManager.IsPropertyRegistered(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), null));
                Assert.Throws<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsTrueForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.That(propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), "stringProperty"), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                Assert.That(propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), "stringProperty"), Is.False);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheMapPropertyNameToXmlElementNameMethod
        [TestFixture]
        public class TheMapPropertyNameToXmlElementNameMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentNullException>(() => propertyDataManager.MapPropertyNameToXmlElementName(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof(PropertyDataManagerFacts), null));
                Assert.Throws<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsRightXmlNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof(ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.That(xmlName, Is.EqualTo("PropertyWithoutMapping"));
            }

            [TestCase]
            public void ReturnsRightXmlNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof(ObjectWithXmlMappings), "PropertyWithMapping");
                Assert.That(xmlName, Is.EqualTo("MappedXmlProperty"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheMapXmlElementNameToPropertyNameMethod
        [TestFixture]
        public class TheMapXmlElementNameToPropertyNameMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentNullException>(() => propertyDataManager.MapXmlElementNameToPropertyName(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.Throws<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof(PropertyDataManagerFacts), null));
                Assert.Throws<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsRightPropertyNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof(ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.That(propertyName, Is.EqualTo("PropertyWithoutMapping"));
            }

            [TestCase]
            public void ReturnsRightPropertyNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof(ObjectWithXmlMappings), "MappedXmlProperty");
                Assert.That(propertyName, Is.EqualTo("PropertyWithMapping"));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheRegisterPropertyMethod
        [TestFixture]
        public class TheRegisterPropertyMethod
        {
            #region Methods
            [TestCase]
            public void SuccessfullyRegistersProperty()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof(PropertyDataManagerFacts));
                Assert.That(catelTypeInfo.GetCatelProperties().Count, Is.EqualTo(1));
                Assert.That(catelTypeInfo.GetPropertyData("stringProperty").GetDefaultValue(), Is.EqualTo("defaultValue"));
            }

            [TestCase]
            public void ThrowsPropertyAlreadyRegisteredExceptionForDoubleRegistration()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                Assert.Throws<PropertyAlreadyRegisteredException>(() => RegisterProperty(propertyDataManager, "stringProperty", "defaultValue"));
            }
            #endregion
        }
        #endregion
    }
}
