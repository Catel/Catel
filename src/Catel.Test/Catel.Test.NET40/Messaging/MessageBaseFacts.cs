// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Messaging
{
    using Catel.Messaging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class MessageBaseFacts
    {
        public class TestMessage : MessageBase<TestMessage, string>
        {
            public TestMessage() { }

            public TestMessage(string content)
                : base(content)
            {
            }
        }

        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void CorrectlySetsInjectionData()
            {
                var message = new TestMessage("my content");

                Assert.AreEqual("my content", message.Data);
            }
        }

        [TestClass]
        public class TheSendWithMethod
        {
            private bool _messageSent;
            private string _messageData;

            [TestMethod]
            public void SendsMessageWithDataWithoutTag()
            {
                var messageMediator = MessageMediator.Default;
                messageMediator.Register<TestMessage>(this, OnTestMessage);

                _messageSent = false;
                _messageData = null;

                TestMessage.SendWith("test");

                messageMediator.Unregister<TestMessage>(this, OnTestMessage);

                Assert.IsTrue(_messageSent);
                Assert.AreEqual("test", _messageData);
            }

            [TestMethod]
            public void SendsMessageWithDataWithTag()
            {
                var messageMediator = MessageMediator.Default;
                messageMediator.Register<TestMessage>(this, OnTestMessage, "mytag");

                _messageSent = false;
                _messageData = null;

                TestMessage.SendWith("test", "mytag");

                messageMediator.Unregister<TestMessage>(this, OnTestMessage, "mytag");

                Assert.IsTrue(_messageSent);
                Assert.AreEqual("test", _messageData);
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
                _messageData = message.Data;
            }
        }

        [TestClass]
        public class TheRegisterMethod
        {
            private bool _messageSent;

            [TestMethod]
            public void RegistersHandlerForMessageWithoutTag()
            {
                TestMessage.Register(this, OnTestMessage);

                _messageSent = false;

                TestMessage.SendWith("test");

                TestMessage.Unregister(this, OnTestMessage);

                Assert.IsTrue(_messageSent);
            }

            [TestMethod]
            public void RegistersHandlerForMessageWithTag()
            {
                TestMessage.Register(this, OnTestMessage, "mytag");

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                TestMessage.Unregister(this, OnTestMessage, "mytag");

                Assert.IsTrue(_messageSent);
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
            }
        }

        [TestClass]
        public class TheUnregisterMethod
        {
            private bool _messageSent;

            [TestMethod]
            public void UnregistersHandlerForMessageWithoutTag()
            {
                TestMessage.Register(this, OnTestMessage);

                _messageSent = false;

                TestMessage.SendWith("test");

                TestMessage.Unregister(this, OnTestMessage);

                Assert.IsTrue(_messageSent);

                _messageSent = false;

                TestMessage.SendWith("test");

                Assert.IsFalse(_messageSent);
            }

            [TestMethod]
            public void UnregistersHandlerForMessageWithTag()
            {
                TestMessage.Register(this, OnTestMessage, "mytag");

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                TestMessage.Unregister(this, OnTestMessage, "mytag");

                Assert.IsTrue(_messageSent);

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                Assert.IsFalse(_messageSent);
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
            }
        }

        [TestClass]
        public class TheWithMethod
        {
            [TestMethod]
            public void CreatesMessageWithData()
            {
                var message = TestMessage.With("test");

                Assert.AreEqual("test", message.Data);
            }
        }
    }
}