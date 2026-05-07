namespace Catel.Tests.Messaging;

using Catel.Messaging;
using NUnit.Framework;

public class SimpleMessageFacts
{
    [TestFixture]
    public class The_With_Method
    {
        [Test]
        public void With_Sets_Data()
        {
            var message = SimpleMessage.With("hello");

            Assert.That(message.Data, Is.EqualTo("hello"));
        }
    }
}
