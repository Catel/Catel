namespace Catel.Tests.Messaging
{
    using System;
    using Catel.Messaging;
    using NUnit.Framework;

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
