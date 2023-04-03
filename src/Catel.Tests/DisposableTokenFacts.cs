namespace Catel.Tests
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
        public void Accepts_Null_Instance()
        {
            using (var token = new DisposableToken(null, 
                x => { },
                x => { }))
            {

            }
        }

        [Test]
        public void Initializes_When_Constructed()
        {
            var container = new DisposableTokenTestContainer();

            Assert.IsFalse(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);

            using (var token = container.Suspend())
            {
                Assert.IsTrue(container.IsSuspended);
                Assert.IsFalse(container.IsDisposed);
                Assert.IsTrue(ReferenceEquals(container, ((DisposableToken<DisposableTokenTestContainer>)token).Instance));
            }
        }

        [Test]
        public void Disposes_When_Disposed()
        {
            var container = new DisposableTokenTestContainer();

            Assert.IsFalse(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);

            var token = container.Suspend();

            Assert.IsTrue(container.IsSuspended);
            Assert.IsFalse(container.IsDisposed);
            Assert.IsTrue(ReferenceEquals(container, ((DisposableToken<DisposableTokenTestContainer>)token).Instance));

#pragma warning disable IDISP017 // Prefer using.
#pragma warning disable IDISP016 // Don't use disposed instance.
            token.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
#pragma warning restore IDISP017 // Prefer using.

            Assert.IsTrue(container.IsSuspended);
            Assert.IsTrue(container.IsDisposed);
        }
    }
}
