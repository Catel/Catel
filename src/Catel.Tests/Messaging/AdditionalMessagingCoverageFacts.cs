namespace Catel.Tests.Messaging;

using System;
using Catel.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

public class AdditionalMessagingCoverageFacts
{
    [TestFixture]
    public class The_CombinedMessage_Class
    {
        [Test]
        public void SendWith_Sends_Data_And_Exception()
        {
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            CombinedMessage? receivedMessage = null;
            Action<CombinedMessage> handler = x => receivedMessage = x;

            messageMediator.Register(typeof(AdditionalMessagingCoverageFacts), handler);

            try
            {
                var expectedException = new InvalidOperationException("boom");

                CombinedMessage.SendWith(messageMediator, true, expectedException);

                Assert.That(receivedMessage, Is.Not.Null);
                Assert.That(receivedMessage!.Data, Is.True);
                Assert.That(receivedMessage.Exception, Is.EqualTo(expectedException));
            }
            finally
            {
                messageMediator.Unregister(typeof(AdditionalMessagingCoverageFacts), handler);
            }
        }
    }

    [TestFixture]
    public class The_SimpleMessage_Class
    {
        [Test]
        public void With_Sets_Data()
        {
            var message = SimpleMessage.With("hello");

            Assert.That(message.Data, Is.EqualTo("hello"));
        }
    }

    [TestFixture]
    public class The_MessageRecipientAttribute_Class
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
