namespace Catel.Tests.Caching;

using Catel.Caching;
using Catel.Caching.Policies;
using NUnit.Framework;

public class ExpiredEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Sets_Properties_Correctly()
        {
            var args = new ExpiredEventArgs<string, int>("key1", 42, true);

            Assert.That(args.Key, Is.EqualTo("key1"));
            Assert.That(args.Value, Is.EqualTo(42));
            Assert.That(args.Dispose, Is.True);
        }

        [Test]
        public void Dispose_Can_Be_Changed()
        {
            var args = new ExpiredEventArgs<string, int>("key1", 42, true);

            args.Dispose = false;

            Assert.That(args.Dispose, Is.False);
        }
    }
}
