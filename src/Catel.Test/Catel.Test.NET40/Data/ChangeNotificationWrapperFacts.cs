// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeNotificationWrapperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

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

        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValue()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ChangeNotificationWrapper(null));
            }
        }

        [TestClass]
        public class TheSupportsNotifyPropertyChangedProperty
        {
            [TestMethod]
            public void ReturnsTrueForPropertyChangedItem()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsTrue(wrapper.SupportsNotifyPropertyChanged);
            }

            [TestMethod]
            public void ReturnsFalseForNonPropertyChangedItem()
            {
                var model = new object();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsFalse(wrapper.SupportsNotifyPropertyChanged);                
            }
        }

        [TestClass]
        public class TheSupportsNotifyCollectionChangedProperty
        {
            [TestMethod]
            public void ReturnsTrueForCollectionChangedItem()
            {
                var collection = new ObservableCollection<int>();
                var wrapper = new ChangeNotificationWrapper(collection);

                Assert.IsTrue(wrapper.SupportsNotifyCollectionChanged);
            }

            [TestMethod]
            public void ReturnsFalseForNonCollectionChangedItem()
            {
                var collection = new List<int>();
                var wrapper = new ChangeNotificationWrapper(collection);

                Assert.IsFalse(wrapper.SupportsNotifyCollectionChanged);
            }
        }

        [TestClass]
        public class TheIsUsefulForObjectMethod
        {
            [TestMethod]
            public void ReturnsFalseForNullObject()
            {
                Assert.IsFalse(ChangeNotificationWrapper.IsUsefulForObject(null));
            }

            [TestMethod]
            public void ReturnsFalseForObjectNotImplementingINotifyPropertyChanged()
            {
                Assert.IsFalse(ChangeNotificationWrapper.IsUsefulForObject(15));
            }

            [TestMethod]
            public void ReturnsTrueForObjectImplementingINotifyPropertyChanged()
            {
                Assert.IsTrue(ChangeNotificationWrapper.IsUsefulForObject(new TestModel()));
            }
        }

        [TestClass]
        public class TheUnsubscribeFromAllEventsMethod
        {
            [TestMethod]
            public void UnsubscribesFromPropertyChangedEvents()
            {
                var testModel = new TestModel();

                var wrapper = new ChangeNotificationWrapper(testModel);

                wrapper.UnsubscribeFromAllEvents();

                bool eventRaised = false;
                wrapper.PropertyChanged += (sender, e) => eventRaised = true;

                testModel.FirstName = "Geert";

                Assert.IsFalse(eventRaised);
            }

            [TestMethod]
            public void UnsubscribesFromCollectionChangedEvents()
            {
                var collection = new ObservableCollection<TestModel>();
                var wrapper = new ChangeNotificationWrapper(collection);

                wrapper.UnsubscribeFromAllEvents();

                bool eventRaised = false;
                wrapper.CollectionChanged += (sender, e) => eventRaised = true;

                collection.Add(new TestModel());

                Assert.IsFalse(eventRaised);
            }

            [TestMethod]
            public void UnsubscribesFromCollectionItemPropertyChangedEvents()
            {
                var testModel = new TestModel();
                var collection = new ObservableCollection<TestModel>(new[] { testModel });

                var wrapper = new ChangeNotificationWrapper(collection);

                wrapper.UnsubscribeFromAllEvents();

                bool eventRaised = false;
                wrapper.CollectionItemPropertyChanged += (sender, e) => eventRaised = true;

                testModel.FirstName = "Geert";

                Assert.IsFalse(eventRaised);
            }
        }

        [TestClass]
        public class ThePropertyChangesLogic 
        {
            [TestMethod]
            public void HandlesPropertyChangesCorrectly()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                bool propertyChanged = false;

                wrapper.PropertyChanged += (sender, e) => propertyChanged = true;

                model.FirstName = "Geert";

                Assert.IsTrue(propertyChanged);
            }
        }

        [TestClass]
        public class TheCollectionChangesLogic
        {
            [TestMethod]
            public void HandlesCollectionChangesCorrectly()
            {
                var collection = new ObservableCollection<TestModel>();
                var wrapper = new ChangeNotificationWrapper(collection);

                bool itemsAdded = false;
                bool itemsRemoved = false;

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
                Assert.IsTrue(itemsAdded);
                Assert.IsFalse(itemsRemoved);

                collection.Remove(model);
                Assert.IsTrue(itemsRemoved);
            }

            [TestMethod]
            public void HandlesCollectionItemPropertyChangesCorrectly()
            {
                var collection = new ObservableCollection<TestModel>();
                var model = new TestModel();
                collection.Add(model);

                var wrapper = new ChangeNotificationWrapper(collection);

                bool collectionItemPropertyChanged = false;

                wrapper.CollectionItemPropertyChanged += (sender, e) => collectionItemPropertyChanged = true;

                model.FirstName = "Geert";

                Assert.IsTrue(collectionItemPropertyChanged);
            }
        }

        [TestClass]
        public class TheMemoryLeakChecks
        {
            [TestMethod]
            public void DoesNotLeakForPropertyChanged()
            {
                var model = new TestModel();
                var wrapper = new ChangeNotificationWrapper(model);

                Assert.IsTrue(wrapper.IsObjectAlive);

                model = null;
                GC.Collect();

                Assert.IsFalse(wrapper.IsObjectAlive);
            }

            [TestMethod]
            public void DoesNotLeakForCollectionChanged()
            {
                var model = new TestModel();
                var collectionModel = new ObservableCollection<TestModel>(new[] {model});
                var wrapper = new ChangeNotificationWrapper(collectionModel);

                Assert.IsTrue(wrapper.IsObjectAlive);

                collectionModel = null;
                GC.Collect();

                Assert.IsFalse(wrapper.IsObjectAlive);
            }
        }
    }
}