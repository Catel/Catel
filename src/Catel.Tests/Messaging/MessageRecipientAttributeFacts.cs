namespace Catel.Tests.Messaging;

using Catel.Messaging;
using NUnit.Framework;

public class MessageRecipientAttributeFacts
{
    [TestFixture]
    public class The_Tag_Property
    {
        [Test]
        public void Allows_Setting_Tag()
        {
            var attribute = new MessageRecipientAttribute();

            attribute.Tag = "my-tag";

            Assert.That(attribute.Tag, Is.EqualTo("my-tag"));
        }
    }
}
