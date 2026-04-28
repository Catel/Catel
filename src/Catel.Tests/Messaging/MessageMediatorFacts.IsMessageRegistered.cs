namespace Catel.Tests.Messaging;

using System;
using Catel.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

public partial class MessageMediatorFacts
{
    [TestFixture]
    public class The_IsMessageRegistered_Method
    {
        [TestCase]
        public void ReturnsArgumentNullException()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            Assert.Throws<ArgumentNullException>(() => mediator.IsMessageRegistered(null));
        }


        [TestCase]
        public void ReturnsFalseWhenNotRegistered()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.False);
        }

        [TestCase]
        public void ReturnsTrueWhenRegistered()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new MessageRecipient();

            mediator.Register<string>(recipient, recipient.OnMessage);

            Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.True);
        }

        [TestCase]
        public void ReturnsFalseWhenNotRegisteredWithTag()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new MessageRecipient();

            mediator.Register<string>(recipient, recipient.OnMessage);

            Assert.That(mediator.IsMessageRegistered(typeof(string), "myTag"), Is.False);
        }

        [TestCase]
        public void ReturnsTrueWhenRegisteredWithTag()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new MessageRecipient();

            mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

            Assert.That(mediator.IsMessageRegistered(typeof(string), "myTag"), Is.True);
        }

        [TestCase]
        public void ReturnsFalseWhenRegisteredWithTagButNotProvided()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new MessageRecipient();

            mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

            Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.False);
        }

        [TestCase]
        public void ReturnsTrueWhenRegisteredWithAndWithoutTag()
        {
            var mediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            var recipient = new MessageRecipient();

            mediator.Register<string>(recipient, recipient.OnMessage, "myTag");
            mediator.Register<string>(recipient, recipient.AnotherOnMessage);

            Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.True);
        }
    }
}
