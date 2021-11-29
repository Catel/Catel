// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorageValueInfoFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Caching
{
    using System;
    using Catel.Caching;
    using Catel.Caching.Policies;

    using NUnit.Framework;

    public class CacheStorageValueInfoFacts
    {
        [TestFixture]
        public class TheCanExpireProperty
        {
            [TestCase]
            public void ReturnsFalseWhenTimeSpanIsZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0));

                Assert.IsFalse(valueInfo.CanExpire);
            }

            [TestCase]
            public void ReturnsTrueWhenTimeSpanIsNotZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 5));

                Assert.IsTrue(valueInfo.CanExpire);
            }
        }

        [TestFixture]
        public class TheIsExpiredProperty
        {
            [TestCase]
            public void ReturnsFalseWhenTimeSpanIsZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0));

                Assert.IsFalse(valueInfo.IsExpired);
            }

            [TestCase]
            public void ReturnsFalseWhenTimeSpanIsNotZeroButValueIsNotExpired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 2));

                Assert.IsFalse(valueInfo.IsExpired);
            }

            [TestCase]
            public void ReturnsFalseWhileTheValueIsReadAndSlidingPolicyIsUsed()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 1)));
                var startTime = FastDateTime.Now;
                do
                {
#pragma warning disable 168
                    var value = valueInfo.Value;
#pragma warning restore 168
                    Assert.IsFalse(valueInfo.IsExpired);
                }
                while (FastDateTime.Now.Subtract(startTime).TotalSeconds < 3);
            }

            [TestCase]
            public void ReturnsTrueWhenTheValueIsNotReadAndSlidingPolicyIsUsed()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 0, 0, 250)));

                ThreadHelper.Sleep(500);

                Assert.IsTrue(valueInfo.IsExpired);
            }

            [TestCase]
            public void ReturnsTrueWhenTimeSpanIsNotZeroAndValueIsExpired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(500);

                Assert.IsTrue(valueInfo.IsExpired);
            }
        }

        [TestFixture]
        public class TheDisposeValueMethod
        {
            private sealed class CustomDisposable : IDisposable
            {
                public CustomDisposable()
                {
                    IsDiposed = false;
                }

                public bool IsDiposed { get; private set; }

                public void Dispose()
                {
                    IsDiposed = true;
                }
            }

            [TestCase]
            public void ValueIsNotDisposedBeforeCall()
            {
                using (var disposable = new CustomDisposable())
                {
                    var valueInfo = new CacheStorageValueInfo<CustomDisposable>(disposable, TimeSpan.FromMilliseconds(250));

                    Assert.That(disposable.IsDiposed, Is.False);
                }
            }

            [TestCase]
            public void ValueIsDisposedAfterCall()
            {
                using (var disposable = new CustomDisposable())
                {
                    var valueInfo = new CacheStorageValueInfo<CustomDisposable>(disposable, TimeSpan.FromMilliseconds(250));

                    valueInfo.DisposeValue();

                    Assert.That(disposable.IsDiposed, Is.True);
                }
            }
        }
    }
}
