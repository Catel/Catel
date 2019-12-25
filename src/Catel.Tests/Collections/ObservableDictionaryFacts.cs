// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableDictionaryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2019 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Catel.Collections;
    using NUnit.Framework;

    public class ObservableDictionaryFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [Test]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                Assert.That(() => new DispatcherObservableDictionary<object, object>(null, null), Throws.ArgumentNullException);
            }

            [Test]
            public void ReturnsDefaultComparer()
            {
                var defaultComparer = EqualityComparer<int>.Default;

                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                Assert.AreEqual(defaultComparer, observableDictionary.Comparer);
            }

            [Test]
            public void ReturnsCustomComparer()
            {
                var customComparer = StringComparer.OrdinalIgnoreCase;

                var observableDictionary = new DispatcherObservableDictionary<string, int>(customComparer)
                {
                    {
                        "1", 1
                    }
                };

                Assert.AreEqual(customComparer, observableDictionary.Comparer);
            }
        }

        [TestFixture]
        public class TheAddMethod
        {
            [Test]
            public void ThrowsInvalidCastExceptionForAddObject()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                Assert.That(() => observableDictionary.Add((object)"1", 1), Throws.TypeOf<InvalidCastException>());
            }

            [Test]
            public void ThrowsArgumentNullExceptionForAddObject()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                Assert.That(() => observableDictionary.Add(1, null), Throws.ArgumentNullException);
            }

            [Test]
            public void RaisesEventWhileAddingKvp()
            {
                var counter = 0;
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add(new KeyValuePair<int, int>(1, 1));
                Assert.AreEqual(1, counter);

                observableDictionary.Add(new KeyValuePair<int, int>(2, 2));
                Assert.AreEqual(2, counter);

                observableDictionary.Add(new KeyValuePair<int, int>(3, 3));
                Assert.AreEqual(3, counter);

                observableDictionary.Add(new KeyValuePair<int, int>(4, 4));
                Assert.AreEqual(4, counter);
            }

            [Test]
            public void RaisesEventWhileAddingObject()
            {
                var counter = 0;
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add((object)1, (object)1);
                Assert.AreEqual(1, counter);

                observableDictionary.Add((object)2, (object)2);
                Assert.AreEqual(2, counter);

                observableDictionary.Add((object)3, (object)3);
                Assert.AreEqual(3, counter);

                observableDictionary.Add((object)4, (object)4);
                Assert.AreEqual(4, counter);
            }

            [Test]
            public void RaiseEventWhileAddingStronglyTyped()
            {
                var counter = 0;
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary.Add(1, 1);
                Assert.AreEqual(1, counter);

                observableDictionary.Add(2, 2);
                Assert.AreEqual(2, counter);

                observableDictionary.Add(3, 3);
                Assert.AreEqual(3, counter);

                observableDictionary.Add(4, 4);
                Assert.AreEqual(4, counter);
            }
        }

        [TestFixture]
        public class TheClearMethod
        {
            [Test]
            public void RaisesEventWhileClearing()
            {
                bool wasRaised = false;
                var observableDictionary = new DispatcherObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => wasRaised = args.Action == NotifyCollectionChangedAction.Reset;

                observableDictionary.Clear();

                Assert.IsTrue(wasRaised && observableDictionary.Count == 0);
            }
        }

        [TestFixture]
        public class TheContainsMethod
        {
            [Test]
            public void ContainsKvpSuccess()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains(new KeyValuePair<int, int>(1, 2));

                Assert.IsTrue(result);
            }

            [Test]
            public void ContainsObjectTrue()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)1);

                Assert.IsTrue(result);
            }

            [Test]
            public void ContainsObjectFalse()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)2);

                Assert.IsFalse(result);
            }

            [Test]
            public void ContainsObjectInvalidTypeFalse()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 2
                    }
                };

                var result = observableDictionary.Contains((object)"1");

                Assert.IsFalse(result);
            }
        }

        [TestFixture]
        public class TheContainsKeyMethod
        {
            [Test]
            public void ContainsKeyTrue()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.ContainsKey(1);

                Assert.IsTrue(success);
            }

            [Test]
            public void ContainsKeyFalse()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.ContainsKey(2);

                Assert.IsFalse(success);
            }
        }

        [TestFixture]
        public class TheCopyToMethod
        {
            [Test]
            public void PopulatesArray()
            {
                Array arr = new KeyValuePair<int, int>[2];

                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    },
                    {
                        2, 2
                    }
                };

                observableDictionary.CopyTo(arr, 0);

                Assert.IsTrue(arr.Length == 2);
            }
            [Test]
            public void PopulatesKvpArray()
            {
                KeyValuePair<int, int>[] arr = new KeyValuePair<int, int>[2];

                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    },
                    {
                        2, 2
                    }
                };

                observableDictionary.CopyTo(arr, 0);

                Assert.IsTrue(arr.Length == 2);
            }
        }

        [TestFixture]
        public class TheIndexer
        {
            [Test]
            public void ThrowsInvalidCastExceptionForAddingObject()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                Assert.That(() => observableDictionary[(object)"1"] = 1, Throws.TypeOf<InvalidCastException>());
            }

            [Test]
            public void RaisesEventWhileAddingObject()
            {
                var counter = 0;
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary[(object)1] = 1;
                Assert.AreEqual(1, counter);

                observableDictionary[(object)2] = 2;
                Assert.AreEqual(2, counter);

                observableDictionary[(object)3] = 3;
                Assert.AreEqual(3, counter);

                observableDictionary[(object)4] = 4;
                Assert.AreEqual(4, counter);
            }

            [Test]
            public void RaisesEventWhileAddingStronglyTyped()
            {
                var counter = 0;
                var observableDictionary = new DispatcherObservableDictionary<int, int>();

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter++;

                observableDictionary[1] = 1;
                Assert.AreEqual(1, counter);

                observableDictionary[2] = 2;
                Assert.AreEqual(2, counter);

                observableDictionary[3] = 3;
                Assert.AreEqual(3, counter);

                observableDictionary[4] = 4;
                Assert.AreEqual(4, counter);
            }

            [Test]
            public void RaisesEventWhileUpdatingObject()
            {
                var isUpdated = false;
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => isUpdated = args.Action == NotifyCollectionChangedAction.Replace;

                observableDictionary[(object)1] = 3;

                Assert.IsTrue(isUpdated);
            }

            [Test]
            public void RaisesEventWhileUpdatingStronglyTyped()
            {
                var isUpdated = false;
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => isUpdated = args.Action == NotifyCollectionChangedAction.Replace;

                observableDictionary[1] = 3;

                Assert.IsTrue(isUpdated);
            }

            [Test]
            public void ReturnsNullForInvalidTypedObject()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>()
                {
                    {
                        1, 1
                    }
                };

                var result = observableDictionary[(object)"1"];

                Assert.IsNull(result);
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [Test]
            public void RaiseEventWhileRemovingStronglyTyped()
            {
                var counter = 1;
                var observableDictionary = new DispatcherObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                observableDictionary.AutomaticallyDispatchChangeNotifications = false;
                observableDictionary.CollectionChanged += (sender, args) => counter--;

                observableDictionary.Remove(1);
                Assert.AreEqual(0, counter);
            }
        }

        [TestFixture]
        public class TheTryGetValueMethod
        {
            [Test]
            public void ReturnsValueFromValidKey()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.TryGetValue(1, out int value);

                Assert.IsTrue(success && value == 1);
            }

            [Test]
            public void ReturnsDefaultFromInvalidKey()
            {
                var observableDictionary = new DispatcherObservableDictionary<int, int>
                {
                    {
                        1, 1
                    }
                };

                var success = observableDictionary.TryGetValue(2, out int value);

                Assert.IsTrue(!success && value == 0);
            }
        }
    }
}
