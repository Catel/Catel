// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDictionaryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Caching
{
    using System;
    using System.Linq;
    using Catel.Collections;
    using NUnit.Framework;

    public class ListDictionaryFacts
    {
        [TestFixture]
        public class CustomComparer
        {
            [TestCase("key", "key", true)]
            [TestCase("key", "KEY", true)]
            [TestCase("key", "key ", false)]
            [TestCase("key", "KEY ", false)]
            public void CheckComparisonsOrdinalIgnoreCase(string key, string retrievalKey, bool expected)
            {
                var list = new ListDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                list.Add(key, "value");

                Assert.AreEqual(expected, list.ContainsKey(retrievalKey));
            }

            [TestCase("key", "key", true)]
            [TestCase("key", "KEY", false)]
            [TestCase("key", "key ", false)]
            [TestCase("key", "KEY ", false)]
            public void CheckComparisonsCurrentCulture(string key, string retrievalKey, bool expected)
            {
                var list = new ListDictionary<string, string>(StringComparer.CurrentCulture);

                list.Add(key, "value");

                Assert.AreEqual(expected, list.ContainsKey(retrievalKey));
            }
        }

        [TestFixture]
        public class KeepsOrder
        {
            #region Methods
            [TestCase(4)]
            public void AddingItemsDoesntChangeItemsOrder(int itemsCount)
            {
                var listDictionary = new ListDictionary<string, int>();

                for (var i = 0; i < itemsCount; i++)
                {
                    listDictionary[i.ToString()] = i;
                }

                var keyValuePairs = listDictionary.ToList();
                for (var i = 0; i < itemsCount; i++)
                {
                    Assert.AreEqual(i.ToString(), keyValuePairs[i].Key);
                }
            }

            [TestCase(4, 3)]
            [TestCase(9, 5)]
            public void AddRemoveAddItemsDoesntChangeItemsOrder(int itemsToAddCount, int itemsToRemoveCount)
            {
                var dict = new ListDictionary<string, int>();

                for (var i = 0; i < itemsToAddCount; i++)
                {
                    var key = i.ToString();
                    dict[key] = 0;
                }

                for (var i = 0; i < itemsToRemoveCount; i++)
                {
                    var key = (itemsToAddCount - itemsToRemoveCount + i).ToString();
                    dict.Remove(key);
                }

                for (var i = itemsToAddCount - itemsToRemoveCount; i < itemsToAddCount; i++)
                {
                    var key = i.ToString();
                    dict[key] = 0;
                }

                var keyValuePairs = dict.ToList();
                for (var i = 0; i < itemsToAddCount; i++)
                {
                    Assert.AreEqual(i.ToString(), keyValuePairs[i].Key);
                }
            }

           #endregion
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [TestCase(4)]
            [TestCase(6)]
            [TestCase(2)]
            public void RemovedAllItems(int itemsCount)
            {
                var listDictionary = new ListDictionary<string, int>();

                for (int i = 0; i < itemsCount; i++)
                {
                    listDictionary[i.ToString()] = i;
                }

                for (int i = 0; i < itemsCount; i++)
                {
                    listDictionary.Remove(i.ToString());
                }

                Assert.AreEqual(0, listDictionary.Count);
            }
        }
    }
}