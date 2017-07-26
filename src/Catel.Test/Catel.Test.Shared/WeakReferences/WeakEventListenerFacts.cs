// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakEventListenerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
   
    using Catel.MVVM;
    using MVVM.Auditing;

#if NET
    using System.Windows.Data;
#endif

    using NUnit.Framework;

    public class WeakEventListenerFacts
    {
        #region Test classes

        #region Nested type: CustomObservableCollection
        public class CustomObservableCollection : ObservableCollection<DateTime>
        {
        }
        #endregion

        #region Nested type: EventListener
        public class EventListener
        {
            #region Constructors
            public EventListener()
            {
                // Always reset the static handler count
                StaticEventHandlerCounter = 0;
            }
            #endregion

            #region Properties
            public int StaticEventCounter { get; private set; }

            public static int StaticEventHandlerCounter { get; private set; }

            public int PublicActionCounter { get; private set; }

            public int PublicEventCounter { get; private set; }

            public int PrivateEventCounter { get; private set; }

            public int PublicClassEventCount { get; private set; }
            public int PrivateClassEventCount { get; private set; }
            public int PropertyChangedEventCount { get; private set; }
            public int CollectionChangedEventCount { get; private set; }
            #endregion

            #region Methods
            public void OnStaticEvent(object sender, ViewModelClosedEventArgs e)
            {
                StaticEventCounter++;
            }

            public static void OnEventStaticHandler(object sender, EventArgs e)
            {
                StaticEventHandlerCounter++;
            }

            public void OnPublicAction()
            {
                PublicActionCounter++;
            }

            public void OnPublicEvent(object sender, ViewModelClosedEventArgs e)
            {
                PublicEventCounter++;
            }

            public void OnPrivateEvent(object sender, EventArgs e)
            {
                PrivateEventCounter++;
            }

            public IWeakEventListener SubscribeToPublicClassEvent(EventSource source)
            {
                return WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(this, source, "PublicEvent", OnPublicClassEvent);
            }

            public void OnPublicClassEvent(object sender, EventArgs e)
            {
                PublicClassEventCount++;
            }

            public IWeakEventListener SubscribeToPrivateClassEvent(EventSource source)
            {
                return WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(this, source, "PublicEvent", OnPrivateClassEvent);
            }

            private void OnPrivateClassEvent(object sender, EventArgs e)
            {
                PrivateClassEventCount++;
            }

            public void OnPropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {
                PropertyChangedEventCount++;
            }

            public void OnCollectionChangedEvent(object sender, NotifyCollectionChangedEventArgs e)
            {
                CollectionChangedEventCount++;
            }
            #endregion
        }
        #endregion

        #region Nested type: EventSource
        public class EventSource : INotifyPropertyChanged, INotifyCollectionChanged
        {
            #region INotifyCollectionChanged Members
            public event NotifyCollectionChangedEventHandler CollectionChanged;
            #endregion

            #region INotifyPropertyChanged Members
            public event PropertyChangedEventHandler PropertyChanged;
            #endregion

            #region Methods
            public static void RaiseStaticEvent()
            {
                StaticEvent.SafeInvoke(null, () => new ViewModelClosedEventArgs(new TestViewModel(), true));
            }

            public void RaisePublicEvent()
            {
                PublicEvent.SafeInvoke(this, () => new ViewModelClosedEventArgs(new TestViewModel(), true));
            }

            public void RaisePrivateEvent()
            {
                PrivateEvent.SafeInvoke(this, EventArgs.Empty);
            }

            public void RaisePropertyChangedEvent()
            {
                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs(string.Empty));
            }

            public void RaiseCollectionChangedEvent()
            {
                CollectionChanged.SafeInvoke(this, () => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            #endregion

            public static event EventHandler<ViewModelClosedEventArgs> StaticEvent;
            public event EventHandler<ViewModelClosedEventArgs> PublicEvent;
            private event EventHandler<EventArgs> PrivateEvent;
        }
        #endregion

        #endregion

        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void DoesNotLeakWithStaticEvents()
            {
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(listener, null, "StaticEvent", listener.OnPublicEvent);

                Assert.IsFalse(weakEventListener.IsSourceAlive);
                Assert.IsTrue(weakEventListener.IsStaticEvent);
                Assert.IsTrue(weakEventListener.IsTargetAlive);
                Assert.IsFalse(weakEventListener.IsStaticEventHandler);
            }

            [TestCase]
            public void DoesNotLeakWithStaticEventHandlers()
            {
                var source = new EventSource();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(null, source, "PublicEvent", EventListener.OnEventStaticHandler);

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsStaticEvent);
                Assert.IsFalse(weakEventListener.IsTargetAlive);
                Assert.IsTrue(weakEventListener.IsStaticEventHandler);
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionWhenEverythingIsStatic()
            {
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakGenericEvent(null, null, "StaticEvent", EventListener.OnEventStaticHandler));
            }
        }

        [TestFixture]
        public class TheStaticOverloadsWithoutAnyTypeSpecificationMethods
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullListener()
            {
                var source = new EventSource();
                var listener = new EventListener();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => WeakEventListener.SubscribeToWeakGenericEvent<ViewModelClosedEventArgs>(null, source, "event", listener.OnPublicEvent));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullSource()
            {
                var listener = new EventListener();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => listener.SubscribeToWeakGenericEvent<ViewModelClosedEventArgs>(null, "event", listener.OnPublicEvent));
            }

            [TestCase]
            public void DoesNotLeakWithAutomaticDetectionOfAllTypes()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToWeakGenericEvent<ViewModelClosedEventArgs>(source, "PublicEvent", listener.OnPublicEvent);

                Assert.AreEqual(0, listener.PublicEventCounter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PublicEventCounter);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithAutomaticDetectionOfAllTypesForPropertyChanged()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToWeakPropertyChangedEvent(source, listener.OnPropertyChangedEvent);

                Assert.AreEqual(0, listener.PropertyChangedEventCount);

                source.RaisePropertyChangedEvent();

                Assert.AreEqual(1, listener.PropertyChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithAutomaticDetectionOfAllTypesForCollectionChanged()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToWeakCollectionChangedEvent(source, listener.OnCollectionChangedEvent);

                Assert.AreEqual(0, listener.CollectionChangedEventCount);

                source.RaiseCollectionChangedEvent();

                Assert.AreEqual(1, listener.CollectionChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }
        }

        [TestFixture]
        public class TheStaticOverloadsWithoutEventArgsSpecificationMethods
        {
            [TestCase]
            public void DoesNotLeakWithAutomaticEventArgsDetection()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource>.SubscribeToWeakGenericEvent<ViewModelClosedEventArgs>(listener, source, "PublicEvent", listener.OnPublicEvent);

                Assert.AreEqual(0, listener.PublicEventCounter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PublicEventCounter);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithAutomaticPropertyChangedEventArgsDetection()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource>.SubscribeToWeakPropertyChangedEvent(listener, source, listener.OnPropertyChangedEvent);

                Assert.AreEqual(0, listener.PropertyChangedEventCount);

                source.RaisePropertyChangedEvent();

                Assert.AreEqual(1, listener.PropertyChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithAutomaticCollectionChangedEventArgsDetection()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource>.SubscribeToWeakCollectionChangedEvent(listener, source, listener.OnCollectionChangedEvent);

                Assert.AreEqual(0, listener.CollectionChangedEventCount);

                source.RaiseCollectionChangedEvent();

                Assert.AreEqual(1, listener.CollectionChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }
        }

        [TestFixture]
        public class TheSubscribeToWeakCollectionChangedEventMethod
        {
            [TestCase]
            public void AutomaticallySubscribesToCollectionChangedWhenEventNameIsNotSpecified()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, NotifyCollectionChangedEventArgs>.SubscribeToWeakCollectionChangedEvent(listener, source, listener.OnCollectionChangedEvent);

                Assert.AreEqual(0, listener.CollectionChangedEventCount);

                source.RaiseCollectionChangedEvent();

                Assert.AreEqual(1, listener.CollectionChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void SupportsCustomCollections()
            {
                var source = new CustomObservableCollection();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToWeakCollectionChangedEvent(source, listener.OnCollectionChangedEvent);

                Assert.AreEqual(0, listener.CollectionChangedEventCount);

                source.Add(DateTime.Now);

                Assert.AreEqual(1, listener.CollectionChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

#if NET
            [TestCase]
            public void SupportsExplicitlyImplementedEvents()
            {
                var listener = new EventListener();

                // ObservableCollection implements ICollectionChanged explicitly
                var source = new ListCollectionView(new List<int>(new [] { 1, 2, 3 }));

                WeakEventListener.SubscribeToWeakCollectionChangedEvent(listener, source, listener.OnCollectionChangedEvent);

                source.AddNewItem(4);

                Assert.AreEqual(1, listener.CollectionChangedEventCount);
            }
#endif

            [TestCase]
            public void DoesNotLeakWithCollectionChangedEvent()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, NotifyCollectionChangedEventArgs>.SubscribeToWeakCollectionChangedEvent(listener, source, listener.OnCollectionChangedEvent, eventName: "CollectionChanged");

                Assert.AreEqual(0, listener.CollectionChangedEventCount);

                source.RaiseCollectionChangedEvent();

                Assert.AreEqual(1, listener.CollectionChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }
        }

        [TestFixture]
        public class TheSubscribeToWeakPropertyChangedEventMethod
        {
            [TestCase]
            public void AutomaticallySubscribesToPropertyChangedWhenEventNameIsNotSpecified()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, PropertyChangedEventArgs>.SubscribeToWeakPropertyChangedEvent(listener, source, listener.OnPropertyChangedEvent);

                Assert.AreEqual(0, listener.PropertyChangedEventCount);

                source.RaisePropertyChangedEvent();

                Assert.AreEqual(1, listener.PropertyChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void SupportsExplicitlyImplementedEvents()
            {
                var listener = new EventListener();

                // ObservableCollection implements INotifyPropertyChanged explicitly
                var source = new ObservableCollection<int>();

                WeakEventListener.SubscribeToWeakPropertyChangedEvent(listener, source, listener.OnPropertyChangedEvent);

                source.Add(1);

                Assert.AreEqual(2, listener.PropertyChangedEventCount);
            }

            [TestCase]
            public void DoesNotLeakWithPropertyChangedEvent()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, PropertyChangedEventArgs>.SubscribeToWeakPropertyChangedEvent(listener, source, listener.OnPropertyChangedEvent, eventName: "PropertyChanged");

                Assert.AreEqual(0, listener.PropertyChangedEventCount);

                source.RaisePropertyChangedEvent();

                Assert.AreEqual(1, listener.PropertyChangedEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }
        }

        [TestFixture]
        public class TheWeakEventListener
        {
            [TestCase]
            public void DoesNotLeakWithoutAnyInvocation()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(listener, source, "PublicEvent", listener.OnPublicEvent);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithStaticEvents()
            {
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(listener, null, "StaticEvent", listener.OnPublicEvent);

                Assert.AreEqual(0, listener.StaticEventCounter);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsFalse(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);
            }

            [TestCase]
            public void DoesNotLeakWithStaticEventHandlers()
            {
                var source = new EventSource();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(null, source, "PublicEvent", EventListener.OnEventStaticHandler);

                Assert.AreEqual(0, EventListener.StaticEventHandlerCounter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, EventListener.StaticEventHandlerCounter);

                // Some dummy code to make sure the previous source is removed
                source = new EventSource();
                GC.Collect();
                var type = source.GetType();

                Assert.IsFalse(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);
            }

            [TestCase]
            public void DoesNotLeakWithAnonymousHandlers()
            {
                var source = new EventSource();
                var listener = new EventListener();
                int count = 0;

                WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(listener, source, "PublicEvent", (sender, e) => count++);

                Assert.AreEqual(0, count);

                source.RaisePublicEvent();

                Assert.AreEqual(1, count);
            }

            [TestCase]
            public void DoesNotLeakWithPublicActions()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener.SubscribeToWeakEvent(listener, source, "PublicEvent", listener.OnPublicAction);

                Assert.AreEqual(0, listener.PublicActionCounter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PublicActionCounter);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithActionsDefinedInDisplayClass()
            {
                var source = new EventSource();

                var counter = 0;

                Action action = () =>
                {
                    counter++;
                };

                var weakEventListener = WeakEventListener.SubscribeToWeakEvent(this, source, "PublicEvent", action);

                Assert.AreEqual(0, counter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, counter);

                // Some dummy code to make sure the previous listener is removed
                GC.Collect();

                //Assert.IsTrue(weakEventListener.IsSourceAlive);
                //Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void DoesNotLeakWithPublicEvents()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = WeakEventListener<EventListener, EventSource, ViewModelClosedEventArgs>.SubscribeToWeakGenericEvent(listener, source, "PublicEvent", listener.OnPublicEvent);

                Assert.AreEqual(0, listener.PublicEventCounter);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PublicEventCounter);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

            [TestCase]
            public void ThrowsExceptionWithPrivateEvents()
            {
                var source = new EventSource();
                var listener = new EventListener();

                try
                {
                    var weakEventListener = WeakEventListener<EventListener, EventSource, EventArgs>.SubscribeToWeakGenericEvent(listener, source, "PrivateEvent", listener.OnPrivateEvent);

                    Assert.Fail("Expected an exception because this is a private event subscribed to from the outside");
                }
                catch (Exception)
                {
                }
            }

            [TestCase]
            public void DoesNotLeakWithPublicEventHandlerSubscribedFromClassItself()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToPublicClassEvent(source);

                Assert.AreEqual(0, listener.PublicClassEventCount);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PublicClassEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }

#if NET
            [TestCase]
            public void DoesNotLeakWithPrivateEventHandlerSubscribedFromClassItself()
            {
                var source = new EventSource();
                var listener = new EventListener();

                var weakEventListener = listener.SubscribeToPrivateClassEvent(source);

                Assert.AreEqual(0, listener.PrivateClassEventCount);

                source.RaisePublicEvent();

                Assert.AreEqual(1, listener.PrivateClassEventCount);

                // Some dummy code to make sure the previous listener is removed
                listener = new EventListener();
                GC.Collect();
                var type = listener.GetType();

                Assert.IsTrue(weakEventListener.IsSourceAlive);
                Assert.IsFalse(weakEventListener.IsTargetAlive);

                // Some dummy code to make sure the source stays in memory
                source.GetType();
            }
#endif
        }
    }
}