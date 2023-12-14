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
            public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);
        }

        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValue()
            {
                Assert.Throws<ArgumentNullException>(() => new ChangeNotificationWrapper(null));
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

                Assert.That(wrapper.SupportsNotifyPropertyChanged, Is.True);
            }

            [TestCase]
            public void ReturnsFalseForNonPropertyChangedItem()
            {
                var model = new object();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.That(wrapper.SupportsNotifyPropertyChanged, Is.False);
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

                Assert.That(wrapper.SupportsNotifyCollectionChanged, Is.True);
            }

            [TestCase]
            public void ReturnsFalseForNonCollectionChangedItem()
            {
                var collection = new List<int>();
                var wrapper = new ChangeNotificationWrapper(collection);

                Assert.That(wrapper.SupportsNotifyCollectionChanged, Is.False);
            }
        }

        [TestFixture, RequiresThread(System.Threading.ApartmentState.STA)]
        public class TheIsUsefulForObjectMethod
        {
            [TestCase]
            public void ReturnsFalseForNullObject()
            {
                Assert.That(ChangeNotificationWrapper.IsUsefulForObject(null), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForObjectNotImplementingINotifyPropertyChanged()
            {
                Assert.That(ChangeNotificationWrapper.IsUsefulForObject(15), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForObjectImplementingINotifyPropertyChanged()
            {
                Assert.That(ChangeNotificationWrapper.IsUsefulForObject(new TestModel()), Is.True);
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

                Assert.That(eventRaised, Is.False);
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

                Assert.That(eventRaised, Is.False);
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

                Assert.That(eventRaised, Is.False);
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

                Assert.That(propertyChanged, Is.True);
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
                    if (e.OldItems is not null)
                    {
                        itemsRemoved = true;
                    }

                    if (e.NewItems is not null)
                    {
                        itemsAdded = true;
                    }
                };

                var model = new TestModel();
                collection.Add(model);
                Assert.That(itemsAdded, Is.True, "Item should have been added");
                Assert.That(itemsRemoved, Is.False, "Item should not (yet) have been removed");

                Assert.That(collection.Remove(model), Is.True, "Item should have been removed from collection");
                Assert.That(itemsRemoved, Is.True, "Item should have been removed");
            }

            [TestCase]
            public void HandlesCollectionChangesCorrectlyInSuspensionModeMixedConsolidate()
            {
                var collection = new FastObservableCollection<TestModel>
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
                    if (e.OldItems is not null)
                    {
                        itemsRemoved = true;
                    }

                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        itemsReset = true;
                    }

                    if (e.NewItems is not null)
                    {
                        itemsAdded = true;
                    }
                };

                using (collection.SuspendChangeNotifications(SuspensionMode.MixedConsolidate))
                {
                    collection.ReplaceRange(new[] { new TestModel() });
                }

                Assert.That(itemsAdded, Is.True, "Items should be added");
                Assert.That(itemsRemoved, Is.True, "Items should be removed");
                Assert.That(itemsReset, Is.False, "Items should not be reset");
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

                Assert.That(collectionItemPropertyChanged, Is.True);
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

                Assert.That(collectionItemPropertyChanged, Is.False);
            }

            [TestCase]
            public void HandlesChangesOfSuspendedFastObservableCollectionCorrectly()
            {
                var collection = new FastObservableCollection<TestModel>
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

                Assert.That(collectionItemPropertyChanged, Is.True, "Collection item property should have changed");
            }

            [TestCase]
            public void HandlesClearOfSuspendedFastObservableCollectionCorrectly()
            {
                var collection = new FastObservableCollection<TestModel>
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

                Assert.That(collectionItemPropertyChanged, Is.False);
            }
        }

        [TestFixture, Explicit]
        public class TheMemoryLeakChecks
        {
            [TestCase]
            public void DoesNotLeakForPropertyChanged()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.That(wrapper.IsObjectAlive, Is.True);

                model = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.That(wrapper.IsObjectAlive, Is.False);
            }

            [TestCase]
            public void DoesNotLeakForCollectionChanged()
            {
                var model = new TestModel();
                var collectionModel = new ObservableCollection<TestModel>(new[] { model });
                var wrapper = new ChangeNotificationWrapper(collectionModel);

                Assert.That(wrapper.IsObjectAlive, Is.True);

                collectionModel = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.That(wrapper.IsObjectAlive, Is.False);
            }
        }
    }
}
