// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Collections
{
    using System.Collections.ObjectModel;
    using Catel.Collections;

    using NUnit.Framework;

    public class CollectionHelperFacts
    {
        [TestFixture]
        public class TheIsEqualToMethod
        {
            [TestCase]
            public void ReturnsFalseForDifferentCollections()
            {
                var collectionA = new Collection<int> {1};
                var collectionB = new Collection<int> {1, 2};

                Assert.IsFalse(CollectionHelper.IsEqualTo(collectionA, collectionB));
            }

            [TestCase]
            public void ReturnsFalseForDifferentCollectionsWithEqualCount()
            {
                var collectionA = new Collection<int> { 1, 2, 4 };
                var collectionB = new Collection<int> { 1, 2, 3 };

                Assert.IsFalse(CollectionHelper.IsEqualTo(collectionA, collectionB));
            }

            [TestCase]
            public void ReturnsTrueForEqualCollections()
            {
                var collectionA = new Collection<int> {1};
                var collectionB = new Collection<int> {1};

                Assert.IsTrue(CollectionHelper.IsEqualTo(collectionA, collectionB));
            }

            [TestCase]
            public void ReturnsTrueForEqualCollectionsWithSameObjectReferences()
            {
                var obj = new { Name = "test" };

                var collection1 = new Collection<object>(new object[] { obj });
                var collection2 = new Collection<object>(new object[] { obj });

                Assert.IsTrue(CollectionHelper.IsEqualTo(collection1, collection2));
            }

            [TestCase]
            public void ReturnsTrueForEqualObjects()
            {
                var collection = new Collection<int> {1};

                Assert.IsTrue(CollectionHelper.IsEqualTo(collection, collection));
            }

            [TestCase]
            public void ReturnsFalseForNullFirstCollection()
            {
                Collection<int> collectionA = null;
                var collectionB = new Collection<int> {1};

                Assert.IsFalse(CollectionHelper.IsEqualTo(collectionA, collectionB));
            }

            [TestCase]
            public void ReturnsFalseForNullSecondCollection()
            {
                var collectionA = new Collection<int> {1};
                Collection<int> collectionB = null;

                Assert.IsFalse(CollectionHelper.IsEqualTo(collectionA, collectionB));
            }
        }
    }
}