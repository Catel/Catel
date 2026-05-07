namespace Catel.Tests.Messaging;

using System;
using Catel.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

public class CombinedMessageFacts
{
    [TestFixture]
    public class The_SendWith_Method
    {
        [Test]
        public void SendWith_Sends_Data_And_Exception()
        {
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new object();
            CombinedMessage? receivedMessage = null;
            Action<CombinedMessage> handler = x => receivedMessage = x;

            messageMediator.Register(recipient, handler);

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
                messageMediator.Unregister(recipient, handler);
            }
        }
    }
}
