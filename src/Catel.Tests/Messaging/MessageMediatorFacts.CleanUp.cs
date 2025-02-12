namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public partial class MessageMediatorFacts
    {
        [TestFixture]
        public class TheCleanUpMethod
        {
            [TestCase]
            public void CleanUpWorksWhenNoHandlersRegistered()
            {
                var mediator = new MessageMediator();

                mediator.CleanUp();
            }

            [TestCase]
            public void CleanUpKeepsNonGarbageCollectedHandlersRegistered()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                mediator.CleanUp();

                Assert.That(mediator.IsRegistered<string>(recipient, recipient.OnMessage), Is.True);
            }

            [TestCase, Explicit]
            public void CleanUpClearsGarbageCollectedHandlers()
            {
                var mediator = new MessageMediator();
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
