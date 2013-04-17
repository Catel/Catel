// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageMediatorHelperFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Messaging
{
    using System;
    using Catel.Messaging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class MessageMediatorHelperFacts
    {
        [TestClass]
        public class TheSubscribeRecipientMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => MessageMediatorHelper.SubscribeRecipient(null));
            }

            [TestMethod]
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

            [TestMethod]
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

        [TestClass]
        public class TheUnsubscribeRecipientMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => MessageMediatorHelper.UnsubscribeRecipient(null));
            }

            [TestMethod]
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

            [TestMethod]
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