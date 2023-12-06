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
                Assert.Throws<ArgumentException>(() => _propertyBag.IsAvailable(null));
                Assert.Throws<ArgumentException>(() => _propertyBag.IsAvailable(string.Empty));
            }

            [TestCase]
            public void ReturnsFalseForNonRegisteredPropertyName()
            {
                Assert.That(_propertyBag.IsAvailable("MyProperty"), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForRegisteredPropertyName()
            {
                _propertyBag.SetValue("MyProperty", 1);

                Assert.That(_propertyBag.IsAvailable("MyProperty"), Is.True);
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

                Assert.That(allProperties.Count, Is.EqualTo(2));

                Assert.That(allProperties[0], Is.EqualTo("FirstProperty"));
                //Assert.AreEqual(1, allProperties[0].Value);

                Assert.That(allProperties[1], Is.EqualTo("SecondProperty"));
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
                Assert.Throws<ArgumentException>(() => _propertyBag.GetValue<object>(null));
                Assert.Throws<ArgumentException>(() => _propertyBag.GetValue<object>(string.Empty));
            }

            [TestCase]
            public void ReturnsDefaultValueForNonRegisteredProperty()
            {
                Assert.That(_propertyBag.GetValue<string>("StringProperty"), Is.EqualTo(null));
                Assert.That(_propertyBag.GetValue<int>("IntProperty"), Is.EqualTo(0));
            }

            [TestCase]
            public void ReturnsRightPropertyValue()
            {
                _propertyBag.SetValue("StringProperty", "test");
                _propertyBag.SetValue("IntProperty", 1);

                Assert.That(_propertyBag.GetValue<string>("StringProperty"), Is.EqualTo("test"));
                Assert.That(_propertyBag.GetValue<int>("IntProperty"), Is.EqualTo(1));
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
                Assert.Throws<ArgumentException>(() => _propertyBag.SetValue<object>(null, null));
                Assert.Throws<ArgumentException>(() => _propertyBag.SetValue<object>(string.Empty, null));
            }

            [TestCase]
            public void SetsPropertyCorrectly()
            {
                _propertyBag.SetValue("StringProperty", "A");
                Assert.That(_propertyBag.GetValue<string>("StringProperty"), Is.EqualTo("A"));

                _propertyBag.SetValue("StringProperty", "B");
                Assert.That(_propertyBag.GetValue<string>("StringProperty"), Is.EqualTo("B"));
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

                Assert.That(eventCount, Is.EqualTo(1));

                _propertyBag.SetValue("ChangeNotificationTest", "DEF");

                Assert.That(eventCount, Is.EqualTo(2));
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

                Assert.That(eventCount, Is.EqualTo(1));

                _propertyBag.SetValue("ChangeNotificationTest2", "ABC");

                Assert.That(eventCount, Is.EqualTo(1));
            }
        }
    }
}
