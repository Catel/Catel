namespace Catel.Tests.Messaging;

using Catel.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

public class MessageBaseFacts
{
    public class TestMessage : MessageBase<TestMessage, string>
    {
        public TestMessage()
        {
        }

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
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            messageMediator.Register<TestMessage>(this, OnTestMessage);

            _messageSent = false;
            _messageData = null;

            var ranInitializer = false;

            TestMessage.SendWith(messageMediator, "test", x =>
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
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            messageMediator.Register<TestMessage>(this, OnTestMessage);

            _messageSent = false;
            _messageData = null;

            TestMessage.SendWith(messageMediator, "test");

            messageMediator.Unregister<TestMessage>(this, OnTestMessage);

            Assert.That(_messageSent, Is.True);
            Assert.That(_messageData, Is.EqualTo("test"));
        }

        [TestCase]
        public void SendsMessageWithDataWithTag()
        {
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);
            messageMediator.Register<TestMessage>(this, OnTestMessage, "mytag");

            _messageSent = false;
            _messageData = null;

            TestMessage.SendWith(messageMediator, "test", "mytag");

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
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            TestMessage.Register(messageMediator, this, OnTestMessage);

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test");

            TestMessage.Unregister(messageMediator, this, OnTestMessage);

            Assert.That(_messageSent, Is.True);
        }

        [TestCase]
        public void RegistersHandlerForMessageWithTag()
        {
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            TestMessage.Register(messageMediator, this, OnTestMessage, "mytag");

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test", "mytag");

            TestMessage.Unregister(messageMediator, this, OnTestMessage, "mytag");

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
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            TestMessage.Register(messageMediator, this, OnTestMessage);

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test");

            TestMessage.Unregister(messageMediator, this, OnTestMessage);

            Assert.That(_messageSent, Is.True);

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test");

            Assert.That(_messageSent, Is.False);
        }

        [TestCase]
        public void UnregistersHandlerForMessageWithTag()
        {
            var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

            TestMessage.Register(messageMediator, this, OnTestMessage, "mytag");

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test", "mytag");

            TestMessage.Unregister(messageMediator, this, OnTestMessage, "mytag");

            Assert.That(_messageSent, Is.True);

            _messageSent = false;

            TestMessage.SendWith(messageMediator, "test", "mytag");

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
