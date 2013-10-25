namespace Catel.Test.Data
{
    using System;
    using System.Linq;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif


    public class PropertyDataManagerFacts
    {
        [TestClass]
        public class TheGetPropertiesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.GetProperties(null));
            }

            [TestMethod]
            public void ReturnsRightPropertiesByType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                RegisterProperty(propertyDataManager, "objectProperty", new object());
                RegisterProperty(propertyDataManager, "intProperty", 1);

                var registeredProperties = propertyDataManager.GetProperties(typeof(PropertyDataManagerFacts));
                var keys = registeredProperties.Keys.ToArray();
                Assert.AreEqual(3, registeredProperties.Count);
                Assert.AreEqual("stringProperty", keys[0]);
                Assert.AreEqual("objectProperty", keys[1]);
                Assert.AreEqual("intProperty", keys[2]);
            }
        }

        [TestClass]
        public class TheRegisterPropertyMethod
        {
            [TestMethod]
            public void SuccessfullyRegistersProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                var registeredProperties = propertyDataManager.GetProperties(typeof(PropertyDataManagerFacts));
                Assert.AreEqual(1, registeredProperties.Count);
                Assert.AreEqual("defaultValue", registeredProperties["stringProperty"].GetDefaultValue());
            }

            [TestMethod]
            public void ThrowsPropertyAlreadyRegisteredExceptionForDoubleRegistration()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                ExceptionTester.CallMethodAndExpectException<PropertyAlreadyRegisteredException>(() => RegisterProperty(propertyDataManager, "stringProperty", "defaultValue"));
            }
        }

        [TestClass]
        public class TheIsPropertyRegisteredMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.IsPropertyRegistered(null, "stringProperty"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestMethod]
            public void ReturnsTrueForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.IsTrue(propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), "stringProperty"));
            }

            [TestMethod]
            public void ReturnsFalseForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                Assert.IsFalse(propertyDataManager.IsPropertyRegistered(typeof(PropertyDataManagerFacts), "stringProperty"));
            }
        }

        [TestClass]
        public class TheGetPropertyDataMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.GetPropertyData(null, "stringProperty"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestMethod]
            public void ThrowsPropertyNotRegisteredExceptionForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                ExceptionTester.CallMethodAndExpectException<PropertyNotRegisteredException>(() => propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), "stringProperty"));
            }

            [TestMethod]
            public void ReturnsPropertyDataForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                var propertyData = propertyDataManager.GetPropertyData(typeof(PropertyDataManagerFacts), "stringProperty");
                Assert.IsNotNull(propertyData);
                Assert.AreEqual("stringProperty", propertyData.Name);
            }
        }

        [TestClass]
        public class TheMapXmlElementNameToPropertyNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.MapXmlElementNameToPropertyName(null, "stringProperty"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof(PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestMethod]
            public void ReturnsRightPropertyNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof(ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.AreEqual("PropertyWithoutMapping", propertyName);
            }

            [TestMethod]
            public void ReturnsRightPropertyNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof(ObjectWithXmlMappings), "MappedXmlProperty");
                Assert.AreEqual("PropertyWithMapping", propertyName);
            }
        }

        [TestClass]
        public class TheMapPropertyNameToXmlElementNameMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.MapPropertyNameToXmlElementName(null, "stringProperty"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof(PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof(PropertyDataManagerFacts), string.Empty));
            }

            [TestMethod]
            public void ReturnsRightXmlNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof(ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.AreEqual("PropertyWithoutMapping", xmlName);
            }

            [TestMethod]
            public void ReturnsRightXmlNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof(ObjectWithXmlMappings), "PropertyWithMapping");
                Assert.AreEqual("MappedXmlProperty", xmlName);
            }
        }

        [TestClass]
        public class SupportsGenericClasses
        {
            public class GenericClass<T> : ModelBase
            {
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                /// <summary>
                /// Register the FirstName property so it is known in the class.
                /// </summary>
                public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
            }

            [TestMethod]
            public void ReturnsNoPropertiesForOpenGenericTypes()
            {
                var propertyDataManager = new PropertyDataManager();
                var properties = propertyDataManager.GetProperties(typeof(GenericClass<>));

                Assert.AreEqual(0, properties.Count);
            }

            [TestMethod]
            public void ReturnsPropertiesForClosedGenericTypes()
            {
                var propertyDataManager = new PropertyDataManager();
                var properties = propertyDataManager.GetProperties(typeof(GenericClass<int>));

                Assert.AreNotEqual(0, properties.Count);
            }
        }

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
                new PropertyData(name, typeof(T), defaultValue, false, null, false, false, false, false, false));
        }
        #endregion
    }
}
