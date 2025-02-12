namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class The_SendMessage_Method
        {
            [TestCase]
            public void ReturnsFalseForUnregisteredHandlers()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();

                Assert.That(sender.SendMessage(mediator, "test"), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForUnregisteredHandlersWithTag()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(sender.SendMessage(mediator, "test", "myTag"), Is.False);
            }

            [TestCase]
            public void ReturnsTrueForRegisteredHandlers()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(sender.SendMessage(mediator, "test"), Is.True);
                Assert.That(recipient.MessagesReceived, Is.EqualTo(1));
            }

            [TestCase]
            public void ReturnsTrueForRegisteredHandlersWithTag()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                // Double registration with separate tags is possible
                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");
                mediator.Register<string>(recipient, recipient.OnMessage, "anotherTag");

                Assert.That(sender.SendMessage(mediator, "test", "myTag"), Is.True);
                Assert.That(recipient.MessagesReceived, Is.EqualTo(1));
            }

            [TestCase, Explicit]
            public void ReturnsFalseForHandlersClearedByGarbageCollector()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                recipient = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.That(sender.SendMessage(mediator, "test"), Is.False);
                Assert.That(mediator.GetRegisteredHandlers<string>().Count, Is.EqualTo(0), "SendMessage should auto cleanup");
            }
        }
    }
}
