namespace Catel.Messaging
{
    public static class IMessageMediatorExtensions
    {
        /// <summary>
        /// Determines whether the specified message type is registered.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="messageMediator">The message mediator.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> if the message type is registered; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMessageRegistered<TMessage>(this IMessageMediator messageMediator, object? tag = null)
        {
            var messageType = typeof(TMessage);

            return messageMediator.IsMessageRegistered(messageType, tag);
        }
    }
}
