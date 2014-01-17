// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageMediatorFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        [TestClass]
        public class TheCleanUpMethod
        {
            #region Methods
            [TestMethod]
            public void CleanUpWorksWhenNoHandlersRegistered()
            {
                var mediator = new MessageMediator();

                mediator.CleanUp();
            }

            [TestMethod]
            public void CleanUpKeepsNonGarbageCollectedHandlersRegistered()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                mediator.CleanUp();

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
            }

            [TestMethod]
            public void CleanUpClearsGarbageCollectedHandlers()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                recipient = null;
                GC.Collect();

                mediator.CleanUp();

                Assert.AreEqual(0, mediator.GetRegisteredHandlers<string>().Count);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsMessageRegisteredMethod
        [TestClass]
        public class TheIsMessageRegisteredMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsArgumentNullException()
            {
                var mediator = new MessageMediator();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => mediator.IsMessageRegistered(null));
            }


            [TestMethod]
            public void ReturnsFalseWhenNotRegistered()
            {
                var mediator = new MessageMediator();

                Assert.IsFalse(mediator.IsMessageRegistered(typeof (string)));
            }

            [TestMethod]
            public void ReturnsTrueWhenRegistered()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsTrue(mediator.IsMessageRegistered(typeof (string)));
            }

            [TestMethod]
            public void ReturnsFalseWhenNotRegisteredWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsFalse(mediator.IsMessageRegistered(typeof (string), "myTag"));
            }

            [TestMethod]
            public void ReturnsTrueWhenRegisteredWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.IsTrue(mediator.IsMessageRegistered(typeof (string), "myTag"));
            }

            [TestMethod]
            public void ReturnsFalseWhenRegisteredWithTagButNotProvided()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.IsFalse(mediator.IsMessageRegistered(typeof(string)));
            }

            [TestMethod]
            public void ReturnsTrueWhenRegisteredWithAndWithoutTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");
                mediator.Register<string>(recipient, recipient.AnotherOnMessage);

                Assert.IsTrue(mediator.IsMessageRegistered(typeof(string)));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheRegisterMethod
        [TestClass]
        public class TheRegisterMethod
        {
            #region Methods
            [TestMethod]
            public void RegistersHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage));

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
            }

            [TestMethod]
            public void RegistersHandlerWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"));

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"));
                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "anotherTag"));
            }

            [TestMethod]
            public void ReturnsFalsForDoubleRegistration()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.IsTrue(mediator.Register<string>(recipient, recipient.OnMessage));
                Assert.IsFalse(mediator.Register<string>(recipient, recipient.OnMessage));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheSendMessageMethod
        [TestClass]
        public class TheSendMessageMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsFalseForUnregisteredHandlers()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();

                Assert.IsFalse(sender.SendMessage(mediator, "test"));
            }

            [TestMethod]
            public void ReturnsFalseForUnregisteredHandlersWithTag()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsFalse(sender.SendMessage(mediator, "test", "myTag"));
            }

            [TestMethod]
            public void ReturnsTrueForRegisteredHandlers()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsTrue(sender.SendMessage(mediator, "test"));
                Assert.AreEqual(1, recipient.MessagesReceived);
            }

            [TestMethod]
            public void ReturnsTrueForRegisteredHandlersWithTag()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                // Double registration with separate tags is possible
                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");
                mediator.Register<string>(recipient, recipient.OnMessage, "anotherTag");

                Assert.IsTrue(sender.SendMessage(mediator, "test", "myTag"));
                Assert.AreEqual(1, recipient.MessagesReceived);
            }

            [TestMethod]
            public void ReturnsFalseForHandlersClearedByGarbageCollector()
            {
                var mediator = new MessageMediator();
                var sender = new MessageSender();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                recipient = null;
                GC.Collect();

                Assert.IsFalse(sender.SendMessage(mediator, "test"));
                Assert.AreEqual(0, mediator.GetRegisteredHandlers<string>().Count, "SendMessage should auto cleanup");
            }
            #endregion
        }
        #endregion

        #region Nested type: TheUnregisterMethod
        [TestClass]
        public class TheUnregisterMethod
        {
            #region Methods
            [TestMethod]
            public void UnregistersRegisteredHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
                Assert.IsTrue(mediator.Unregister<string>(recipient, recipient.OnMessage));
                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
            }

            [TestMethod]
            public void UnregistersRegisteredHandlerWithTag()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage, "myTag");

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"));
                Assert.IsTrue(mediator.Unregister<string>(recipient, recipient.OnMessage, "myTag"));
                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage, "myTag"));
            }

            [TestMethod]
            public void ReturnsFalseForUnregisteredHandler()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
                Assert.IsFalse(mediator.Unregister<string>(recipient, recipient.OnMessage));
            }

            [TestMethod]
            public void UnregistersAllMethodsOfRecipient()
            {
                var mediator = new MessageMediator();
                var recipient = new MessageRecipient();

                mediator.Register<string>(recipient, recipient.OnMessage);
                mediator.Register<string>(recipient, recipient.AnotherOnMessage);
                mediator.Register<string>(recipient, recipient.YetAnotherOnMessage);

                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.AnotherOnMessage));
                Assert.IsTrue(mediator.IsRegistered<string>(recipient, recipient.YetAnotherOnMessage));

                mediator.UnregisterRecipient(recipient);

                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.OnMessage));
                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.AnotherOnMessage));
                Assert.IsFalse(mediator.IsRegistered<string>(recipient, recipient.YetAnotherOnMessage));
            }
            #endregion
        }
        #endregion
    }
}