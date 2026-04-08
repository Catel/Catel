namespace Catel.Tests.Collections;

using System;
using System.Collections.Generic;
using Catel.Collections;
using Catel.Services;
using Moq;
using NUnit.Framework;

public class EnumerableExtensionsFacts
{
    [TestFixture]
    public class TheToObservableDictionaryMethod
    {
        [Test]
        public void ReturnsDefault()
        {
            var items = new List<KeyValuePair<int, int>>()
            {
                {
                    new KeyValuePair<int, int>(1, 1)
                }
            };

            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var observableDictionary = items.ToObservableDictionary(d => d.Key, d => d.Value, dispatcherServiceMock.Object);

            var success = observableDictionary.Contains(items[0]);

            Assert.That(success, Is.True);
        }

        [Test]
        public void ReturnsDefaultWithOnlyKeySelector()
        {
            var items = new List<KeyValuePair<int, int>>()
            {
                {
                    new KeyValuePair<int, int>(1, 1)
                }
            };

            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var observableDictionary = items.ToObservableDictionary(d => d.Key, dispatcherServiceMock.Object);

            var success = observableDictionary.Contains(new KeyValuePair<int, KeyValuePair<int, int>>(1, items[0]));

            Assert.That(success, Is.True);
        }

        [Test]
        public void ReturnsDefaultWithComparer()
        {
            var items = new List<KeyValuePair<string, int>>()
            {
                {
                    new KeyValuePair<string, int>("Foo", 1)
                }
            };

            var dispatcherServiceMock = new Mock<IDispatcherService>();

            var observableDictionary = items.ToObservableDictionary(d => d.Key, d => d.Value, 
                dispatcherServiceMock.Object, StringComparer.OrdinalIgnoreCase);

            var hasItem = observableDictionary.Contains(items[0]);
            var comparerWorks = observableDictionary.ContainsKey("foo");

            Assert.That(hasItem && comparerWorks, Is.True);
        }
    }
}
