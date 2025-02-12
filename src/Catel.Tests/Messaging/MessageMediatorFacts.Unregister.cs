namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class The_Unregister_Method
        {
            [TestCase]
            public void UnregistersRegisteredHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
                Assert.That(mediator.Unregister<string>(recipient, recipient.OnMessage), Is.True);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.False);
            }

            [TestCase]
            public void UnregistersRegisteredHandlerWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"), Is.True);
                Assert.That(mediator.Unregister<string>(recipient, recipient.OnMessage, "myTag"), Is.True);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"), Is.False);
            }

            [TestCase]
            public void ReturnsFalseForUnregisteredHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.False);
                Assert.That(mediator.Unregister<string>(recipient, recipient.OnMessage), Is.False);
            }

            [TestCase]
            public void UnregistersAllMethodsOfRecipient()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);
                mediator.Register<string>(recipient, recipient.AnotherOnMessage);
                mediator.Register<string>(recipient, recipient.YetAnotherOnMessage);

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.AnotherOnMessage), Is.True);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.YetAnotherOnMessage), Is.True);

                mediator.UnregisterRecipient(recipient);

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.False);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.AnotherOnMessage), Is.False);
                Assert.That(mediator.IsRegistered<string>(recipient, recipient.YetAnotherOnMessage), Is.False);
            }
        }
    }
}
