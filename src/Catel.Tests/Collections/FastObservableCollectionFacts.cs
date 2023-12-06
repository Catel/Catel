namespace Catel.Tests.Collections
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
            public static void Synchronize<T>(IList<T> targetCollection, IList<T> sourceCollection, IList<NotifyRangedCollectionChangedEventArgs> eventArgsList)
            {
                eventArgsList.ForEach(eventArgs => Synchronize(targetCollection, sourceCollection, eventArgs));
            }

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

                Assert.That(fastCollection.IsDirty, Is.False);
            }

            [Test]
            public void ReturnsTrueWhenPendingNotificationsAreListed()
            {
                var fastCollection = new FastObservableCollection<int>();

                using (fastCollection.SuspendChangeNotifications())
                {
                    fastCollection.Add(1);

                    Assert.That(fastCollection.IsDirty, Is.True);
                }

                Assert.That(fastCollection.IsDirty, Is.False);
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

                    Assert.That(fastCollection.NotificationsSuspended, Is.True);
                }

                Assert.That(fastCollection.NotificationsSuspended, Is.False);
            }

            [Test]
            public void ReturnsFalseAfterChangedDisposing()
            {
                var fastCollection = new FastObservableCollection<int>();

                using (var firstToken = fastCollection.SuspendChangeNotifications())
                {
                    using (var secondToken = fastCollection.SuspendChangeNotifications())
                    {
                    }
                }

                Assert.That(fastCollection.NotificationsSuspended, Is.False);
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
                Assert.Throws<ArgumentNullException>(() => fastCollection.AddItems(null));
                Assert.Throws<ArgumentNullException>(() => fastCollection.AddItems(null, SuspensionMode.Adding));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                Assert.Throws<InvalidOperationException>(() => fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing));
            }

            [Test]
            public void RaisesSingleEventWhileAddingRange()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; };

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 });

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.AddItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding);

                Assert.That(counter, Is.EqualTo(2));

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.That(counter, Is.EqualTo(3));

                fastCollection.AddItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), SuspensionMode.Adding);

                Assert.That(counter, Is.EqualTo(4));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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
                Assert.Throws<ArgumentNullException>(() => fastCollection.InsertItems(null, 0));
                Assert.Throws<ArgumentNullException>(() => fastCollection.InsertItems(null, 0, SuspensionMode.Adding));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                Assert.Throws<InvalidOperationException>(() => fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Removing));
            }

            [Test]
            public void RaisesSingleEventWhileInsertingRange()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0);

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.InsertItems(new[] { 1, 2, 3, 4, 5 }, 0, SuspensionMode.Adding);

                Assert.That(counter, Is.EqualTo(2));

                fastCollection.InsertItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), 0);

                Assert.That(counter, Is.EqualTo(3));

                fastCollection.InsertItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), 0, SuspensionMode.Adding);

                Assert.That(counter, Is.EqualTo(4));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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
                Assert.Throws<ArgumentNullException>(() => fastCollection.RemoveItems(null));
                Assert.Throws<ArgumentNullException>(() => fastCollection.RemoveItems(null, SuspensionMode.Removing));
            }

            [Test]
            public void ThrowsInvalidOperationExceptionForInvalidSuspensionMode()
            {
                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                Assert.Throws<InvalidOperationException>(() => fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Adding));
            }

            [Test]
            public void RaisesSingleEventWhileRemovingRange()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>(new[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 });
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 });

                Assert.That(counter, Is.EqualTo(1));

                fastCollection.RemoveItems(new[] { 1, 2, 3, 4, 5 }, SuspensionMode.Removing);

                Assert.That(counter, Is.EqualTo(2));

                fastCollection.RemoveItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }));

                Assert.That(counter, Is.EqualTo(3));

                fastCollection.RemoveItems(new ArrayList(new[] { 1, 2, 3, 4, 5 }), SuspensionMode.Removing);

                Assert.That(counter, Is.EqualTo(4));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(count, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(fastCollection[3], Is.EqualTo(2));
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

                Assert.That(counter, Is.EqualTo(1));
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

                Assert.That(counter, Is.EqualTo(0));
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

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(fastCollection.Count, Is.EqualTo(0));
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

                Assert.That(allInts, Is.EqualTo(42));
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

                Assert.That(collectionChanged, Is.EqualTo(false));
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

                using (var token = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);

                    fastCollection.Reset();
                    Assert.That(counter, Is.EqualTo(0));

                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
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

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);
            }

            [Test]
            public void CascadedAddingItemsInAddingMode()
            {
                var counter = 0;
                var eventArgs = (NotifyCollectionChangedEventArgs)null;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => { counter++; eventArgs = e; };

                using (var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    using (var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                    {
                        fastCollection.Add(1);
                        fastCollection.Add(2);
                        fastCollection.Add(3);
                        fastCollection.Add(4);
                        fastCollection.Add(5);
                    }

                    Assert.That(counter, Is.EqualTo(0));
                    Assert.That(eventArgs, Is.Null);
                }

                Assert.That(counter, Is.EqualTo(1));

                // ReSharper disable PossibleNullReferenceException
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);

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

                using (var firstToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                    using (var secondToken = fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                    {
                        fastCollection.Add(1);
                        fastCollection.Add(2);
                    }

                    Assert.That(counter, Is.EqualTo(0));
                    Assert.That(eventArgs, Is.Null);

                    fastCollection.Add(3);
                    fastCollection.Add(4);
                    fastCollection.Add(5);
                }

                Assert.That(counter, Is.EqualTo(1));

                // ReSharper disable PossibleNullReferenceException
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.NewItems).AsCollection);

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

                Assert.That(counter, Is.EqualTo(9));
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

                Assert.That(counter, Is.EqualTo(1));
                Assert.That(eventArgs.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(new[] { 1, 2, 3, 4, 5 }, Is.EqualTo(eventArgs.OldItems).AsCollection);
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

                var context = GetSuspensionContext(fastCollection);
                Assert.That(context, Is.Null);
            }

            [Test]
            public void CleanedUpSuspensionContextAfterDoingNothing()
            {
                var fastCollection = new FastObservableCollection<int>();
                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Adding))
                {
                }

                var context = GetSuspensionContext(fastCollection);
                Assert.That(context, Is.Null);
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].SuspensionMode, Is.EqualTo(SuspensionMode.Mixed));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList.First(args => args.SuspensionMode == SuspensionMode.Mixed).ChangedItems.Count, Is.EqualTo(3));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList[0]);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
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

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
            }
        }

        [TestFixture]
        public class TheMixedBashMode
        {
            [Test]
            public void EventArgsContainMixedItemsAfterClearing()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    fastCollection.Clear(); // { }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList.First(args => args.SuspensionMode == SuspensionMode.MixedBash).ChangedItems.Count, Is.EqualTo(3));
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChanges()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Add(6); // { 1, 2, 3, 4, 5, 6 };
                    sourceCollection.Remove(3); // { 1, 2, 4, 5, 6 };
                    sourceCollection.Add(7); // { 1, 2, 4, 5, 6, 7 };
                    sourceCollection.Remove(4); // { 1, 2, 5, 6, 7 };
                    sourceCollection.Remove(5); // { 1, 2, 6, 7 };
                    sourceCollection.Add(3); // { 1, 2, 6, 7, 3 };
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(5));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[2].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(eventArgsList[3].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[4].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithReplacingItem()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection[0] = 6; // { 6, 2, 3, 4, 5 };
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesTwoEventForMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Move(0, 3); // { 2, 3, 4, 1, 5 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesTwoEventForSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Remove(1); // { 2, 3, 4, 5 }
                    sourceCollection.Insert(3, 1); // { 2, 3, 4, 1, 5 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventForAddingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Add(1); // { 1 }
                    sourceCollection.Add(2); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesThreeEventsForAddingAndMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Add(2); // { 2 }
                    sourceCollection.Add(1); // { 2, 1 }
                    sourceCollection.Move(1, 0); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(3));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[2].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventForInsertingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Insert(0, 1); // { 1 }
                    sourceCollection.Insert(1, 2); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventWithSingleItemWithMixedBashModeAndAddAction()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    fastCollection.Add(4);
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].ChangedItems.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].NewItems.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].SuspensionMode, Is.EqualTo(SuspensionMode.MixedBash));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithAddingAndRemovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedBash))
                {
                    sourceCollection.Add(1);
                    sourceCollection.Add(2);
                    sourceCollection.Add(3);
                    sourceCollection.Remove(3);
                    sourceCollection.Remove(2);
                    sourceCollection.Add(2);
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(3));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }
        }

        [TestFixture]
        public class TheMixedConsolidateMode
        {
            [Test]
            public void EventArgsContainMixedItemsAfterClearing()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { 1, 2, 3 };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    fastCollection.Clear(); // { }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList.First(args => args.SuspensionMode == SuspensionMode.MixedConsolidate).ChangedItems.Count, Is.EqualTo(3));
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChanges()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Add(6); // { 1, 2, 3, 4, 5, 6 };
                    sourceCollection.Remove(3); // { 1, 2, 4, 5, 6 };
                    sourceCollection.Add(7); // { 1, 2, 4, 5, 6, 7 };
                    sourceCollection.Remove(4); // { 1, 2, 5, 6, 7 };
                    sourceCollection.Remove(5); // { 1, 2, 6, 7 };
                    sourceCollection.Add(3); // { 1, 2, 6, 7, 3 };
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(5));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[2].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
                Assert.That(eventArgsList[3].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[4].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithReplacingItem()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection[0] = 6; // { 6, 2, 3, 4, 5 };
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesTwoEventForMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Move(0, 3); // { 2, 3, 4, 1, 5 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesTwoEventForSimulatedMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { 1, 2, 3, 4, 5 };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Remove(1); // { 2, 3, 4, 5 }
                    sourceCollection.Insert(3, 1); // { 2, 3, 4, 1, 5 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
                Assert.That(eventArgsList[1].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { 1, 2, 3, 4, 5 };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventForAddingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Add(1); // { 1 }
                    sourceCollection.Add(2); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventForInsertingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Insert(0, 1); // { 1 }
                    sourceCollection.Insert(1, 2); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventsForAddingAndMovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int>();
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Add(2); // { 2 }
                    sourceCollection.Add(1); // { 2, 1 }
                    sourceCollection.Move(1, 0); // { 1, 2 }
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));

                var targetCollection = new List<int>();
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }

            [Test]
            public void RaisesSingleEventForAddAndRemoveActions()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var fastCollection = new FastObservableCollection<int> { };
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    fastCollection.Add(1);
                    fastCollection.Add(2);
                    fastCollection.Add(3);
                    fastCollection.Remove(3);
                    fastCollection.Remove(2);
                    fastCollection.Add(2);
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));
                Assert.That(eventArgsList[0].ChangedItems.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].NewItems.Count, Is.EqualTo(2));
                Assert.That(eventArgsList[0].SuspensionMode, Is.EqualTo(SuspensionMode.MixedConsolidate));
                Assert.That(eventArgsList[0].Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            }

            [Test]
            public void TargetCollectionAimsSourceCollectionChangesWithAddingAndRemovingItems()
            {
                var eventArgsList = new List<NotifyRangedCollectionChangedEventArgs>();
                var sourceCollection = new FastObservableCollection<int> { };
                sourceCollection.AutomaticallyDispatchChangeNotifications = false;
                sourceCollection.CollectionChanged += (sender, args) => { eventArgsList.Add((NotifyRangedCollectionChangedEventArgs)args); };

                using (sourceCollection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    sourceCollection.Add(1);
                    sourceCollection.Add(2);
                    sourceCollection.Add(3);
                    sourceCollection.Remove(3);
                    sourceCollection.Remove(2);
                    sourceCollection.Add(2);
                }

                Assert.That(eventArgsList.Count, Is.EqualTo(1));

                var targetCollection = new List<int> { };
                FastObservableCollectionFactsHelper.Synchronize(targetCollection, sourceCollection, eventArgsList);
                Assert.That(targetCollection, Is.EqualTo(sourceCollection).AsCollection);
            }
        }

        [TestFixture]
        public class TheSilentMode
        {
            [Test]
            public void MultipleActionsInSilentMode()
            {
                var counter = 0;

                var fastCollection = new FastObservableCollection<int>();
                fastCollection.AutomaticallyDispatchChangeNotifications = false;
                fastCollection.CollectionChanged += (sender, e) => counter++;

                using (fastCollection.SuspendChangeNotifications(SuspensionMode.Silent))
                {
                    fastCollection.Add(0);
                    fastCollection.Add(1);

                    fastCollection.Remove(0);
                    fastCollection.Remove(1);

                    fastCollection.AddRange(new[] { 1, 2 });

                    fastCollection[0] = 5;

                    fastCollection.Move(0, 1);

                    fastCollection.Clear();
                }

                Assert.That(counter, Is.EqualTo(0));
            }
        }
    }
}
