namespace Catel.Tests.Messaging
{
    using Catel.Messaging;

    public class MessageSender
    {
        public bool SendMessage(IMessageMediator messageMediator, string message)
        {
            return SendMessage(messageMediator, message, null);
        }

        public bool SendMessage(IMessageMediator messageMediator, string message, object tag)
        {
            return messageMediator.SendMessage(message, tag);
        }
    }

    public class MessageRecipient
    {
        public int MessagesReceived { get; private set; }

        public int MessagesReceivedViaMessageMediatorWithTag { get; private set; }

        public int MessagesReceivedViaMessageMediatorWithoutTag { get; private set; }

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
}
