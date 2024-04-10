namespace Catel.Tests.Caching
{
    using System;
    using Catel.Caching;
    using Catel.Caching.Policies;

    using NUnit.Framework;

    public class CacheStorageValueInfoFacts
    {
        [TestFixture]
        public class The_CanExpire_Property
        {
            [TestCase]
            public void Returns_True_When_TimeSpan_Is_Not_Zero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 5));

                Assert.That(valueInfo.CanExpire, Is.True);
            }
        }

        [TestFixture]
        public class The_IsExpired_Property
        {
            [TestCase]
            public void Returns_False_When_TimeSpan_Is_Zero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0));

                Assert.That(valueInfo.IsExpired, Is.False);
            }

            [TestCase]
            public void Returns_False_When_TimeSpan_Is_Not_Zero_But_Value_Is_Not_Expired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 2));

                Assert.That(valueInfo.IsExpired, Is.False);
            }

            [TestCase]
            public void Returns_False_While_The_Value_Is_Read_And_Sliding_Policy_Is_Used()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 1)));
                var startTime = FastDateTime.Now;
                do
                {
#pragma warning disable 168
                    var value = valueInfo.Value;
#pragma warning restore 168
                    Assert.That(valueInfo.IsExpired, Is.False);
                }
                while (FastDateTime.Now.Subtract(startTime).TotalSeconds < 3);
            }

            [TestCase]
            public void Returns_True_When_The_Value_Is_Not_Read_And_Sliding_Policy_Is_Used()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 0, 0, 250)));

                ThreadHelper.Sleep(500);

                Assert.That(valueInfo.IsExpired, Is.True);
            }

            [TestCase]
            public void Returns_True_When_TimeSpan_Is_Not_Zero_And_Value_Is_Expired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(500);

                Assert.That(valueInfo.IsExpired, Is.True);
            }
        }

        [TestFixture]
        public class The_DisposeValue_Method
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
            public void Value_Is_Not_Disposed_Before_Call()
            {
                using (var disposable = new CustomDisposable())
                {
                    var valueInfo = new CacheStorageValueInfo<CustomDisposable>(disposable, TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDisposed, Is.False);
                }
            }

            [TestCase]
            public void Value_Is_Disposed_After_Call()
            {
                using (var disposable = new CustomDisposable())
                {
                    var valueInfo = new CacheStorageValueInfo<CustomDisposable>(disposable, TimeSpan.FromMilliseconds(250));

                    valueInfo.DisposeValue();

                    Assert.That(disposable.IsDisposed, Is.True);
                }
            }
        }
    }
}
