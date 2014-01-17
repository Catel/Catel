// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Collections;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CollectionExtensionsFacts
    {
        [TestClass]
        public class TheCanMoveItemUpMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.CanMoveItemUp(null, item));
            }

            [TestMethod]
            public void ReturnsFalseForNullItem()
            {
                var list = new List<int> {};

                Assert.IsFalse(list.CanMoveItemUp(null));
            }

            [TestMethod]
            public void ReturnsFalseForListWithSingleItem()
            {
                var list = new List<int> { 1 };

                Assert.IsFalse(list.CanMoveItemUp(1));
            }

            [TestMethod]
            public void ReturnsFalseForNonContainedItem()
            {
                var list = new List<int> { 1, 2 };

                Assert.IsFalse(list.CanMoveItemUp(3));                
            }

            [TestMethod]
            public void ReturnsFalseForItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsFalse(list.CanMoveItemUp(1));
            }

            [TestMethod]
            public void ReturnsTrueForItemNoAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.CanMoveItemUp(2));
            }
        }

        [TestClass]
        public class TheMoveItemUpMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemUp(null, item));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullItem()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => list.MoveItemUp(null));
            }

            [TestMethod]
            public void ReturnsFalseForNotContainedItem()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsFalse(list.MoveItemUp(4));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueForContainedItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemUp(1));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueAndMovesItemUpForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemUp(2));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(2, list[0]);
                Assert.AreEqual(1, list[1]);
                Assert.AreEqual(3, list[2]);
            }
        }

        [TestClass]
        public class TheCanMoveItemDownMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.CanMoveItemDown(null, item));
            }

            [TestMethod]
            public void ReturnsFalseForNullItem()
            {
                var list = new List<int> { };

                Assert.IsFalse(list.CanMoveItemDown(null));
            }

            [TestMethod]
            public void ReturnsFalseForListWithSingleItem()
            {
                var list = new List<int> { 1 };

                Assert.IsFalse(list.CanMoveItemDown(1));
            }

            [TestMethod]
            public void ReturnsFalseForNonContainedItem()
            {
                var list = new List<int> { 1, 2 };

                Assert.IsFalse(list.CanMoveItemDown(3));
            }

            [TestMethod]
            public void ReturnsFalseForItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsFalse(list.CanMoveItemDown(3));
            }

            [TestMethod]
            public void ReturnsTrueForItemNoAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.CanMoveItemDown(2));
            }
        }

        [TestClass]
        public class TheMoveItemUpByIndexMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemUpByIndex(null, 2));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForIndexSmallerThanZero()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemUpByIndex(null, -1));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForIndexLargerThanListCount()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemUpByIndex(null, 3));
            }

            [TestMethod]
            public void ReturnsTrueForContainedItemAtFirstPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemUpByIndex(0));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueAndMovesItemUpForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemUpByIndex(1));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(2, list[0]);
                Assert.AreEqual(1, list[1]);
                Assert.AreEqual(3, list[2]);
            }
        }

        [TestClass]
        public class TheMoveItemDownMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                int item = 2;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemDown(null, item));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullItem()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => list.MoveItemDown(null));
            }

            [TestMethod]
            public void ReturnsFalseForNotContainedItem()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsFalse(list.MoveItemDown(4));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueForContainedItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemDown(3));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueAndMovesItemDownForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemDown(2));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(3, list[1]);
                Assert.AreEqual(2, list[2]);
            }            
        }

        [TestClass]
        public class TheMoveItemDownByIndexMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.MoveItemDownByIndex(null, 2));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForIndexSmallerThanZero()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => list.MoveItemDownByIndex(-1));
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForIndexLargerThanListCount()
            {
                var list = new List<int> { 1, 2, 3 };
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => list.MoveItemDownByIndex(3));
            }

            [TestMethod]
            public void ReturnsTrueForContainedItemAtLastPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemDownByIndex(2));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
                Assert.AreEqual(3, list[2]);
            }

            [TestMethod]
            public void ReturnsTrueAndMovesItemDownForContainedItemAtSecondPosition()
            {
                var list = new List<int> { 1, 2, 3 };

                Assert.IsTrue(list.MoveItemDownByIndex(1));
                Assert.AreEqual(3, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(3, list[1]);
                Assert.AreEqual(2, list[2]);
            }            
        }

        [TestClass]
        public class TheRemoveFirstMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.RemoveFirst(null));
            }

            [TestMethod]
            public void ExitsSilentlyForEmptyList()
            {
                var list = new List<int>();

                list.RemoveFirst();
            }

            [TestMethod]
            public void RemovesFirstItemFromList()
            {
                var list = new List<int>(new [] { 1, 2, 3 });

                list.RemoveFirst();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(2, list[0]);
                Assert.AreEqual(3, list[1]);
            }
        }

        [TestClass]
        public class TheRemoveLastMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullList()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.RemoveLast(null));
            }

            [TestMethod]
            public void ExitsSilentlyForEmptyList()
            {
                var list = new List<int>();

                list.RemoveLast();
            }

            [TestMethod]
            public void RemovesLastItemFromList()
            {
                var list = new List<int>(new[] { 1, 2, 3 });

                list.RemoveLast();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(1, list[0]);
                Assert.AreEqual(2, list[1]);
            }
        }

        [TestClass]
        public class TheAddRangeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var newList = new List<int> { 4, 5, 6 };

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.AddRange(null, newList));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullRange()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => collection.AddRange(null));
            }

            [TestMethod]
            public void AddsItemsToEmptyCollection()
            {
                var collection = new ObservableCollection<int>();
                var newList = new List<int> { 4, 5, 6 };

                collection.AddRange(newList);

                Assert.AreEqual(3, collection.Count);
                Assert.AreEqual(4, collection[0]);
                Assert.AreEqual(5, collection[1]);
                Assert.AreEqual(6, collection[2]);
            }

            [TestMethod]
            public void AddsItemsToFilledCollection()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };
                var newList = new List<int> { 4, 5, 6 };

                collection.AddRange(newList);

                Assert.AreEqual(6, collection.Count);
                Assert.AreEqual(1, collection[0]);
                Assert.AreEqual(2, collection[1]);
                Assert.AreEqual(3, collection[2]);
                Assert.AreEqual(4, collection[3]);
                Assert.AreEqual(5, collection[4]);
                Assert.AreEqual(6, collection[5]);
            }

            [TestMethod]
            public void AddsNoItemsToFilledCollectionWhenItemsToAddIsEmpty()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };
                var newList = new List<int>();

                collection.AddRange(newList);

                Assert.AreEqual(3, collection.Count);
                Assert.AreEqual(1, collection[0]);
                Assert.AreEqual(2, collection[1]);
                Assert.AreEqual(3, collection[2]);
            }    
        }

        [TestClass]
        public class TheReplaceRangeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var newList = new List<int> { 4, 5, 6 };

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => CollectionExtensions.ReplaceRange(null, newList));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullRange()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => collection.ReplaceRange(null));
            }

            [TestMethod]
            public void ReplacesFilledCollectionByEmptyCollection()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };
                var newList = new List<int>();

                collection.ReplaceRange(newList);

                Assert.AreEqual(0, collection.Count);
            }

            [TestMethod]
            public void ReplacesEmptyCollectionByFilledCollection()
            {
                var collection = new ObservableCollection<int>();
                var newList = new List<int> { 4, 5, 6 };

                collection.ReplaceRange(newList);

                Assert.AreEqual(3, collection.Count);
                Assert.AreEqual(4, collection[0]);
                Assert.AreEqual(5, collection[1]);
                Assert.AreEqual(6, collection[2]);
            }

            [TestMethod]
            public void ReplacedFilledCollectionByFilledCollection()
            {
                var collection = new ObservableCollection<int> { 1, 2, 3 };
                var newList = new List<int> { 4, 5, 6 };

                collection.ReplaceRange(newList);

                Assert.AreEqual(3, collection.Count);
                Assert.AreEqual(4, collection[0]);
                Assert.AreEqual(5, collection[1]);
                Assert.AreEqual(6, collection[2]);
            }
        }
    }
}