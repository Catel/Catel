﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedRepositoryBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Extensions.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Catel.Data;
    using Catel.Data.Repositories;

    using NUnit.Framework;

#if NETFX_CORE
    using System.Threading.Tasks;
#endif

    public class CachedRepositoryBaseFacts
    {
        public class TestCachedRepository : CachedRepositoryBase<TestModel>
        {
            protected override void RetrieveData(Action<IEnumerable<TestModel>> completed)
            {
                var list = new List<TestModel>();
                for (int i = 0; i < 10; i++)
                {
                    list.Add(new TestModel { Name = string.Format("Name {0}", i + 1) });
                }

                // Fake async stuff
#if NETFX_CORE
                var task = Task.Delay(250);
                task.ContinueWith((t, o) => completed(list), null);
                task.Start();
#else
                var thread = new Thread(() =>
                {
                    ThreadHelper.Sleep(250);
                    completed(list);
                });
                thread.Start();
#endif
            }
        }

        public class TestModel : ModelBase
        {
            #region Constants
            /// <summary>
            /// Register the Name property so it is known in the class.
            /// </summary>
            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof (string), null);
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }
            #endregion
        }

        [TestFixture]
        public class TheDataLoadedTimestampProperty
        {
            [TestCase]
            public void IsValidWhenDataIsLoaded()
            {
                var completion = new ManualResetEvent(false);
                DateTime dateTime = DateTime.MinValue;

                var repository = new TestCachedRepository();
                repository.GetData(items => dateTime = DateTime.Now);

                completion.WaitOne(1000);

                // Don't check for milliseconds, they can differ
                Assert.AreEqual(dateTime.Hour, repository.DataLoadedTimestamp.Hour);
                Assert.AreEqual(dateTime.Minute, repository.DataLoadedTimestamp.Minute);
                Assert.AreEqual(dateTime.Second, repository.DataLoadedTimestamp.Second);
            }
        }

        [TestFixture]
        public class TheDataProperty
        {
            [TestCase]
            public void IsNotNullWhenDataIsLoaded()
            {
                var completion = new ManualResetEvent(false);

                var repository = new TestCachedRepository();
                repository.GetData();

                completion.WaitOne(1000);

                Assert.IsNotNull(repository.Data);
            }
        }

        [TestFixture]
        public class TheExpirationProperty
        {
            [TestCase]
            public void ExpiresTheLoadedDataCorrectly()
            {
                var completion = new ManualResetEvent(false);

                var repository = new TestCachedRepository();
                repository.Expiration = new TimeSpan(0, 0, 2);
                repository.GetData();

                completion.WaitOne(1000);

                Assert.IsTrue(repository.IsDataLoaded);
                Assert.IsNotNull(repository.Data);

                completion = new ManualResetEvent(false);
                completion.WaitOne(3000);

                Assert.IsFalse(repository.IsDataLoaded);
                Assert.IsNull(repository.Data);
            }
        }

        [TestFixture]
        public class TheGetDataMethod
        {
            [TestCase]
            public void CallsCompletedAfterDataLoading()
            {
                var completion = new ManualResetEvent(false);
                var completedCallCount = 0;

                var repository = new TestCachedRepository();
                repository.GetData(items => completedCallCount++);

                completion.WaitOne(1000);

                Assert.AreEqual(1, completedCallCount);
            }

            [TestCase]
            public void QueuesCompletedDuringDataLoading()
            {
                var completion = new ManualResetEvent(false);
                var completedCallCount = 0;

                var repository = new TestCachedRepository();
                repository.GetData(items => { completedCallCount++; });
                repository.GetData(items => { completedCallCount++; });
                repository.GetData(items =>
                {
                    completedCallCount++;
                    completion.Set();
                });

                completion.WaitOne(1000);

                Assert.AreEqual(3, completedCallCount);
            }
        }

        [TestFixture]
        public class TheIsDataLoadedProperty
        {
            [TestCase]
            public void IsSetToTrueAfterDataHasBeenLoaded()
            {
                var completion = new ManualResetEvent(false);
                var completedCallCount = 0;

                var repository = new TestCachedRepository();
                repository.GetData(items => completedCallCount++);

                Assert.IsFalse(repository.IsDataLoaded);

                completion.WaitOne(1000);

                Assert.IsTrue(repository.IsDataLoaded);
            }
        }

        [TestFixture]
        public class TheIsLoadingDataProperty
        {
            [TestCase]
            public void IsToggledDuringDataLoading()
            {
                var completion = new ManualResetEvent(false);
                var completedCallCount = 0;

                var repository = new TestCachedRepository();

                Assert.IsFalse(repository.IsLoadingData);

                repository.GetData(items => completedCallCount++);

                Assert.IsTrue(repository.IsLoadingData);

                completion.WaitOne(1000);

                Assert.IsFalse(repository.IsLoadingData);
            }
        }
    }
}