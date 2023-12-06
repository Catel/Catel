namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    public class MessageMediatorHelperFacts
    {
        [TestFixture]
        public class TheSubscribeRecipientMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                Assert.Throws<ArgumentNullException>(() => MessageMediatorHelper.SubscribeRecipient(null));
            }

            [TestCase]
            public void SubscribesToMessagesWithoutTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test");
                messageMediator.SendMessage("test 2");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(2));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));
            }

            [TestCase]
            public void SubscribesToMessagesWithTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test", "tag");
                messageMediator.SendMessage("test 2", "tag");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(2));
            }
        }

        [TestFixture]
        public class TheUnsubscribeRecipientMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                Assert.Throws<ArgumentNullException>(() => MessageMediatorHelper.UnsubscribeRecipient(null));
            }

            [TestCase]
            public void UnsubscribesToMessagesWithoutTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test");
                messageMediator.SendMessage("test 2");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(2));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));

                recipient.UnsubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test 3");
                messageMediator.SendMessage("test 4");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(2));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));
            }

            [TestCase]
            public void UnsubscribesToMessagesWithTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(0));

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test", "tag");
                messageMediator.SendMessage("test 2", "tag");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(2));

                recipient.UnsubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test 3", "tag");
                messageMediator.SendMessage("test 4", "tag");

                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithoutTag, Is.EqualTo(0));
                Assert.That(recipient.MessagesReceivedViaMessageMediatorWithTag, Is.EqualTo(2));
            }
        }
    }
}