// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorageFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;

    using NUnit.Framework;

    public class CacheStorageFacts
    {
#if NET
        [TestFixture]
        public class TheThreadSafeFunctionality
        {
            private static readonly ILog Log = LogManager.GetCurrentClassLogger();
            private readonly List<Guid> _randomGuids = new List<Guid>();

            public TheThreadSafeFunctionality()
            {
                for (int i = 0; i < 10; i++)
                {
                    _randomGuids.Add(Guid.NewGuid());
                }
            }

            [TestCase]
            public void RunMultipleThreadsWithRandomAccessCalls()
            {
                var cacheStorage = new CacheStorage<Guid, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(500)));

                var threads = new List<Thread>();
                for (int i = 0; i < 25; i++)
                {
                    var thread = new Thread(() =>
                    {
                        var random = new Random();

                        for (int j = 0; j < 1000; j++)
                        {
                            var randomGuid = _randomGuids[random.Next(0, 9)];
                            cacheStorage.GetFromCacheOrFetch(randomGuid, () =>
                            {
                                var threadId = Thread.CurrentThread.ManagedThreadId;
                                Log.Info("Key '{0}' is now controlled by thread '{1}'", randomGuid, threadId);
                                return threadId;
                            });

                            ThreadHelper.Sleep(10);
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                while (true)
                {
                    bool anyThreadAlive = false;

                    foreach (var thread in threads)
                    {
                        if (thread.IsAlive)
                        {
                            anyThreadAlive = true;
                            break;
                        }
                    }

                    if (!anyThreadAlive)
                    {
                        break;
                    }

                    ThreadHelper.Sleep(500);
                }
            }
        }
#endif

        [TestFixture]
        public class TheIndexerProperty
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() =>
                {
                    var value = cache[null];
                    Assert.IsNull(value);
                });
            }

            [TestCase]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.AreEqual(2, cache["2"]);
            }
        }

        [TestFixture]
        public class TheGetMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() =>
                {
                    var value = cache.Get(null);
                    Assert.IsNull(value);
                });
            }

            [TestCase]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.AreEqual(2, cache.Get("2"));
            }
        }

        [TestFixture]
        public class TheContainsMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Contains(null));
            }

            [TestCase]
            public void ReturnsFalseForNonExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.IsFalse(cache.Contains("3"));
            }

            [TestCase]
            public void ReturnsTrueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.IsTrue(cache.Contains("2"));
            }
        }

        [TestFixture]
        public class TheGetFromCacheOrFetchMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.GetFromCacheOrFetch(null, () => 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullFunction()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.GetFromCacheOrFetch("1", null));
            }

            [TestCase]
            public void AddsItemToCacheAndReturnsIt()
            {
                var cache = new CacheStorage<string, int>();

                var value = cache.GetFromCacheOrFetch("1", () => 1);

                Assert.IsTrue(cache.Contains("1"));
                Assert.AreEqual(1, cache["1"]);
                Assert.AreEqual(1, value);
            }

            [TestCase]
            public void ReturnsCachedItem()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2);

                Assert.IsTrue(cache.Contains("1"));
                Assert.AreEqual(1, cache["1"]);
                Assert.AreEqual(1, value);
            }

            [TestCase]
            public void AddsItemToCacheWithOverrideAndReturnsIt()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2, true);

                Assert.IsTrue(cache.Contains("1"));
                Assert.AreEqual(2, cache["1"]);
                Assert.AreEqual(2, value);
            }
        }

        [TestFixture]
        public class TheAddMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Add(null, 1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValueIfNotAllowNullValues()
            {
                var cache = new CacheStorage<string, object>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Add(null, null));
            }

            [TestCase]
            public void AddsNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.AreEqual(1, cache["1"]);
            }

            [TestCase]
            public void AddsNonExistingValueForTrueOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, true);

                Assert.AreEqual(2, cache["1"]);
            }

            [TestCase]
            public void DoesNotAddExistingValueForFalseOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, false);

                Assert.AreEqual(1, cache["1"]);
            }
        }

        [TestFixture]
        public class TheRemoveMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Remove(null));
            }

            [TestCase]
            public void RemovesExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.IsTrue(cache.Contains("1"));

                cache.Remove("1");

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestCase]
            public void RemovesNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Remove("1");
            }
        }

        [TestFixture]
        public class TheAutoExpireFunctionality
        {
            [TestCase]
            public void IsAutomaticallyEnabledWhenStartedDisabledButAddingItemWithCustomExpirationPolicy()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.IsTrue(cache.Contains("1"));

                ThreadHelper.Sleep(750);

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestCase]
            public void AutomaticallyRemovesExpiredItems()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.IsTrue(cache.Contains("1"));

                ThreadHelper.Sleep(750);

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestCase]
            public void AutomaticallyRemovesExpiredItemsOfACacheStorageWithDefaultExpirationPolicyInitializationCode()
            {
                var cache = new CacheStorage<string, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(250)));
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1);

                Assert.IsTrue(cache.Contains("1"));

                ThreadHelper.Sleep(750);

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestCase]
            public void AddsAndExpiresSeveralItems()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                for (int i = 0; i < 5; i++)
                {
                    ThreadHelper.Sleep(1000);

                    int innerI = i;
                    var value = cache.GetFromCacheOrFetch("key", () => innerI, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.AreEqual(i, value);
                }
            }
        }
    }
}