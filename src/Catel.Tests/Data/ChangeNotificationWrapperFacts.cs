// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationWrapperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using Catel.Collections;
    using Catel.Data;

    using NUnit.Framework;

    public class ChangeNotificationWrapperFacts
    {
        public class TestModel : ModelBase
        {
            /// <summary>
            /// Gets the first name.
            /// </summary>
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            /// <summary>
            /// Register the FirstName property so it is known in the class.
            /// </summary>
            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
        }

        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ChangeNotificationWrapper(null));
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class TheSupportsNotifyPropertyChangedProperty
        {
            [TestCase]
            public void ReturnsTrueForPropertyChangedItem()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsTrue(wrapper.SupportsNotifyPropertyChanged);
            }

            [TestCase]
            public void ReturnsFalseForNonPropertyChangedItem()
            {
                var model = new object();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsFalse(wrapper.SupportsNotifyPropertyChanged);                
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class TheSupportsNotifyCollectionChangedProperty
        {
            [TestCase]
            public void ReturnsTrueForCollectionChangedItem()
            {
                var collection = new ObservableCollection<int>();
                var wrapper = new ChangeNotificationWrapper(collection);

                Assert.IsTrue(wrapper.SupportsNotifyCollectionChanged);
            }

            [TestCase]
            public void ReturnsFalseForNonCollectionChangedItem()
            {
                var collection = new List<int>();
                var wrapper = new ChangeNotificationWrapper(collection);

                Assert.IsFalse(wrapper.SupportsNotifyCollectionChanged);
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class TheIsUsefulForObjectMethod
        {
            [TestCase]
            public void ReturnsFalseForNullObject()
            {
                Assert.IsFalse(ChangeNotificationWrapper.IsUsefulForObject(null));
            }

            [TestCase]
            public void ReturnsFalseForObjectNotImplementingINotifyPropertyChanged()
            {
                Assert.IsFalse(ChangeNotificationWrapper.IsUsefulForObject(15));
            }

            [TestCase]
            public void ReturnsTrueForObjectImplementingINotifyPropertyChanged()
            {
                Assert.IsTrue(ChangeNotificationWrapper.IsUsefulForObject(new TestModel()));
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class TheUnsubscribeFromAllEventsMethod
        {
            [TestCase]
            public void UnsubscribesFromPropertyChangedEvents()
            {
                var testModel = new TestModel();

                var wrapper = new ChangeNotificationWrapper(testModel);

                wrapper.UnsubscribeFromAllEvents();

                var eventRaised = false;
                wrapper.PropertyChanged += (sender, e) => eventRaised = true;

                testModel.FirstName = "Geert";

                Assert.IsFalse(eventRaised);
            }

            [TestCase]
            public void UnsubscribesFromCollectionChangedEvents()
            {
                var collection = new ObservableCollection<TestModel>();
                var wrapper = new ChangeNotificationWrapper(collection);

                wrapper.UnsubscribeFromAllEvents();

                var eventRaised = false;
                wrapper.CollectionChanged += (sender, e) => eventRaised = true;

                collection.Add(new TestModel());

                Assert.IsFalse(eventRaised);
            }

            [TestCase]
            public void UnsubscribesFromCollectionItemPropertyChangedEvents()
            {
                var testModel = new TestModel();
                var collection = new ObservableCollection<TestModel>(new[] { testModel });

                var wrapper = new ChangeNotificationWrapper(collection);

                wrapper.UnsubscribeFromAllEvents();

                var eventRaised = false;
                wrapper.CollectionItemPropertyChanged += (sender, e) => eventRaised = true;

                testModel.FirstName = "Geert";

                Assert.IsFalse(eventRaised);
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class ThePropertyChangesLogic 
        {
            [TestCase]
            public void HandlesPropertyChangesCorrectly()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                var propertyChanged = false;

                wrapper.PropertyChanged += (sender, e) => propertyChanged = true;

                model.FirstName = "Geert";

                Assert.IsTrue(propertyChanged);
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA), Explicit]
        public class TheCollectionChangesLogic
        {
            [TestCase]
            public void HandlesCollectionChangesCorrectly()
            {
                var collection = new ObservableCollection<TestModel>();
                var wrapper = new ChangeNotificationWrapper(collection);

                var itemsAdded = false;
                var itemsRemoved = false;

                wrapper.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems != null)
                    {
                        itemsRemoved = true;
                    }

                    if (e.NewItems != null)
                    {
                        itemsAdded = true;
                    }
                };

                var model = new TestModel();
                collection.Add(model);
                Assert.IsTrue(itemsAdded, "Item should have been added");
                Assert.IsFalse(itemsRemoved, "Item should not (yet) have been removed");

                Assert.IsTrue(collection.Remove(model), "Item should have been removed from collection");
                Assert.IsTrue(itemsRemoved, "Item should have been removed");
            }

            [TestCase]
            public void HandlesCollectionChangesCorrectlyInSuspensionModeMixedConsolidate()
            {
                var collection = new DispatcherFastObservableCollection<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };

                var wrapper = new ChangeNotificationWrapper(collection);

                var itemsReset = false;
                var itemsAdded = false;
                var itemsRemoved = false;

                var model = new TestModel();
                collection.Add(model);

                wrapper.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems != null)
                    {
                        itemsRemoved = true;
                    }

                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        itemsReset = true;
                    }

                    if (e.NewItems != null)
                    {
                        itemsAdded = true;
                    }
                };

                using (collection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    collection.ReplaceRange(new [] { new TestModel() });
                }

                Assert.IsTrue(itemsAdded, "Items should be added");
                Assert.IsTrue(itemsRemoved, "Items should be removed");
                Assert.IsFalse(itemsReset, "Items should not be reset");
            }

            [TestCase]
            public void HandlesCollectionItemPropertyChangesCorrectly()
            {
                var collection = new ObservableCollection<TestModel>();
                var model = new TestModel();
                collection.Add(model);

                var wrapper = new ChangeNotificationWrapper(collection);

                var collectionItemPropertyChanged = false;

                wrapper.CollectionItemPropertyChanged += (sender, e) => collectionItemPropertyChanged = true;

                model.FirstName = "Geert";

                Assert.IsTrue(collectionItemPropertyChanged);
            }

            [TestCase]
            public void HandlesCollectionResetsCorrectly()
            {
                var collection = new ObservableCollection<TestModel>();
                TestModel model = null;

                for (var i = 0; i < 10; i++)
                {
                    var randomModel = new TestModel();
                    collection.Add(randomModel);
                }

                model = collection[0];

                var wrapper = new ChangeNotificationWrapper(collection);

                var collectionItemPropertyChanged = false;

                wrapper.CollectionItemPropertyChanged += (sender, e) => collectionItemPropertyChanged = true;

                collection.Clear();
                
                model.FirstName = "Geert";

                Assert.IsFalse(collectionItemPropertyChanged);
            }

            [TestCase]
            public void HandlesChangesOfSuspendedFastObservableCollectionCorrectly()
            {
                var collection = new DispatcherFastObservableCollection<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };

                for (var i = 0; i < 10; i++)
                {
                    var randomModel = new TestModel();
                    collection.Add(randomModel);
                }

                var wrapper = new ChangeNotificationWrapper(collection);

                var collectionItemPropertyChanged = false;
                wrapper.CollectionItemPropertyChanged += (sender, e) => collectionItemPropertyChanged = true;

                var newModel = new TestModel();

                using (collection.SuspendChangeNotifications())
                {
                    collection.Clear();
                    collection.Add(newModel);
                }

                newModel.FirstName = "Geert";

                Assert.IsTrue(collectionItemPropertyChanged, "Collection item property should have changed");
            }

            [TestCase]
            public void HandlesClearOfSuspendedFastObservableCollectionCorrectly()
            {
                var collection = new DispatcherFastObservableCollection<TestModel>
                {
                    AutomaticallyDispatchChangeNotifications = false
                };

                TestModel model = null;

                for (var i = 0; i < 10; i++)
                {
                    var randomModel = new TestModel();
                    collection.Add(randomModel);
                }

                model = collection[0];

                var wrapper = new ChangeNotificationWrapper(collection);

                var collectionItemPropertyChanged = false;
                wrapper.CollectionItemPropertyChanged += (sender, e) => collectionItemPropertyChanged = true;

                using (collection.SuspendChangeNotifications())
                {
                    collection.Clear();
                }

                model.FirstName = "Geert";

                Assert.IsFalse(collectionItemPropertyChanged);
            }
        }

        [TestFixture]
        public class TheMemoryLeakChecks
        {
            [TestCase]
            public void DoesNotLeakForPropertyChanged()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsTrue(wrapper.IsObjectAlive);

                model = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.IsFalse(wrapper.IsObjectAlive);
            }

            [TestCase]
            public void DoesNotLeakForCollectionChanged()
            {
                var model = new TestModel();
                var collectionModel = new ObservableCollection<TestModel>(new[] {model});
                var wrapper = new ChangeNotificationWrapper(collectionModel);

                Assert.IsTrue(wrapper.IsObjectAlive);

                collectionModel = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.IsFalse(wrapper.IsObjectAlive);
            }
        }
    }
}
