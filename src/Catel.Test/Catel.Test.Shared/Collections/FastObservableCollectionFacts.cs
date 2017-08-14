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
        internal static class FastObservableCollectionFactsHelper
        {
            public static void Synchronize<T>(IList<T> targetCollection, IList<T> sourceCollection, NotifyRangedCollectionChangedEventArgs eventArg)
            {
                if (eventArg.Action == NotifyCollectionChangedAction.Add)
                {
                    var i = 0;
                    foreach (var index in eventArg.Indices)
                    {
                        var item = (T)eventArg.NewItems[i];
                        targetCollection.Insert(index, item);
                        i++;
                    }
                }
                else if (eventArg.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var index in eventArg.Indices)
                    {
                        targetCollection.RemoveAt(index);
                    }
                }
                else if (eventArg.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (eventArg.SuspensionMode == SuspensionMode.None)
                    {
                        targetCollection.Clear();
                        targetCollection.AddRange(sourceCollection);
                    }
                    else if (eventArg.SuspensionMode == SuspensionMode.Mixed)
                    {
                        var i = 0;
                        foreach (var index in eventArg.Indices)
                        {
                            var action = eventArg.MixedActions[i];
                            if (action == NotifyCollectionChangedAction.Add)
                            {
                                var item = (T)eventArg.ChangedItems[i];
                                targetCollection.Insert(index, item);
                            }
                            else if (action == NotifyCollectionChangedAction.Remove)
                            {
                                targetCollection.RemoveAt(index);
                            }
                            i++;
                        }
                    }
                }
            }
        }

        [TestFixture]
        public class TheIsDirtyProperty
        {
            [Test]
            public void ReturnsFalseWhenNoPendingNotificationsAreListed()
            {
                var fastCollection = new FastObservableCollection<int>();

                fastCollection.Add(1);

                Assert.IsFalse(fastCollection.IsDirty);
            }

            [Test]
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
            [Test]
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

            [Test]
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
            [Test]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null, SuspensionMode.Adding));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing));
            }

            [Test]
            public void RaisesSingleEventWhileAddingRange()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, counter);

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.AreEqual(2, counter);

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.AreEqual(3, counter);

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), SuspensionMode.Adding);

                Assert.AreEqual(4, counter);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeAdding()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };


                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Mixed);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }
        }

        [TestFixture]
        public class TheInsertRangeMethod
        {
            [Test]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.InsertItems(null, 0));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.InsertItems(null, 0, SuspensionMode.Adding));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Removing));
            }

            [Test]
            public void RaisesSingleEventWhileInsertingRange()
            {
                var counter = 0;

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

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeAdding()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Adding);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.None);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Mixed);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }
        }

        [TestFixture]
        public class TheRemoveRangeMethod
        {
            [Test]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null, SuspensionMode.Removing));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding));
            }

            [Test]
            public void RaisesSingleEventWhileRemovingRange()
            {
                var counter = 0;

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

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeRemoving()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Mixed);

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeWithoutSuspensionMode()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                var count = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { eventArgs = e; count++; };

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.AreEqual(1, count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }
        }

        [TestFixture]
        public class TheSuspendNotificationsMethod
        {
            [Test]
            public void SuspendsValidationWhileMovingItems()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Move(1, 3);
                }

                Assert.AreEqual(1, counter);
                Assert.AreEqual(2, fastCollection[3]);
            }

            [Test]
            public void SuspendsValidationWhileMovingItemsWithRemoveAndAdd()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Remove(2);
                    fastCollection.Insert(3, 2);
                }

                Assert.AreEqual(1, counter);
            }

            [Test]
            public void SuspendsValidationWhileDoingNothing()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                }

                Assert.AreEqual(0, counter);
            }

            [Test]
            public void SuspendsValidationWhileClearing()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Clear();
                }

                Assert.AreEqual(1, counter);
                Assert.AreEqual(0, fastCollection.Count);
            }
        }

        [TestFixture]
        public class SupportsLinq
        {
            [Test]
            public void ReturnsSingleElementUsingLinq()
            {
                var fastCollection = new FastObservableCollection<int>();

                for (var i = 0; i < 43; i++) fastCollection.Add(i);

                var allInts = (from x in fastCollection where x == 42 select x).FirstOrDefault();

                Assert.AreEqual(42, allInts);
            }
        }

        [TestFixture]
        public class TheResetMethod
        {
            [Test]
            public void ResetWithoutSuspendChangeNotifications()
            {
                var collectionChanged = false;
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { collectionChanged = true; };

                fastCollection.Reset();

                Assert.AreEqual(true, collectionChanged);
            }

            [Test]
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
            [Test]
            public void AddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; eventArgs = e; };

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

            [Test]
            public void CascadedAddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; eventArgs = e; };

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

            [Test]
            public void CascadedAddingItemsInAddingModeWithInterceptingDisposing()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; eventArgs = e; };

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

            [Test]
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

                fastCollection.AddRange(new[] { 1, 2 });

                fastCollection[0] = 5;

                fastCollection.Move(0, 1);

                fastCollection.Clear();

                Assert.AreEqual(9, counter);
            }

            [Test]
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

            [Test]
            public void CleanedUpSuspensionContextAfterAdding()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                }

                var context = this.GetSuspensionContext(fastCollection);
                Assert.IsNull(context);
            }

            [Test]
            public void CleanedUpSuspensionContextAfterDoingNothing()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                }

                var context = this.GetSuspensionContext(fastCollection);
                Assert.IsNull(context);
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForAddingInRemovingMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Add(0));
                }
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForClearingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Clear());
                }
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForMovingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Move(0, 1));
                }
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForRemovingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection.Remove(0));
                }
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForSettingInAddingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(() => fastCollection[0] = 0);
                }
            }

            [Test]
            [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
            public void ThrowsInvalidOperationExceptionForChangingMode()
            {
                var fastCollection = new FastObservableCollection<int> { 0 };
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    Assert.Throws<InvalidOperationException>(
                        () =>
                            {
                                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Removing))
                                {
                                }
                            });
                }
            }
        }

        [TestFixture]
        public class TheMixedMode
        {
            [Test]
            public void ModeIsMixed()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.Add(4);
                    fastCollection.Remove(3);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(SuspensionMode.Mixed, eventArgsList[0].SuspensionMode);
            }

            [Test]
            public void EventArgsContainMixedItemsAfterClearing()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.Clear();
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(3, eventArgsList.First(args => args.SuspensionMode == SuspensionMode.Mixed).ChangedItems.Count);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChanges()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Add(6);
                    sourceCollection.Remove(3);
                    sourceCollection.Add(7);
                    sourceCollection.Remove(4);
                    sourceCollection.Remove(5);
                    sourceCollection.Add(3);
                }

                Assert.AreEqual(1, eventArgsList.Count);

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Move(0, 3);
                }

                Assert.AreEqual(1, eventArgsList.Count);

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Remove(1);
                    sourceCollection.Insert(3, 1);
                }

                Assert.AreEqual(1, eventArgsList.Count);

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithReplacingItem()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection[0] = 6;
                }

                Assert.AreEqual(1, eventArgsList.Count);

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void RaisesSingleEventForMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Move(0, 3);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            public void RaisesSingleEventForSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Remove(1);
                    sourceCollection.Insert(3, 1);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            public void RaisesSingleEventForAddingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Add(1);
                    sourceCollection.Add(2);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            public void RaisesSingleEventForInsertingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Insert(0, 1);
                    sourceCollection.Insert(1, 2);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            public void RaisesSingleEventForAddingAndMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    sourceCollection.Add(2);
                    sourceCollection.Add(1);
                    sourceCollection.Move(1, 0);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }
        }
    }
}