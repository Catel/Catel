namespace Catel.Messaging
{
    /// <summary>
    /// Simplified base class for messages distributed via the Catel MessageMediator subsystem. Inherit from this class
    /// to define individual message types.
    /// <para />
    /// An alternative to this class is the <seealso cref="MessageBase{TMessage, TData}"/>, but it's a bit more verbose to define.
    /// </summary>
    public abstract class MessageBase : IMessage
    {
        // Empty by design, but as a placeholder to make it easier to use instead of MessageBase<TMessage, TData>
    }
}
