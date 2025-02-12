namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class The_Register_Method
        {
            [TestCase]
            public void RegistersHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.False);

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
            }

            [TestCase]
            public void RegistersHandlerWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"), Is.False);

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"), Is.True);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "anotherTag"), Is.False);
            }

            [TestCase]
            public void ReturnsFalsForDoubleRegistration()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(mediator.Register<string>(recipient, recipient.OnMessage), Is.True);
                Assert.That(mediator.Register<string>(recipient, recipient.OnMessage), Is.False);
            }
        }
    }
}
