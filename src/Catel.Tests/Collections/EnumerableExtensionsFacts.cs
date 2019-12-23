// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using Catel.Collections;
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

                var observableDictionary = items.ToObservableDictionary(d => d.Key, d => d.Value);

                var success = observableDictionary.Contains(items[0]);

                Assert.IsTrue(success);
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

                var observableDictionary = items.ToObservableDictionary(d => d.Key);

                var success = observableDictionary.Contains(new KeyValuePair<int, KeyValuePair<int, int>>(1, items[0]));

                Assert.IsTrue(success);
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

                var observableDictionary = items.ToObservableDictionary(d => d.Key, d => d.Value, StringComparer.OrdinalIgnoreCase);

                var hasItem = observableDictionary.Contains(items[0]);
                var comparerWorks = observableDictionary.ContainsKey("foo");

                Assert.IsTrue(hasItem && comparerWorks);
            }
        }
    }
}
