// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListDictionaryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Caching
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Caching;
    using NUnit.Framework;

    public class ListDictionaryFacts
    {
        [TestFixture]
        public class KeepsOrder
        {
            #region Methods
            [TestCase(4)]
            public void AddingItemsDoesntChangeItemsOrder(int itemsCount)
            {
                var listDictionary = new ListDictionary<string, int>();

                for (int i = 0; i < itemsCount; i++)
                {
                    listDictionary[i.ToString()] = i;
                }

                var keyValuePairs = listDictionary.ToList();
                for (int i = 0; i < itemsCount; i++)
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
    }
}