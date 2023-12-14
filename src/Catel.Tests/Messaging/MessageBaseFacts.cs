namespace Catel.Tests.Messaging
{
    using Catel.Messaging;
    using NUnit.Framework;

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

        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void CorrectlySetsInjectionData()
            {
                var message = new TestMessage("my content");

                Assert.That(message.Data, Is.EqualTo("my content"));
            }
        }

        [TestFixture]
        public class TheSendWithMethod
        {
            private bool _messageSent;
            private string _messageData;

            [TestCase]
            public void RunsInitializerIfSpecified()
            {
                var messageMediator = MessageMediator.Default;
                messageMediator.Register<TestMessage>(this, OnTestMessage);

                _messageSent = false;
                _messageData = null;

                var ranInitializer = false;

                TestMessage.SendWith("test", x =>
                {
                    ranInitializer = true;
                });

                messageMediator.Unregister<TestMessage>(this, OnTestMessage);

                Assert.That(ranInitializer, Is.True);
                Assert.That(_messageSent, Is.True);
                Assert.That(_messageData, Is.EqualTo("test"));
            }

            [TestCase]
            public void SendsMessageWithDataWithoutTag()
            {
                var messageMediator = MessageMediator.Default;
                messageMediator.Register<TestMessage>(this, OnTestMessage);

                _messageSent = false;
                _messageData = null;

                TestMessage.SendWith("test");

                messageMediator.Unregister<TestMessage>(this, OnTestMessage);

                Assert.That(_messageSent, Is.True);
                Assert.That(_messageData, Is.EqualTo("test"));
            }

            [TestCase]
            public void SendsMessageWithDataWithTag()
            {
                var messageMediator = MessageMediator.Default;
                messageMediator.Register<TestMessage>(this, OnTestMessage, "mytag");

                _messageSent = false;
                _messageData = null;

                TestMessage.SendWith("test", "mytag");

                messageMediator.Unregister<TestMessage>(this, OnTestMessage, "mytag");

                Assert.That(_messageSent, Is.True);
                Assert.That(_messageData, Is.EqualTo("test"));
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
                _messageData = message.Data;
            }
        }

        [TestFixture]
        public class TheRegisterMethod
        {
            private bool _messageSent;

            [TestCase]
            public void RegistersHandlerForMessageWithoutTag()
            {
                TestMessage.Register(this, OnTestMessage);

                _messageSent = false;

                TestMessage.SendWith("test");

                TestMessage.Unregister(this, OnTestMessage);

                Assert.That(_messageSent, Is.True);
            }

            [TestCase]
            public void RegistersHandlerForMessageWithTag()
            {
                TestMessage.Register(this, OnTestMessage, "mytag");

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                TestMessage.Unregister(this, OnTestMessage, "mytag");

                Assert.That(_messageSent, Is.True);
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
            }
        }

        [TestFixture]
        public class TheUnregisterMethod
        {
            private bool _messageSent;

            [TestCase]
            public void UnregistersHandlerForMessageWithoutTag()
            {
                TestMessage.Register(this, OnTestMessage);

                _messageSent = false;

                TestMessage.SendWith("test");

                TestMessage.Unregister(this, OnTestMessage);

                Assert.That(_messageSent, Is.True);

                _messageSent = false;

                TestMessage.SendWith("test");

                Assert.That(_messageSent, Is.False);
            }

            [TestCase]
            public void UnregistersHandlerForMessageWithTag()
            {
                TestMessage.Register(this, OnTestMessage, "mytag");

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                TestMessage.Unregister(this, OnTestMessage, "mytag");

                Assert.That(_messageSent, Is.True);

                _messageSent = false;

                TestMessage.SendWith("test", "mytag");

                Assert.That(_messageSent, Is.False);
            }

            public void OnTestMessage(TestMessage message)
            {
                _messageSent = true;
            }
        }

        [TestFixture]
        public class TheWithMethod
        {
            [TestCase]
            public void CreatesMessageWithData()
            {
                var message = TestMessage.With("test");

                Assert.That(message.Data, Is.EqualTo("test"));
            }
        }
    }
}
