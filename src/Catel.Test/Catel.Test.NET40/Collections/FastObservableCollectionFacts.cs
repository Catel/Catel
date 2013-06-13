// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollectionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Collections
{
    using System;
    using Catel.Collections;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class FastObservableCollectionFacts
    {
        [TestClass]
        public class TheAddRangeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.AddItems(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheRemoveRangeMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCollection()
            {
                var fastCollection = new FastObservableCollection<int>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => fastCollection.RemoveItems(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheSuspendNotificationsMethod
        {
            [TestMethod]
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
    }
}