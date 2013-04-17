// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorageFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Caching
{
    using System;
    using Catel.Caching;
    using Catel.Caching.Policies;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CacheStorageFacts
    {
        [TestClass]
        public class TheIndexerProperty
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() =>
                {
                    var value = cache[null];
                    Assert.IsNull(value);
                });
            }

            [TestMethod]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.AreEqual(2, cache["2"]);
            }
        }

        [TestClass]
        public class TheGetMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() =>
                {
                    var value = cache.Get(null);
                    Assert.IsNull(value);
                });
            }

            [TestMethod]
            public void ReturnsRightValueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.AreEqual(2, cache.Get("2"));
            }
        }

        [TestClass]
        public class TheContainsMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Contains(null));
            }

            [TestMethod]
            public void ReturnsFalseForNonExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.IsFalse(cache.Contains("3"));
            }

            [TestMethod]
            public void ReturnsTrueForExistingKey()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.IsTrue(cache.Contains("2"));
            }
        }

        [TestClass]
        public class TheGetFromCacheOrFetchMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.GetFromCacheOrFetch(null, () => 1));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullFunction()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.GetFromCacheOrFetch("1", null));
            }
            
            /*
			[TestMethod]
            public void ThrowsArgumentExceptionForNullFunctionValueIfNotAllowNullValues()
            {
                var cache = new CacheStorage<string, object>();
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => cache.GetFromCacheOrFetch("1", () => null));
            }
            */

            [TestMethod]
            public void AddsItemToCacheAndReturnsIt()
            {
                var cache = new CacheStorage<string, int>();

                var value = cache.GetFromCacheOrFetch("1", () => 1);

                Assert.IsTrue(cache.Contains("1"));                
                Assert.AreEqual(1, cache["1"]);
                Assert.AreEqual(1, value);
            }

            [TestMethod]
            public void ReturnsCachedItem()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2);

                Assert.IsTrue(cache.Contains("1"));
                Assert.AreEqual(1, cache["1"]);
                Assert.AreEqual(1, value);
            }

            [TestMethod]
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

        [TestClass]
        public class TheAddMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Add(null, 1));
            }           
            
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValueIfNotAllowNullValues()
            {
                var cache = new CacheStorage<string, object>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Add(null, null));
            }

            [TestMethod]
            public void AddsNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.AreEqual(1, cache["1"]);
            }

            [TestMethod]
            public void AddsNonExistingValueForTrueOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, true);

                Assert.AreEqual(2, cache["1"]);
            }

            [TestMethod]
            public void DoesNotAddExistingValueForFalseOverride()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, false);

                Assert.AreEqual(1, cache["1"]);
            }
        }

        [TestClass]
        public class TheRemoveMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullKey()
            {
                var cache = new CacheStorage<string, int>();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => cache.Remove(null));
            }

            [TestMethod]
            public void RemovesExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.IsTrue(cache.Contains("1"));

                cache.Remove("1");

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestMethod]
            public void RemovesNonExistingValue()
            {
                var cache = new CacheStorage<string, int>();
                cache.Remove("1");
            }
        }

        [TestClass]
        public class TheAutoExpireFunctionality
        {
            [TestMethod]
            public void AutomaticallyRemovesExpiredItems()
            {
                var cache = new CacheStorage<string, int>();
     			
     			cache.Add("1", 1, expiration: new TimeSpan(0, 0, 1));

				Assert.IsTrue(cache.Contains("1"));

				ThreadHelper.Sleep(1500);

				Assert.IsFalse(cache.Contains("1"));
			}

            [TestMethod]
            public void AutomaticallyRemovesExpiredItemsOfACacheStorageWithDefaultExpirationPolicyInitializationCode()
            {
                var cache = new CacheStorage<string, int>(() => ExpirationPolicy.Duration(TimeSpan.FromSeconds(3)));

                cache.Add("1", 1);

                Assert.IsTrue(cache.Contains("1"));

                ThreadHelper.Sleep(5500);

                Assert.IsFalse(cache.Contains("1"));
            }

            [TestMethod]
            public void AddsAndExpiresSeveralItems()
            {
                var cache = new CacheStorage<string, int>();

                for (int i = 0; i < 5; i++)
                {
                    var value = cache.GetFromCacheOrFetch("key", () => i, expiration: new TimeSpan(0, 0, 1));

                    Assert.AreEqual(i, value);

                    ThreadHelper.Sleep(2000);
                }
            }
        }
    }
}