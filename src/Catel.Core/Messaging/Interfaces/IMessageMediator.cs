namespace Catel.Messaging
{
    using System;

    ///<summary>
    /// The interface definition for our Message mediator. This allows loose-event coupling between components
    /// in an application by sending messages to registered elements.
    /// <para />
    /// This class implements the mediator pattern.
    ///</summary>
    public interface IMessageMediator
    {
        /// <summary>
        /// Determines whether the specified message type is registered.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> if the message type is registered; otherwise, <c>false</c>.
        /// </returns>
        bool IsMessageRegistered<TMessage>(object? tag = null);

        /// <summary>
        /// Determines whether the specified message type is registered.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> if the message type is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="messageType"/> is <c>null</c>.</exception>
        bool IsMessageRegistered(Type messageType, object? tag = null);

        /// <summary>
        /// Registers a specific recipient for a specific message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to register.</param>
        /// <param name="handler">The handler method.</param>
        /// <param name="tag">The message tag.</param>
        /// <returns><c>true</c> if the handler is registered successfully; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// A handler cannot be registered twice. If the same handler is already registered, this method will
        /// return <c>false</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        bool Register<TMessage>(object recipient, Action<TMessage> handler, object? tag = null);

        /// <summary>
        /// Unregisters a specific recipient for a specific message with the specified tag.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="handler">The handler method.</param>
        /// <param name="tag">The message tag.</param>
        /// <returns><c>true</c> if the handler is unregistered successfully; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// A handler cannot be unregistered when it is not registered first. If a handler is unregistered while it
        /// is not registered, this method will return <c>false</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        bool Unregister<TMessage>(object recipient, Action<TMessage> handler, object? tag = null);

        /// <summary>
        /// Unregisters a specific recipient for all the (non-static) message the recipient is subscribed to.
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="tag">The message tag.</param>
        /// <returns><c>true</c> if the handler is unregistered successfully; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// A handler cannot be unregistered when it is not registered first. If a handler is unregistered while it
        /// is not registered, this method will return <c>false</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="recipient"/> is <c>null</c>.</exception>
        bool UnregisterRecipient(object recipient, object? tag = null);

        /// <summary>
        /// Unregisters a specific recipient for all the (non-static) message the recipient is subscribed to. 
        /// <para />
        /// This method ignores any tags. If a message recipient matches the specified recipient, it is unsubscribed.
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <returns><c>true</c> if the handler is unregistered successfully; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// A handler cannot be unregistered when it is not registered first. If a handler is unregistered while it
        /// is not registered, this method will return <c>false</c>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="recipient"/> is <c>null</c>.</exception>
        bool UnregisterRecipientAndIgnoreTags(object recipient);

        /// <summary>
        /// Broadcasts a message to all message targets for a given message tag and passes a parameter.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message parameter.</param>
        /// <param name="tag">The message tag.</param>
        /// <returns>
        /// <c>true</c> if any handlers were invoked; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        bool SendMessage<TMessage>(TMessage message, object? tag = null);

        /// <summary>
        /// Cleans up the list of registered handlers. All handlers that are no longer alive
        /// are removed from the list.
        /// <para />
        /// This method is automatically invoked after each call to <see cref="SendMessage{TMessage}(TMessage, object)"/>, but
        /// can also be invoked manually.
        /// </summary>
        void CleanUp();
    }
}
