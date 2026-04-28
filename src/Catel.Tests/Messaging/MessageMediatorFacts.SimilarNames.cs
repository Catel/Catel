namespace Catel.Tests.Messaging;

using Catel.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

[TestFixture]
public class TestRegistrationOfMethodsWithSameName
{
    [Test]
    public void SendMessage()
    {
        var a = new ReceiverA();
        var b = new ReceiverB();

        var messageMediator = new MessageMediator(NullLogger<MessageMediator>.Instance);

        messageMediator.Register<Message>(a, a.OnMessageReceived);
        messageMediator.Register<Message>(b, b.OnMessageReceived);
        messageMediator.Unregister<Message>(b, b.OnMessageReceived); // this actually unregisters a's handler, not b's handler.

        messageMediator.SendMessage(new Message { Text = "hello" });

        Assert.That(a.Received, Is.EqualTo("hello"));
    }
}
