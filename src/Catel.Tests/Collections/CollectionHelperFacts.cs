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
                var collectionA = new Collection<int> { 1 };
                var collectionB = new Collection<int> { 1, 2 };

                Assert.That(CollectionHelper.IsEqualTo(collectionA, collectionB), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForDifferentCollectionsWithEqualCount()
            {
                var collectionA = new Collection<int> { 1, 2, 4 };
                var collectionB = new Collection<int> { 1, 2, 3 };

                Assert.That(CollectionHelper.IsEqualTo(collectionA, collectionB), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForEqualCollections()
            {
                var collectionA = new Collection<int> { 1 };
                var collectionB = new Collection<int> { 1 };

                Assert.That(CollectionHelper.IsEqualTo(collectionA, collectionB), Is.True);
            }

            [TestCase]
            public void ReturnsTrueForEqualCollectionsWithSameObjectReferences()
            {
                var obj = new { Name = "test" };

                var collection1 = new Collection<object>(new object[] { obj });
                var collection2 = new Collection<object>(new object[] { obj });

                Assert.That(CollectionHelper.IsEqualTo(collection1, collection2), Is.True);
            }

            [TestCase]
            public void ReturnsTrueForEqualObjects()
            {
                var collection = new Collection<int> { 1 };

                Assert.That(CollectionHelper.IsEqualTo(collection, collection), Is.True);
            }

            [TestCase]
            public void ReturnsFalseForNullFirstCollection()
            {
                Collection<int> collectionA = null;
                var collectionB = new Collection<int> { 1 };

                Assert.That(CollectionHelper.IsEqualTo(collectionA, collectionB), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForNullSecondCollection()
            {
                var collectionA = new Collection<int> { 1 };
                Collection<int> collectionB = null;

                Assert.That(CollectionHelper.IsEqualTo(collectionA, collectionB), Is.False);
            }
        }
    }
}