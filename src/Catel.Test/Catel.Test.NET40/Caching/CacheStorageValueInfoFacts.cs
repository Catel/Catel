// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheStorageValueInfoFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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

    public class CacheStorageValueInfoFacts
    {
        [TestClass]
        public class TheCanExpireProperty
        {
            [TestMethod]
            public void ReturnsFalseWhenTimeSpanIsZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0));    

                Assert.IsFalse(valueInfo.CanExpire);
            }

            [TestMethod]
            public void ReturnsTrueWhenTimeSpanIsNotZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 5));

                Assert.IsTrue(valueInfo.CanExpire);
            }
        }

        [TestClass]
        public class TheIsExpiredProperty
        {
            [TestMethod]
            public void ReturnsFalseWhenTimeSpanIsZero()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0));

                Assert.IsFalse(valueInfo.IsExpired);
            }

            [TestMethod]
            public void ReturnsFalseWhenTimeSpanIsNotZeroButValueIsNotExpired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 2));

                Assert.IsFalse(valueInfo.IsExpired);
            }

            [TestMethod]
            public void ReturnsFalseWhileTheValueIsReadAndSlidingPolicyIsUsed()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 2)));
                DateTime startTime = DateTime.Now;
                do
                {
#pragma warning disable 168
                    int value = valueInfo.Value;
#pragma warning restore 168
                    Assert.IsFalse(valueInfo.IsExpired);
                }
                while (DateTime.Now.Subtract(startTime).TotalSeconds < 10);
            }

            [TestMethod]
            public void ReturnsTrueWhenTheValueIsNotReadAndSlidingPolicyIsUsed()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new SlidingExpirationPolicy(new TimeSpan(0, 0, 2)));

                ThreadHelper.Sleep(2500);

                Assert.IsTrue(valueInfo.IsExpired);
            }

            [TestMethod]
            public void ReturnsTrueWhenTimeSpanIsNotZeroAndValueIsExpired()
            {
                var valueInfo = new CacheStorageValueInfo<int>(0, new TimeSpan(0, 0, 2));

                ThreadHelper.Sleep(2500);

                Assert.IsTrue(valueInfo.IsExpired);
            }
        }
    }
}