namespace Catel.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Collections;

    using NUnit.Framework;

    public class CollectionExtensionsFacts
    {
        [TestFixture]
        public class TheCanMoveItemUpMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.CanMoveItemUp(null, item));
            }

            [TestCase]
            public void ReturnsFalseForNullItem()
            {
                var list = new List<int> { };

                Assert.That(list.CanMoveItemUp(null), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForListWithSingleItem()
            {
                var list = new List<int> { 1 };

                Assert.That(list.CanMoveItemUp(1), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForNonContainedItem()
            {
                var list = new List<int> { 1, 2 };

                Assert.That(list.CanMoveItemUp(3), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.CanMoveItemUp(1), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForItemNoAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.CanMoveItemUp(2), Is.True);
            }
        }

        [TestFixture]
        public class TheMoveItemUpMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemUp(null, item));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullItem()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentNullException>(() => list.MoveItemUp(null));
            }

            [TestCase]
            public void ReturnsFalseForNotContainedItem()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemUp(4), Is.False);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueForContainedItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemUp(1), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueAndMovesItemUpForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemUp(2), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(2));
                Assert.That(list[1], Is.EqualTo(1));
                Assert.That(list[2], Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class TheCanMoveItemDownMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.CanMoveItemDown(null, item));
            }

            [TestCase]
            public void ReturnsFalseForNullItem()
            {
                var list = new List<int> { };

                Assert.That(list.CanMoveItemDown(null), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForListWithSingleItem()
            {
                var list = new List<int> { 1 };

                Assert.That(list.CanMoveItemDown(1), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForNonContainedItem()
            {
                var list = new List<int> { 1, 2 };

                Assert.That(list.CanMoveItemDown(3), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.CanMoveItemDown(3), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForItemNoAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.CanMoveItemDown(2), Is.True);
            }
        }

        [TestFixture]
        public class TheMoveItemUpByIndexMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemUpByIndex(null, 2));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIndexSmallerThanZero()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemUpByIndex(null, -1));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIndexLargerThanListCount()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemUpByIndex(null, 3));
            }

            [TestCase]
            public void ReturnsTrueForContainedItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemUpByIndex(0), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueAndMovesItemUpForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemUpByIndex(1), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(2));
                Assert.That(list[1], Is.EqualTo(1));
                Assert.That(list[2], Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class TheMoveItemDownMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemDown(null, item));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullItem()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentNullException>(() => list.MoveItemDown(null));
            }

            [TestCase]
            public void ReturnsFalseForNotContainedItem()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemDown(4), Is.False);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueForContainedItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemDown(3), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueAndMovesItemDownForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemDown(2), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(3));
                Assert.That(list[2], Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheMoveItemDownByIndexMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.MoveItemDownByIndex(null, 2));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIndexSmallerThanZero()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentOutOfRangeException>(() => list.MoveItemDownByIndex(-1));
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeExceptionForIndexLargerThanListCount()
            {
                var list = new List<int> { 1, 2, 3 };
                Assert.Throws<ArgumentOutOfRangeException>(() => list.MoveItemDownByIndex(3));
            }

            [TestCase]
            public void ReturnsTrueForContainedItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemDownByIndex(2), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
                Assert.That(list[2], Is.EqualTo(3));
            }

            [TestCase]
            public void ReturnsTrueAndMovesItemDownForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.That(list.MoveItemDownByIndex(1), Is.True);
                Assert.That(list.Count, Is.EqualTo(3));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(3));
                Assert.That(list[2], Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheRemoveFirstMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.RemoveFirst(null));
            }

            [TestCase]
            public void ExitsSilentlyForEmptyList()
            {
                var list = new List<int>();

                list.RemoveFirst();
            }

            [TestCase]
            public void RemovesFirstItemFromList()
            {
                var list = new List<int>(new[] { 1, 2, 3 });

                list.RemoveFirst();

                Assert.That(list.Count, Is.EqualTo(2));
                Assert.That(list[0], Is.EqualTo(2));
                Assert.That(list[1], Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class TheRemoveLastMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.RemoveLast(null));
            }

            [TestCase]
            public void ExitsSilentlyForEmptyList()
            {
                var list = new List<int>();

                list.RemoveLast();
            }

            [TestCase]
            public void RemovesLastItemFromList()
            {
                var list = new List<int>(new[] { 1, 2, 3 });

                list.RemoveLast();

                Assert.That(list.Count, Is.EqualTo(2));
                Assert.That(list[0], Is.EqualTo(1));
                Assert.That(list[1], Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheAddRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var newList = new List<int> { 4, 5, 6 };

                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.AddRange((ICollection<int>)null, newList));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullRange()
            {
                var collection = new Collection<int> { 1, 2, 3 };

                Assert.Throws<ArgumentNullException>(() => collection.AddRange(null));
            }

            [TestCase]
            public void AddsItemsToEmptyCollection()
            {
                var collection = new Collection<int>();
                var newList = new List<int> { 4, 5, 6 };

                collection.AddRange(newList);

                Assert.That(collection.Count, Is.EqualTo(3));
                Assert.That(collection[0], Is.EqualTo(4));
                Assert.That(collection[1], Is.EqualTo(5));
                Assert.That(collection[2], Is.EqualTo(6));
            }

            [TestCase]
            public void AddsItemsToFilledCollection()
            {
                var collection = new Collection<int> { 1, 2, 3 };
                var newList = new List<int> { 4, 5, 6 };

                collection.AddRange(newList);

                Assert.That(collection.Count, Is.EqualTo(6));
                Assert.That(collection[0], Is.EqualTo(1));
                Assert.That(collection[1], Is.EqualTo(2));
                Assert.That(collection[2], Is.EqualTo(3));
                Assert.That(collection[3], Is.EqualTo(4));
                Assert.That(collection[4], Is.EqualTo(5));
                Assert.That(collection[5], Is.EqualTo(6));
            }

            [TestCase]
            public void AddsNoItemsToFilledCollectionWhenItemsToAddIsEmpty()
            {
                var collection = new Collection<int> { 1, 2, 3 };
                var newList = new List<int>();

                collection.AddRange(newList);

                Assert.That(collection.Count, Is.EqualTo(3));
                Assert.That(collection[0], Is.EqualTo(1));
                Assert.That(collection[1], Is.EqualTo(2));
                Assert.That(collection[2], Is.EqualTo(3));
            }
        }

        [TestFixture]
        public class TheReplaceRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var newList = new List<int> { 4, 5, 6 };

                Assert.Throws<ArgumentNullException>(() => Catel.Collections.CollectionExtensions.ReplaceRange((ICollection<int>)null, newList));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullRange()
            {
                var collection = new Collection<int> { 1, 2, 3 };

                Assert.Throws<ArgumentNullException>(() => collection.ReplaceRange(null));
            }

            [TestCase]
            public void ReplacesFilledCollectionByEmptyCollection()
            {
                var collection = new Collection<int> { 1, 2, 3 };
                var newList = new List<int>();

                collection.ReplaceRange(newList);

                Assert.That(collection.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ReplacesEmptyCollectionByFilledCollection()
            {
                var collection = new Collection<int>();
                var newList = new List<int> { 4, 5, 6 };

                collection.ReplaceRange(newList);

                Assert.That(collection.Count, Is.EqualTo(3));
                Assert.That(collection[0], Is.EqualTo(4));
                Assert.That(collection[1], Is.EqualTo(5));
                Assert.That(collection[2], Is.EqualTo(6));
            }

            [TestCase]
            public void ReplacedFilledCollectionByFilledCollection()
            {
                var collection = new Collection<int> { 1, 2, 3 };
                var newList = new List<int> { 4, 5, 6 };

                collection.ReplaceRange(newList);

                Assert.That(collection.Count, Is.EqualTo(3));
                Assert.That(collection[0], Is.EqualTo(4));
                Assert.That(collection[1], Is.EqualTo(5));
                Assert.That(collection[2], Is.EqualTo(6));
            }
        }
    }
}
