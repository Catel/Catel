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
    using System.Text;

    using Catel.Collections;
    using Catel.Reflection;

    using NUnit.Framework;

    public class FastObservableCollectionFacts
    {
        internal static class FastObservableCollectionFactsHelper
        {
            public static void Synchronize<T>(IList<T> targetCollection, IList<T> sourceCollection, IEnumerable<NotifyRangedCollectionChangedEventArgs> eventArgs, bool useIndices)
            {
                if (useIndices)
                {
                    foreach (var eventArg in eventArgs)
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
                            targetCollection.Clear();
                            targetCollection.AddRange(sourceCollection);
                        }
                    }
                }
                else
                {
                    foreach (var eventArg in eventArgs)
                    {
                        if (eventArg.Action == NotifyCollectionChangedAction.Add)
                        {
                            targetCollection.AddRange(eventArg.NewItems.Cast<T>());
                        }
                        else if (eventArg.Action == NotifyCollectionChangedAction.Remove)
                        {
                            eventArg.OldItems.Cast<T>().ForEach(item => targetCollection.Remove(item));
                        }
                        else if (eventArg.Action == NotifyCollectionChangedAction.Reset)
                        {
                            targetCollection.Clear();
                            targetCollection.AddRange(sourceCollection);
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

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeAdding()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                int count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                    {
                        eventArgs = e;
                        count++;
                    };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                Assert.AreEqual(1, count);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                int count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                    {
                        eventArgs = e;
                        count++;
                    };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
                Assert.AreEqual(1, count);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);
                int count = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                {
                    eventArgs = e;
                    count++;
                };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Mixed);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                Assert.AreEqual(1, count);
            }

            [Test]
            public void RaisesSingleAddEventWhileAddingRangeWithoutSuspensionMode()
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

                int count = 0;
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) =>
                    {
                        eventArgs = e;
                        count++;
                    };

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Adding);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                Assert.AreEqual(1, count);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Mixed);

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleAddEventWhileInsertingRangeWithoutSuspensionMode()
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

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing);

                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeNone()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.None);

                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgs.Action);
            }

            [Test]
            public void RaisesSingleRemoveEventWhileRemovingRangeInSuspensionModeMixed()
            {
                var eventArgs = default(NotifyCollectionChangedEventArgs);

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => eventArgs = e;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Mixed);

                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
            }

            [Test]
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
            [Test]
            public void SuspendsValidationWhileAddingAndRemovingItems()
            {
                var counter = 0;

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

            [Test]
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

            [Test]
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
                CollectionAssert.AreEqual(eventArgs.OldItems, new[] { 5, 4, 3, 2, 1 });
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
            public void RaisesSingleAddEventIfTheRemovedItemsAreASubSetOfTheAddedItems()
            {
                var count = 0;
                NotifyCollectionChangedEventArgs eventArgs = null;
                var fastCollection = new FastObservableCollection<int>();

                fastCollection.CollectionChanged += (sender, args) =>
                    {
                        count++;
                        eventArgs = args;
                    };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.AddItems(new[] { 1, 2, 3, 4 });
                    fastCollection.RemoveItems(new[] { 2, 3 });
                }

                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
                Assert.AreEqual(1, count);
                Assert.AreEqual(new[] { 4, 1 }, eventArgs.NewItems.OfType<int>().ToArray());
            }

            [Test]
            public void RaisesSingleRemoveEventIfTheAddedItemsAreASubSetOfTheRemovedItems()
            {
                var count = 0;
                NotifyCollectionChangedEventArgs eventArgs = null;
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AddItems(new[] { 1, 2, 3, 4 });

                fastCollection.CollectionChanged += (sender, args) =>
                    {
                        count++;
                        eventArgs = args;
                    };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.RemoveItems(new[] { 4, 2, 3 });
                    fastCollection.AddItems(new[] { 2 });
                }

                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
                Assert.AreEqual(1, count);
                Assert.AreEqual(new[] { 4, 3 }, eventArgs.OldItems.OfType<int>().ToArray());
            }

            [Test]
            public void RaisesTwoEvents()
            {
                var eventArgsList = new List<NotifyCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AddItems(new[] { 1, 2, 3, 4 });

                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add(args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.RemoveItems(new[] { 4 });
                    fastCollection.AddItems(new[] { 5 });
                }

                Assert.AreEqual(2, eventArgsList.Count);

                Assert.Contains(5, eventArgsList.First(args => args.Action == NotifyCollectionChangedAction.Add).NewItems);
                Assert.Contains(4, eventArgsList.First(args => args.Action == NotifyCollectionChangedAction.Remove).OldItems);
            }

            [Test]
            public void EventArgsContainOldItemsAfterClearing()
            {
                var eventArgsList = new List<NotifyCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add(args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    fastCollection.Clear();
                }

                Assert.AreEqual(3, eventArgsList.First(args => args.Action == NotifyCollectionChangedAction.Remove).OldItems.Count);
            }

            [Test]
            [Ignore("Use values have no sense any more since the indexes are correct")]
            public void TargetCollectionAimsSourceCollectionChangesUsingItems()
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

                // Test using items
                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList, false);

                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesUsingIndices()
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

                // Test using indices
                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList, true);

                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithMovingItemsUsingIndices()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Move(0, 3);
                }

                // Test using indices
                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList, true);

                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithSimulatedMovingItemsUsingIndices()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Remove(1);
                    sourceCollection.Insert(3, 1);
                }

                // Test using indices
                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList, true);

                CollectionAssert.AreEqual(sourceCollection, targetCollection);
            }

            [Test]
            [Ignore("In mixed item generate two events")]
            public void RaisesSingleEventForMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Move(0, 3);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            [Ignore("In mixed move item generate two events")]
            public void RaisesSingleEventForSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Remove(1);
                    sourceCollection.Insert(3, 1);
                }

                Assert.AreEqual(1, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Reset, eventArgsList[0].Action);
            }

            [Test]
            public void RaisesTwoEventForMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Move(0, 3);
                }

                Assert.AreEqual(2, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsList[0].Action);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgsList[1].Action);
            }

            [Test]
            public void RaisesTwoEventForSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.Mixed))
                {
                    // Move 1 forward
                    sourceCollection.Remove(1);
                    sourceCollection.Insert(3, 1);
                }

                Assert.AreEqual(2, eventArgsList.Count);
                Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgsList[0].Action);
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgsList[1].Action);
            }
        }
    }
}