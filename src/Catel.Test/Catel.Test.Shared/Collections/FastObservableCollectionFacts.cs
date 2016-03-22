// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollectionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Collections
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using Catel.Collections;

    using NUnit.Framework;

    public class FastObservableCollectionFacts
    {
        [TestFixture]
        public class TheIsDirtyProperty
        {
            [TestCase]
            public void ReturnsFalseWhenNoPendingNotificationsAreListed()
            {
                var fastCollection = new FastObservableCollection<int>();

                fastCollection.Add(1);

                Assert.IsFalse(fastCollection.IsDirty);
            }

            [TestCase]
            public void ReturnsTrueWhenPendingNotificationsAreListed()
            {
                var fastCollection = new FastObservableCollection<int>();

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);

                    Assert.IsTrue(fastCollection.IsDirty);
                }

                Assert.IsFalse(fastCollection.IsDirty);
            }
        }

        [TestFixture]
        public class TheAddRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null));
            }

            [TestCase]
            public void RaisesSingleEventWhileAddingRange()
            {
                int counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, counter);
            }
        }

        [TestFixture]
        public class TheRemoveRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null));
            }

            [TestCase]
            public void RaisesSingleEventWhileAddingRange()
            {
                int counter = 0;

                var fastCollection = new FastObservableCollection<int>(new [] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, counter);
            }
        }

        [TestFixture]
        public class TheSuspendNotificationsMethod
        {
            [TestCase]
            public void SuspendsValidationWhileAddingAndRemovingItems()
            {
                int counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);
                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);

                    fastCollection.Remove(5);
                    fastCollection.Remove(4);
                    fastCollection.Remove(3);
                    fastCollection.Remove(2);
                    fastCollection.Remove(1);
                }

                Assert.AreEqual(1, counter);
            }
        }

        [TestFixture]
        public class SupportsLinq
        {
            [TestCase]
            public void ReturnsSingleElementUsingLinq()
            {
                var fastCollection = new FastObservableCollection<int>();

                for (int i = 0; i < 43; i++)
                {
                    fastCollection.Add(i);
                }

                var allInts = (from x in fastCollection
                               where x == 42
                               select x).FirstOrDefault();

                Assert.AreEqual(42, allInts);
            }
        }

        [TestFixture]
        public class TheSuspensionModeMethod
        {
            [TestCase]
            public void AddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                    {
                        counter++;
                        eventArgs = e;
                    };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);
                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.AreEqual(1, counter);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                CollectionAssert.AreEqual(eventArgs.NewItems, new[] { 1, 2, 3, 4, 5 });
            }

            [TestCase]
            public void MultipleActionsWithoutSuspendingNotifications()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.Add(0);
                fastCollection.Add(1);

                fastCollection.Remove(0);
                fastCollection.Remove(1);

                CollectionExtensions.AddRange(fastCollection, new[] { 1, 2 });

                fastCollection[0] = 5;

                fastCollection.Move(0, 1);

                fastCollection.Clear();

                Assert.AreEqual(9, counter);
            }

            [TestCase]
            public void RemovingItemsInRemovingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                    {
                        counter++;
                        eventArgs = e;
                    };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                {
                    fastCollection.Remove(1);
                    fastCollection.Remove(2);
                    fastCollection.Remove(3);
                    fastCollection.Remove(4);
                    fastCollection.Remove(5);
                }

                Assert.AreEqual(1, counter);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
                CollectionAssert.AreEqual(eventArgs.OldItems, new[] { 1, 2, 3, 4, 5 });
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForAddingInRemovingMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Add(0));
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForClearingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Clear());
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForMovingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Move(0, 1));
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForRemovingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Remove(0));
                }
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForSettingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection[0] = 0);
                }
            }
        }
    }
}