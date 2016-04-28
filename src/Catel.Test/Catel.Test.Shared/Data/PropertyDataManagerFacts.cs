// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
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
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof (GenericClass<>));

                Assert.AreEqual(0, catelTypeInfo.GetCatelProperties().Count);
            }

            [TestCase]
            public void ReturnsPropertiesForClosedGenericTypes()
            {
                var propertyDataManager = new PropertyDataManager();
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof(GenericClass<int>));

                Assert.AreNotEqual(0, catelTypeInfo.GetCatelProperties().Count);
            }
            #endregion

            #region Nested type: GenericClass
            public class GenericClass<T> : ModelBase
            {
                #region Constants
                /// <summary>
                /// Register the FirstName property so it is known in the class.
                /// </summary>
                public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof (string), null);
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
            propertyDataManager.RegisterProperty(typeof (PropertyDataManagerFacts), name,
                new PropertyData(name, typeof (T), defaultValue, false, null, false, false, false, false, false, false));
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
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.GetCatelTypeInfo(null));
            }

            [TestCase]
            public void ReturnsRightPropertiesByType()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                RegisterProperty(propertyDataManager, "objectProperty", new object());
                RegisterProperty(propertyDataManager, "intProperty", 1);

                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof (PropertyDataManagerFacts));
                var properties = catelTypeInfo.GetCatelProperties();
                var keys = properties.Keys.ToArray();
                Assert.AreEqual(3, properties.Count);
                Assert.AreEqual("stringProperty", keys[0]);
                Assert.AreEqual("objectProperty", keys[1]);
                Assert.AreEqual("intProperty", keys[2]);
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.GetPropertyData(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof (PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.GetPropertyData(typeof (PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ThrowsPropertyNotRegisteredExceptionForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                ExceptionTester.CallMethodAndExpectException<PropertyNotRegisteredException>(() => propertyDataManager.GetPropertyData(typeof (PropertyDataManagerFacts), "stringProperty"));
            }

            [TestCase]
            public void ReturnsPropertyDataForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                var propertyData = propertyDataManager.GetPropertyData(typeof (PropertyDataManagerFacts), "stringProperty");
                Assert.IsNotNull(propertyData);
                Assert.AreEqual("stringProperty", propertyData.Name);
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.IsPropertyRegistered(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof (PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.IsPropertyRegistered(typeof (PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsTrueForRegisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                Assert.IsTrue(propertyDataManager.IsPropertyRegistered(typeof (PropertyDataManagerFacts), "stringProperty"));
            }

            [TestCase]
            public void ReturnsFalseForUnregisteredProperty()
            {
                PropertyDataManager propertyDataManager = new PropertyDataManager();

                Assert.IsFalse(propertyDataManager.IsPropertyRegistered(typeof (PropertyDataManagerFacts), "stringProperty"));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.MapPropertyNameToXmlElementName(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof (PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapPropertyNameToXmlElementName(typeof (PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsRightXmlNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof (ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.AreEqual("PropertyWithoutMapping", xmlName);
            }

            [TestCase]
            public void ReturnsRightXmlNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string xmlName = ObjectWithXmlMappings.PropertyDataManager.MapPropertyNameToXmlElementName(typeof (ObjectWithXmlMappings), "PropertyWithMapping");
                Assert.AreEqual("MappedXmlProperty", xmlName);
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

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => propertyDataManager.MapXmlElementNameToPropertyName(null, "stringProperty"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullAndEmptyName()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof (PropertyDataManagerFacts), null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyDataManager.MapXmlElementNameToPropertyName(typeof (PropertyDataManagerFacts), string.Empty));
            }

            [TestCase]
            public void ReturnsRightPropertyNameWithoutMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof (ObjectWithXmlMappings), "PropertyWithoutMapping");
                Assert.AreEqual("PropertyWithoutMapping", propertyName);
            }

            [TestCase]
            public void ReturnsRightPropertyNameWithMappings()
            {
                // Required to have properties registered
                var objectWithXmlMappings = new ObjectWithXmlMappings();
                objectWithXmlMappings.ToString();

                string propertyName = ObjectWithXmlMappings.PropertyDataManager.MapXmlElementNameToPropertyName(typeof (ObjectWithXmlMappings), "MappedXmlProperty");
                Assert.AreEqual("PropertyWithMapping", propertyName);
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

                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(typeof (PropertyDataManagerFacts));
                Assert.AreEqual(1, catelTypeInfo.GetCatelProperties().Count);
                Assert.AreEqual("defaultValue", catelTypeInfo.GetPropertyData("stringProperty").GetDefaultValue());
            }

            [TestCase]
            public void ThrowsPropertyAlreadyRegisteredExceptionForDoubleRegistration()
            {
                var propertyDataManager = new PropertyDataManager();

                RegisterProperty(propertyDataManager, "stringProperty", "defaultValue");
                ExceptionTester.CallMethodAndExpectException<PropertyAlreadyRegisteredException>(() => RegisterProperty(propertyDataManager, "stringProperty", "defaultValue"));
            }
            #endregion
        }
        #endregion
    }
}