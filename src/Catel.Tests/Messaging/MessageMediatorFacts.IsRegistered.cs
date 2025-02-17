namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class The_IsRegistered_Method
        {
            [Test]
            public void ReturnsTrueAfterRegistration()
            {
                var recipient = new MessageRecipient();
                var messageMediator = new MessageMediator();
                messageMediator.Register<string>(recipient, recipient.OnMessage);
                Assert.That(messageMediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
            }

            [Test]
            public void ReturnsFalseAfterGarbageCollected()
            {
                var recipient = new MessageRecipient();
                var messageMediator = new MessageMediator();
                messageMediator.Register<string>(recipient, recipient.OnMessage);

                recipient = null;

                GC.Collect();

                recipient = new MessageRecipient();
                Assert.That(messageMediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.False);
            }
        }
    }
}
