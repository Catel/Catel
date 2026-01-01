namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using Microsoft.Extensions.Logging.Abstractions;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class The_CleanUp_Method
        {
            [TestCase]
            public void CleanUpWorksWhenNoHandlersRegistered()
            {
                var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

                mediator.CleanUp();
            }

            [TestCase]
            public void CleanUpKeepsNonGarbageCollectedHandlersRegistered()
            {
                var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                mediator.CleanUp();

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
            }

            [TestCase, Explicit]
            public void CleanUpClearsGarbageCollectedHandlers()
            {
                var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                recipient = null;
                GC.Collect();

                mediator.CleanUp();

                Assert.That(mediator.GetRegisteredHandlers<string>().Count, Is.EqualTo(0));
            }
        }
    }
}
