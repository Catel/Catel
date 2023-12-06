namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

    #region Classes
    public class MessageSender
    {
        #region Methods
        public bool SendMessage(IMessageMediator messageMediator, string message)
        {
            return SendMessage(messageMediator, message, null);
        }

        public bool SendMessage(IMessageMediator messageMediator, string message, object tag)
        {
            return messageMediator.SendMessage(message, tag);
        }
        #endregion
    }

    public class MessageRecipient
    {
        #region Properties
        public int MessagesReceived { get; private set; }

        public int MessagesReceivedViaMessageMediatorWithTag { get; private set; }

        public int MessagesReceivedViaMessageMediatorWithoutTag { get; private set; }
        #endregion

        #region Methods
        public void OnMessage(string message)
        {
            MessagesReceived++;
        }

        public void AnotherOnMessage(string message)
        {
            MessagesReceived++;
        }

        public void YetAnotherOnMessage(string message)
        {
            MessagesReceived++;
        }

        [MessageRecipient]
        public void OnMessageWithoutTag(string message)
        {
            MessagesReceivedViaMessageMediatorWithoutTag++;
        }

        [MessageRecipient(Tag = "tag")]
        public void OnMessageWithTag(string message)
        {
            MessagesReceivedViaMessageMediatorWithTag++;
        }

        public void SubscribeViaMessageMediatorHelper(IMessageMediator messageMediator)
        {
            MessageMediatorHelper.SubscribeRecipient(this, messageMediator);
        }

        public void UnsubscribeViaMessageMediatorHelper(IMessageMediator messageMediator)
        {
            MessageMediatorHelper.UnsubscribeRecipient(this, messageMediator);
        }
        #endregion
    }
    #endregion

    public class MessageMediatorFacts
    {
        #region Nested type: TheCleanUpMethod
        [TestFixture]
        public class TheCleanUpMethod
        {
            #region Methods
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
            #endregion
        }
        #endregion

        #region Nested type: TheIsMessageRegisteredMethod
        [TestFixture]
        public class TheIsMessageRegisteredMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsArgumentNullException()
            {
                var mediator = new MessageMediator();

                Assert.Throws<ArgumentNullException>(() => mediator.IsMessageRegistered(null));
            }


            [TestCase]
            public void ReturnsFalseWhenNotRegistered()
            {
                var mediator = new MessageMediator();

                Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.False);
            }

            [TestCase]
            public void ReturnsTrueWhenRegistered()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.True);
            }

            [TestCase]
            public void ReturnsFalseWhenNotRegisteredWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.That(mediator.IsMessageRegistered(typeof(string), "myTag"), Is.False);
            }

            [TestCase]
            public void ReturnsTrueWhenRegisteredWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.That(mediator.IsMessageRegistered(typeof(string), "myTag"), Is.True);
            }

            [TestCase]
            public void ReturnsFalseWhenRegisteredWithTagButNotProvided()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.False);
            }

            [TestCase]
            public void ReturnsTrueWhenRegisteredWithAndWithoutTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");
                mediator.Register<string>(recipient, recipient.AnotherOnMessage);

                Assert.That(mediator.IsMessageRegistered(typeof(string)), Is.True);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheRegisterMethod
        [TestFixture]
        public class TheRegisterMethod
        {
            #region Methods
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
            #endregion
        }
        #endregion

        #region Nested type: TheSendMessageMethod
        [TestFixture]
        public class TheSendMessageMethod
        {
            #region Methods
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
            #endregion
        }
        #endregion

        #region Nested type: TheUnregisterMethod
        [TestFixture]
        public class TheUnregisterMethod
        {
            #region Methods
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
            #endregion
        }
        #endregion


        [TestFixture]
        public class TheIsRegisteredMethod
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

    public class Message
    {
        public string Text { get; set; }
    }

    public class ReceiverA
    {
        public string Received { get; private set; }
        public void OnMessageReceived(Message msg)
        {
            Received = msg.Text;
        }
    }

    public class ReceiverB
    {
        public string Received { get; private set; }
        public void OnMessageReceived(Message msg)
        {
            Received = msg.Text;
        }
    }

    [TestFixture]
    public class TestRegistrationOfMethodsWithSameName
    {
        [Test]
        public void SendMessage()
        {
            var a = new ReceiverA();
            var b = new ReceiverB();

            var m = new MessageMediator();

            m.Register<Message>(a, a.OnMessageReceived);
            m.Register<Message>(b, b.OnMessageReceived);
            m.Unregister<Message>(b, b.OnMessageReceived); // this actually unregisters a's handler, not b's handler.

            m.SendMessage(new Message { Text = "hello" });
            Assert.That(a.Received, Is.EqualTo("hello"));
        }
    }
}
