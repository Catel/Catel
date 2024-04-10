namespace Catel.Tests.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;

    using NUnit.Framework;

    public class CacheStorageFacts
    {
        [TestFixture]
        public class The_ThreadSafe_Functionality
        {
            private static readonly ILog Log = LogManager.GetCurrentClassLogger();
            private readonly List<Guid> _randomGuids = new List<Guid>();

            public The_ThreadSafe_Functionality()
            {
                for (var i = 0; i < 10; i++)
                {
                    _randomGuids.Add(Guid.NewGuid());
                }
            }

            [TestCase]
            public void Run_Multiple_Threads_Using_GetFromCacheOrFetch()
            {
                RunMultipleThreadsWithRandomAccessCalls((cache, key) =>
                {
                    return cache.GetFromCacheOrFetch(key, () =>
                    {
                        var threadId = ThreadHelper.GetCurrentThreadId();
                        //Log.Info("Key '{0}' is now controlled by thread '{1}'", key, threadId);
                        return threadId;
                    });
                });
            }

            [TestCase]
            public void Run_Multiple_Threads_Using_Add_And_Get()
            {
                RunMultipleThreadsWithRandomAccessCalls((cache, key) =>
                {
                    var threadId = ThreadHelper.GetCurrentThreadId();

                    cache.Add(key, threadId);

                    //Log.Info("Key '{0}' is now controlled by thread '{1}'", key, threadId);

                    return cache.Get(key);
                });
            }

            private void RunMultipleThreadsWithRandomAccessCalls(Func<ICacheStorage<Guid, int>, Guid, int> retrievalFunc)
            {
                var cacheStorage = new CacheStorage<Guid, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(250)));

                var threads = new List<Thread>();
                for (var i = 0; i < 50; i++)
                {
                    var thread = new Thread(() =>
                    {
                        var random = new Random();

                        for (var j = 0; j < 1000; j++)
                        {
                            var randomGuid = _randomGuids[random.Next(0, 9)];

                            retrievalFunc(cacheStorage, randomGuid);

                            ThreadHelper.Sleep(10);
                        }
                    });

                    threads.Add(thread);
                    thread.Start();
                }

                while (true)
                {
                    var anyThreadAlive = false;

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

        [TestFixture]
        public class The_Indexer_Property
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var value = cache[null];
                    Assert.That(value, Is.Null);
                });
            }

            [TestCase]
            public void Returns_Right_Value_For_Existing_Key()
            {
                var cache = new CacheStorage<string, object>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache["2"], Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class The_Get_Method
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() =>
                {
                    var value = cache.Get(null);
                    Assert.That(value, Is.Null);
                });
            }

            [TestCase]
            public void Returns_Right_Value_For_Existing_Key()
            {
                var cache = new CacheStorage<string, object>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Get("2"), Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class The_Contains_Method
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() => cache.Contains(null));
            }

            [TestCase]
            public void Returns_False_For_Non_Existing_Key()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Contains("3"), Is.False);
            }

            [TestCase]
            public void Returns_True_For_Existing_Key()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("2", 2);

                Assert.That(cache.Contains("2"), Is.True);
            }
        }

        [TestFixture]
        public class The_GetFromCacheOrFetch_Method
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.GetFromCacheOrFetch(null, () => 1));
            }

            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Function()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.GetFromCacheOrFetch("1", null));
            }

            [TestCase]
            public void Adds_Item_To_Cache_And_Returns_It()
            {
                var cache = new CacheStorage<string, int>();

                var value = cache.GetFromCacheOrFetch("1", () => 1);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(1));
                Assert.That(value, Is.EqualTo(1));
            }

            [TestCase]
            public void Returns_Cached_Item()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(1));
                Assert.That(value, Is.EqualTo(1));
            }

            [TestCase]
            public void Adds_Item_To_Cache_With_Override_And_Returns_It()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);
                var value = cache.GetFromCacheOrFetch("1", () => 2, true);

                Assert.That(cache.Contains("1"), Is.True);
                Assert.That(cache["1"], Is.EqualTo(2));
                Assert.That(value, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class The_Add_Method
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.Add(null, 1));
            }

            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Value_If_Not_Allow_Null_Values()
            {
                var cache = new CacheStorage<string, object>();

                Assert.Throws<ArgumentNullException>(() => cache.Add(null, null));
            }

            [TestCase]
            public void Adds_Non_Existing_Value()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.That(cache["1"], Is.EqualTo(1));
            }

            [TestCase]
            public void Adds_Non_Existing_Value_For_True_Override()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, true);

                Assert.That(cache["1"], Is.EqualTo(2));
            }

            [TestCase]
            public void Does_Not_Add_Existing_Value_For_False_Override()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Add("1", 2, false);

                Assert.That(cache["1"], Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class The_Remove_Method
        {
            [TestCase]
            public void Throws_ArgumentNullException_For_Null_Key()
            {
                var cache = new CacheStorage<string, int>();

                Assert.Throws<ArgumentNullException>(() => cache.Remove(null));
            }

            [TestCase]
            public void Removes_Existing_Value()
            {
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);

                Assert.That(cache.Contains("1"), Is.True);

                cache.Remove("1");

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void Removes_Non_Existing_Value()
            {
                var cache = new CacheStorage<string, int>();
                cache.Remove("1");
            }

            [TestCase]
            public void Does_Not_Raise_Expiring_Event_On_Item_Removal()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expiring += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }

            [TestCase]
            public void Does_Not_Raise_Expired_Event_On_Item_Removal()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expired += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class The_Clear_Method
        {
            [TestCase]
            public void Does_Not_Raise_Expiring_Event_On_Clear_Storage()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expiring += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }

            [TestCase]
            public void Does_Not_Raise_Expired_Event_On_Clear_Storage()
            {
                var counter = 0;
                var cache = new CacheStorage<string, int>();
                cache.Add("1", 1);
                cache.Expired += (sender, e) => counter++;

                Assert.That(counter, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class The_AutoExpire_Functionality
        {
            [TestCase]
            public void Is_Automatically_Enabled_When_Started_Disabled_But_Adding_Item_With_Custom_ExpirationPolicy()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void Automatically_Removes_Expired_Items()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1, expiration: new TimeSpan(0, 0, 0, 0, 250));

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void Automatically_Removes_Expired_Items_Of_A_CacheStorage_With_Default_ExpirationPolicy_Initialization_Code()
            {
                var cache = new CacheStorage<string, int>(() => ExpirationPolicy.Duration(TimeSpan.FromMilliseconds(250)));
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                cache.Add("1", 1);

                Assert.That(cache.Contains("1"), Is.True);

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains("1"), Is.False);
            }

            [TestCase]
            public void Adds_And_Expires_Several_Items()
            {
                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                for (int i = 0; i < 5; i++)
                {
                    ThreadHelper.Sleep(1000);

                    int innerI = i;
                    var value = cache.GetFromCacheOrFetch("key", () => innerI, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(value, Is.EqualTo(i));
                }
            }

            [TestCase]
            public void Raises_Expiring_Event_With_Correct_EventArgs_When_Item_Expires()
            {
                var key = "1";
                var expirationPolicy = new SlidingExpirationPolicy(TimeSpan.FromMilliseconds(250));
                var value = 1;
                var evKey = (string)null;
                var evExpirationPolicy = default(ExpirationPolicy);
                var evValue = 0;

                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expiring += (sender, e) =>
                {
                    evKey = e.Key;
                    evExpirationPolicy = e.ExpirationPolicy;
                    evValue = e.Value;
                };

                cache.Add(key, value, expirationPolicy);

                ThreadHelper.Sleep(750);

                Assert.That(evKey, Is.EqualTo(key));
                Assert.That(evExpirationPolicy, Is.EqualTo(expirationPolicy));
                Assert.That(evValue, Is.EqualTo(value));
            }

            [TestCase]
            public void Item_Stays_In_Cache_When_Expiring_Event_Is_Canceled()
            {
                var key = "1";
                var value = 1;

                var cache = new CacheStorage<string, int>();
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expiring += (sender, e) =>
                {
                    e.Cancel = true;
                };

                cache.Add(key, value, expiration: new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(750);

                Assert.That(cache.Contains(key), Is.True);
            }

            [TestCase]
            public void Raises_Expired_Event_With_Correct_EventArgs_When_Item_Expires()
            {
                var dispose = true;
                var key = "1";
                var value = 1;
                var evDispose = false;
                var evKey = (string)null;
                var evValue = 0;

                var cache = new CacheStorage<string, int>();
                cache.DisposeValuesOnRemoval = dispose;
                cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                cache.Expired += (sender, e) =>
                {
                    evDispose = e.Dispose;
                    evKey = e.Key;
                    evValue = e.Value;
                };

                cache.Add(key, value, expiration: new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(750);

                Assert.That(evDispose, Is.EqualTo(dispose));
                Assert.That(evKey, Is.EqualTo(key));
                Assert.That(evValue, Is.EqualTo(value));
            }

            [TestCase]
            public async Task Is_Disabled_By_Default_Async()
            {
                var cache = new CacheStorage<string, int>();

                cache.Add("1", 1);

                Assert.That(cache.Contains("1"), Is.True);

                await Task.Delay(TimeSpan.FromSeconds(2));

                Assert.That(cache.Contains("1"), Is.True);
            }
        }

        [TestFixture]
        public class The_DisposeItemsOnRemoval_Functionality
        {
            private sealed class CustomDisposable : IDisposable
            {
                public CustomDisposable()
                {
                    IsDisposed = false;
                }

                public bool IsDisposed { get; private set; }

                public void Dispose()
                {
                    IsDisposed = true;
                }
            }

            [TestCase]
            public void Disposes_Expired_Items_When_Disposing_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDisposed, Is.True);
                }
            }

            [TestCase]
            public void Disposes_Item_On_Remove_When_Disposing_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    cache.Remove("disposable");

                    Assert.That(disposable.IsDisposed, Is.True);
                }
            }

            [TestCase]
            public void Disposes_Items_On_Clear_When_Disposing_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    cache.Clear();

                    Assert.That(disposable.IsDisposed, Is.True);
                }
            }

            [TestCase]
            public void Does_Not_Dispose_Expired_Item_When_Disposing_Enabled_But_Canceled_By_EventArgs()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.DisposeValuesOnRemoval = true;
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                    cache.Expired += (sender, e) =>
                    {
                        e.Dispose = false;
                    };

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDisposed, Is.False);
                }
            }

            [TestCase]
            public void Does_Not_Dispose_Expired_Item_When_Disposing_Not_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDisposed, Is.False);
                }
            }

            [TestCase]
            public void Does_Not_Dispose_Item_On_Remove_When_Disposing_Not_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    cache.Remove("disposable");

                    Assert.That(disposable.IsDisposed, Is.False);
                }
            }

            [TestCase]
            public void Does_Not_Dispose_Items_On_Clear_When_Disposing_Not_Enabled()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);

                    cache.Clear();

                    Assert.That(disposable.IsDisposed, Is.False);
                }
            }

            [TestCase]
            public void Disposes_Expired_Item_When_Disposing_Not_Enabled_But_Forced_By_EventArgs()
            {
                using (var disposable = new CustomDisposable())
                {
                    var cache = new CacheStorage<string, CustomDisposable>();
                    cache.ExpirationTimerInterval = TimeSpan.FromMilliseconds(250);
                    cache.Expired += (sender, e) =>
                    {
                        e.Dispose = true;
                    };

                    cache.Add("disposable", disposable, expiration: TimeSpan.FromMilliseconds(250));

                    ThreadHelper.Sleep(750);

                    Assert.That(disposable.IsDisposed, Is.True);
                }
            }
        }
    }
}
