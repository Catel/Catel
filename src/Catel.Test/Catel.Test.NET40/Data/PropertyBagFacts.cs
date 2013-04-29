// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
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

    public class PropertyBagFacts
    {
        [TestClass]
        public class TheIsPropertyAvailableMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                var propertyBag = new PropertyBag();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.IsPropertyAvailable(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.IsPropertyAvailable(string.Empty));
            }

            [TestMethod]
            public void ReturnsFalseForNonRegisteredPropertyName()
            {
                var propertyBag = new PropertyBag();

                Assert.IsFalse(propertyBag.IsPropertyAvailable("MyProperty"));
            }

            [TestMethod]
            public void ReturnsTrueForRegisteredPropertyName()
            {
                var propertyBag = new PropertyBag();
                propertyBag.SetPropertyValue("MyProperty", 1);

                Assert.IsTrue(propertyBag.IsPropertyAvailable("MyProperty"));
            }
        }

        [TestClass]
        public class TheGetAllPropertiesMethod
        {
            [TestMethod]
            public void ReturnsAllRegisteredPropertiesWithCorrectValues()
            {
                var propertyBag = new PropertyBag();
                propertyBag.SetPropertyValue("FirstProperty", 1);
                propertyBag.SetPropertyValue("SecondProperty", "test");

                var allProperties = propertyBag.GetAllProperties().ToList();

                Assert.AreEqual(2, allProperties.Count);

                Assert.AreEqual("FirstProperty", allProperties[0].Key);
                Assert.AreEqual(1, allProperties[0].Value);

                Assert.AreEqual("SecondProperty", allProperties[1].Key);
                Assert.AreEqual("test", allProperties[1].Value);
            }
        }

        [TestClass]
        public class TheGetPropertyValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                var propertyBag = new PropertyBag();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.GetPropertyValue<object>(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.GetPropertyValue<object>(string.Empty));
            }

            [TestMethod]
            public void ReturnsDefaultValueForNonRegisteredProperty()
            {
                var propertyBag = new PropertyBag();

                Assert.AreEqual(null, propertyBag.GetPropertyValue<string>("StringProperty"));
                Assert.AreEqual(0, propertyBag.GetPropertyValue<int>("IntProperty"));
            }

            [TestMethod]
            public void ReturnsRightPropertyValue()
            {
                var propertyBag = new PropertyBag();

                propertyBag.SetPropertyValue("StringProperty", "test");
                propertyBag.SetPropertyValue("IntProperty", 1);
                
                Assert.AreEqual("test", propertyBag.GetPropertyValue<string>("StringProperty"));
                Assert.AreEqual(1, propertyBag.GetPropertyValue<int>("IntProperty"));
            }
        }

        [TestClass]
        public class TheSetPropertyValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                var propertyBag = new PropertyBag();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.SetPropertyValue(null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => propertyBag.SetPropertyValue(string.Empty, null));
            }

            [TestMethod]
            public void SetsPropertyCorrectly()
            {
                var propertyBag = new PropertyBag();

                propertyBag.SetPropertyValue("StringProperty", "A");
                Assert.AreEqual("A", propertyBag.GetPropertyValue<string>("StringProperty"));

                propertyBag.SetPropertyValue("StringProperty", "B");
                Assert.AreEqual("B", propertyBag.GetPropertyValue<string>("StringProperty"));
            }
        }
    }
}