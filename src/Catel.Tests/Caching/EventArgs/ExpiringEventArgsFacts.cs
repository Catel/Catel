namespace Catel.Tests.Caching;

using Catel.Caching;
using Catel.Caching.Policies;
using NUnit.Framework;

public class ExpiringEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Sets_Properties_Correctly()
        {
            var policy = ExpirationPolicy.Duration(System.TimeSpan.FromSeconds(30));
            var args = new ExpiringEventArgs<string, int>("key1", 42, policy);

            Assert.That(args.Key, Is.EqualTo("key1"));
            Assert.That(args.Value, Is.EqualTo(42));
            Assert.That(args.ExpirationPolicy, Is.SameAs(policy));
            Assert.That(args.Cancel, Is.False);
        }

        [Test]
        public void Cancel_Can_Be_Changed()
        {
            var args = new ExpiringEventArgs<string, int>("key1", 42, null);

            args.Cancel = true;

            Assert.That(args.Cancel, Is.True);
        }
    }
}
