// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tests.Data
{
    using System;
    using System.Linq;

    using Catel.Data;

    using NUnit.Framework;

    public class PropertyBagFacts
    {
        [TestFixture(typeof(PropertyBag))]
        [TestFixture(typeof(TypedPropertyBag))]
        public class TheIsAvailableMethod<TPropertyBag>
            where TPropertyBag : IPropertyBag, new()
        {
            private IPropertyBag _propertyBag;

            [SetUp]
            public void CreatePropertyBag()
            {
                _propertyBag = new TPropertyBag();
            }

            [TestCase]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.IsAvailable(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.IsAvailable(string.Empty));
            }

            [TestCase]
            public void ReturnsFalseForNonRegisteredPropertyName()
            {
                Assert.IsFalse(_propertyBag.IsAvailable("MyProperty"));
            }

            [TestCase]
            public void ReturnsTrueForRegisteredPropertyName()
            {
                _propertyBag.SetValue("MyProperty", 1);

                Assert.IsTrue(_propertyBag.IsAvailable("MyProperty"));
            }
        }

        [TestFixture(typeof(PropertyBag))]
        [TestFixture(typeof(TypedPropertyBag))]
        public class TheGetAllNamesMethod<TPropertyBag>
            where TPropertyBag : IPropertyBag, new()
        {
            private IPropertyBag _propertyBag;

            [SetUp]
            public void CreatePropertyBag()
            {
                _propertyBag = new TPropertyBag();
            }

            [TestCase]
            public void ReturnsAllRegisteredProperties()
            {
                _propertyBag.SetValue("FirstProperty", 1);
                _propertyBag.SetValue("SecondProperty", "test");

                var allProperties = _propertyBag.GetAllNames().ToList();

                Assert.AreEqual(2, allProperties.Count);

                Assert.AreEqual("FirstProperty", allProperties[0]);
                //Assert.AreEqual(1, allProperties[0].Value);

                Assert.AreEqual("SecondProperty", allProperties[1]);
                //Assert.AreEqual("test", allProperties[1].Value);
            }
        }

        [TestFixture(typeof(PropertyBag))]
        [TestFixture(typeof(TypedPropertyBag))]
        public class TheGetValueMethod<TPropertyBag>
            where TPropertyBag : IPropertyBag, new()
        {
            private IPropertyBag _propertyBag;

            [SetUp]
            public void CreatePropertyBag()
            {
                _propertyBag = new TPropertyBag();
            }

            [TestCase]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.GetValue<object>(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.GetValue<object>(string.Empty));
            }

            [TestCase]
            public void ReturnsDefaultValueForNonRegisteredProperty()
            {
                Assert.AreEqual(null, _propertyBag.GetValue<string>("StringProperty"));
                Assert.AreEqual(0, _propertyBag.GetValue<int>("IntProperty"));
            }

            [TestCase]
            public void ReturnsRightPropertyValue()
            {
                _propertyBag.SetValue("StringProperty", "test");
                _propertyBag.SetValue("IntProperty", 1);
                
                Assert.AreEqual("test", _propertyBag.GetValue<string>("StringProperty"));
                Assert.AreEqual(1, _propertyBag.GetValue<int>("IntProperty"));
            }
        }

        [TestFixture(typeof(PropertyBag))]
        [TestFixture(typeof(TypedPropertyBag))]
        public class TheSetValueMethod<TPropertyBag>
            where TPropertyBag : IPropertyBag, new()
        {
            private IPropertyBag _propertyBag;

            [SetUp]
            public void CreatePropertyBag()
            {
                _propertyBag = new TPropertyBag();
            }

            [TestCase]
            public void ThrowsArgumentExceptionForInvalidPropertyName()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.SetValue<object>(null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => _propertyBag.SetValue<object>(string.Empty, null));
            }

            [TestCase]
            public void SetsPropertyCorrectly()
            {
                _propertyBag.SetValue("StringProperty", "A");
                Assert.AreEqual("A", _propertyBag.GetValue<string>("StringProperty"));

                _propertyBag.SetValue("StringProperty", "B");
                Assert.AreEqual("B", _propertyBag.GetValue<string>("StringProperty"));
            }

            [TestCase]
            public void RaisesChangeNotificationForDifferentPropertyValues()
            {
                var eventCount = 0;

                _propertyBag.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "ChangeNotificationTest")
                    {
                        eventCount++;
                    }
                };

                _propertyBag.SetValue("ChangeNotificationTest", "ABC");

                Assert.AreEqual(1, eventCount);

                _propertyBag.SetValue("ChangeNotificationTest", "DEF");

                Assert.AreEqual(2, eventCount);
            }

            [TestCase]
            public void DoesNotRaiseChangeNotificationForSamePropertyValues()
            {
                var eventCount = 0;

                _propertyBag.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "ChangeNotificationTest2")
                    {
                        eventCount++;
                    }
                };

                _propertyBag.SetValue("ChangeNotificationTest2", "ABC");

                Assert.AreEqual(1, eventCount);

                _propertyBag.SetValue("ChangeNotificationTest2", "ABC");

                Assert.AreEqual(1, eventCount);
            }
        }
    }
}
