// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Pooling
{
    using Catel.Pooling;
    using NUnit.Framework;

    [TestFixture]
    public class PoolManagerFacts
    {
        [TestCase]
        public void ReusesPooledObjects()
        {
            const int BufferSize = 4096;

            var poolManager = new PoolManager<Buffer4096Poolable>();

            Assert.AreEqual(0, poolManager.Count);
            Assert.AreEqual(0, poolManager.CurrentSize);

            var poolable1 = poolManager.GetObject();
            var poolable2 = poolManager.GetObject();

            poolable1.Dispose();

            Assert.AreEqual(1, poolManager.Count);
            Assert.AreEqual(BufferSize * 1, poolManager.CurrentSize);

            poolable2.Dispose();

            Assert.AreEqual(2, poolManager.Count);
            Assert.AreEqual(BufferSize * 2, poolManager.CurrentSize);

            var poolable3 = poolManager.GetObject();

            Assert.AreEqual(1, poolManager.Count);
            Assert.AreEqual(BufferSize * 1, poolManager.CurrentSize);

            // Compare to poolable 2 because it's a stack which is filo (first in / last out)
            Assert.IsTrue(ReferenceEquals(poolable2, poolable3));
        }

        [TestCase]
        public void DoesNotReturnObjectsWhenPoolManagerGetsTooLarge()
        {
            const int BufferSize = 4096;

            var poolManager = new PoolManager<Buffer4096Poolable>();
            poolManager.MaxSize = BufferSize * 3;

            var poolable1 = poolManager.GetObject();
            var poolable2 = poolManager.GetObject();
            var poolable3 = poolManager.GetObject();
            var poolable4 = poolManager.GetObject();

            poolable1.Dispose();
            poolable2.Dispose();
            poolable3.Dispose();
            poolable4.Dispose();

            Assert.AreEqual(3, poolManager.Count);
            Assert.AreEqual(BufferSize * 3, poolManager.CurrentSize);
        }
    }
}