// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollectionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using Catel.Collections;
    using Catel.Reflection;

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
        public class TheNotificationsSuspendedProperty
        {
            [TestCase]
            public void ReturnsFalseAfterDisposing()
            {
                var fastCollection = new FastObservableCollection<int>();

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);

                    Assert.IsTrue(fastCollection.NotificationsSuspended);
                }

                Assert.IsFalse(fastCollection.NotificationsSuspended);
            }

            [TestCase]
            public void ReturnsFalseAfterChangedDisposing()
            {
                var fastCollection = new FastObservableCollection<int>();

                var firstToken = fastCollection.SuspendChangeNotifications();
                var secondToken = fastCollection.SuspendChangeNotifications();

                firstToken.Dispose();
                secondToken.Dispose();

                Assert.IsFalse(fastCollection.NotificationsSuspended);
            }
        }

        [TestFixture]
        public class TheAddRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null, SuspensionMode.Adding));
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing));
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

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.AreEqual(2, counter);

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.AreEqual(3, counter);

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), SuspensionMode.Adding);

                Assert.AreEqual(4, counter);
            }

            [TestCase]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeAdding()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleResetEventWhileAddingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleResetEventWhileAddingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }
        }

        [TestFixture]
        public class TheInsertRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.InsertItems(null, 0));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.InsertItems(null, 0, SuspensionMode.Adding));
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Removing));
            }

            [TestCase]
            public void RaisesSingleEventWhileInsertingRange()
            {
                int counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0);

                Assert.AreEqual(1, counter);

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Adding);

                Assert.AreEqual(2, counter);

                fastCollection.InsertItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), 0);

                Assert.AreEqual(3, counter);

                fastCollection.InsertItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), 0, SuspensionMode.Adding);

                Assert.AreEqual(4, counter);
            }

            [TestCase]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeAdding()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Adding);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleResetEventWhileInsertingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleResetEventWhileInsertingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }
        }

        [TestFixture]
        public class TheRemoveRangeMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null, SuspensionMode.Removing));
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding));
            }

            [TestCase]
            public void RaisesSingleEventWhileRemovingRange()
            {
                int counter = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, counter);

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing);

                Assert.AreEqual(2, counter);

                fastCollection.RemoveItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.AreEqual(3, counter);

                fastCollection.RemoveItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), SuspensionMode.Removing);

                Assert.AreEqual(4, counter);
            }

            [TestCase]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeRemoving()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing);

                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleResetEventWhileRemovingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [TestCase]
            public void RaisesSingleRemoveEventWhileRemovingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
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
        public class TheResetMethod
        {
            [TestCase]
            public void ResetWithoutSuspendChangeNotifications()
            {
                var collectionChanged = false;
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                {
                    collectionChanged = true;
                };

                fastCollection.Reset();

                Assert.AreEqual(true, collectionChanged);
            }

            [TestCase]
            public void CallingResetWhileAddingItemsInAddingMode()
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

                var token = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding);
                fastCollection.Add(1);
                fastCollection.Add(2);

                fastCollection.Reset();
                Assert.AreEqual(0, counter);

                fastCollection.Add(3);
                fastCollection.Add(4);
                fastCollection.Add(5);
                token.Dispose();

                Assert.AreEqual(1, counter);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                CollectionAssert.AreEqual(eventArgs.NewItems, new[] { 1, 2, 3, 4, 5 });
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
            public void CascadedAddingItemsInAddingMode()
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

                var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding);
                var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding);

                fastCollection.Add(1);
                fastCollection.Add(2);
                fastCollection.Add(3);
                fastCollection.Add(4);
                fastCollection.Add(5);

                secondToken.Dispose();
                Assert.AreEqual(0, counter);
                Assert.IsNull(eventArgs);

                firstToken.Dispose();
                Assert.AreEqual(1, counter);
                // ReSharper disable PossibleNullReferenceException
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                CollectionAssert.AreEqual(eventArgs.NewItems, new[] { 1, 2, 3, 4, 5 });
                // ReSharper restore PossibleNullReferenceException
            }

            [TestCase]
            public void CascadedAddingItemsInAddingModeWithInterceptingDisposing()
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

                var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding);
                var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding);

                fastCollection.Add(1);
                fastCollection.Add(2);

                secondToken.Dispose();
                Assert.AreEqual(0, counter);
                Assert.IsNull(eventArgs);

                fastCollection.Add(3);
                fastCollection.Add(4);
                fastCollection.Add(5);

                firstToken.Dispose();
                Assert.AreEqual(1, counter);
                // ReSharper disable PossibleNullReferenceException
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                CollectionAssert.AreEqual(eventArgs.NewItems, new[] { 1, 2, 3, 4, 5 });
                // ReSharper restore PossibleNullReferenceException
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

                CollectionExtensions.AddRange(((ICollection<int>)fastCollection), new[] { 1, 2 });

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

            private SuspensionContext<T> GetSuspensionContext<T>(FastObservableCollection<T> collection)
            {
                var t = typeof(FastObservableCollection<T>);
                var f = t.GetFieldEx("_suspensionContext", BindingFlags.Instance | BindingFlags.NonPublic);
                var v = f.GetValue(collection) as SuspensionContext<T>;

                return v;
            }

            [TestCase]
            public void CleanedUpSuspensionContextAfterAdding()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                }

                var context = GetSuspensionContext(fastCollection);
                Assert.IsNull(context);
            }

            [TestCase]
            public void CleanedUpSuspensionContextAfterDoingNothing()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                }

                var context = GetSuspensionContext(fastCollection);
                Assert.IsNull(context);
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

            [TestCase]
            [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
            public void ThrowsInvalidOperationExceptionForChangingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => { using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing)) { } });
                }
            }
        }
    }
}