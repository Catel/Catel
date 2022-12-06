namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using Catel.Reflection;
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

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test");
                messageMediator.SendMessage("test 2");

                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);
            }

            [TestCase]
            public void SubscribesToMessagesWithTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test", "tag");
                messageMediator.SendMessage("test 2", "tag");

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithTag);
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

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test");
                messageMediator.SendMessage("test 2");

                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.UnsubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test 3");
                messageMediator.SendMessage("test 4");

                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);
            }

            [TestCase]
            public void UnsubscribesToMessagesWithTagsCorrectly()
            {
                var messageMediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.SubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test", "tag");
                messageMediator.SendMessage("test 2", "tag");

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithTag);

                recipient.UnsubscribeViaMessageMediatorHelper(messageMediator);

                messageMediator.SendMessage("test 3", "tag");
                messageMediator.SendMessage("test 4", "tag");

                Assert.AreEqual(0, recipient.MessagesReceivedViaMessageMediatorWithoutTag);
                Assert.AreEqual(2, recipient.MessagesReceivedViaMessageMediatorWithTag);
            }
        }
    }
}