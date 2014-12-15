// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableTokenFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class DisposableTokenFacts
    {
        public class DisposableTokenTestContainer
        {
            public bool IsSuspended { get; private set; }

            public bool IsDisposed { get; private set; }

            public IDisposable Suspend()
            {
                return new DisposableToken<DisposableTokenTestContainer>(this, x => x.Instance.IsSuspended = true, x => x.Instance.IsDisposed = true);
            }
        }

        [Test]
        public void InitializesWhenConstructed()
        {
            var container = new DisposableTokenTestContainer();

            Assert.IsFalse(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);

            using (var token = container.Suspend())
            {
                Assert.IsTrue(container.IsSuspended);
                Assert.IsFalse(container.IsDisposed);
                Assert.IsTrue(ReferenceEquals(container, ((DisposableToken<DisposableTokenTestContainer>) token).Instance));
            }
        }

        [Test]
        public void DisposesWhenDisposed()
        {
            var container = new DisposableTokenTestContainer();

            Assert.IsFalse(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);

            var token = container.Suspend();

            Assert.IsTrue(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);
            Assert.IsTrue(ReferenceEquals(container, ((DisposableToken<DisposableTokenTestContainer>)token).Instance));

            token.Dispose();

            Assert.IsTrue(container.IsSuspended);
            Assert.IsTrue(container.IsDisposed);
            Assert.IsNull(((DisposableToken<DisposableTokenTestContainer>)token).Instance);            
        }
    }
}