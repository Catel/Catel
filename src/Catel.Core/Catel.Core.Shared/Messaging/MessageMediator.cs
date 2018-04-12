// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageMediator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// This class creates a simple Mediator which loosely connects different objects together.
    /// <para/>
    /// The message handlers are organized using string-based message keys and are held in a WeakReference collection.
    /// </summary>
    public class MessageMediator : IMessageMediator
    {
        #region Types
        /// <summary>
        /// Object containing the weak action and the tag of a weak action.
        /// </summary>
        private struct WeakActionInfo
        {
            #region Fields
            /// <summary>
            /// The action to execute, which is always a <see cref="IWeakAction{TParameter}"/>.
            /// </summary>
            public IExecuteWithObject Action;

            /// <summary>
            /// The tag which can be used to make a difference between messages.
            /// </summary>
            public object Tag;
            #endregion
        }
        #endregion

        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The static instance of the message mediator.
        /// </summary>
        private static readonly IMessageMediator _instance = new MessageMediator();
        #endregion

        #region Fields
        private readonly object _lockObject = new object();

        /// <summary>
        /// The currently registered handlers. The key is the type of the message, then the value is a list of
        /// interested listeners.
        /// </summary>
        private readonly Dictionary<Type, List<WeakActionInfo>> _registeredHandlers = new Dictionary<Type, List<WeakActionInfo>>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance of the message mediator.
        /// </summary>
        /// <value>The default instance.</value>
        public static IMessageMediator Default
        {
            get { return _instance; }
        }
        #endregion

        #region IMessageMediator Members
        /// <summary>
        /// Determines whether the specified message type is registered.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> if the message type is registered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMessageRegistered<TMessage>(object tag = null)
        {
            var messageType = typeof (TMessage);

            return IsMessageRegistered(messageType, tag);
        }

        /// <summary>
        /// Determines whether the specified message type is registered.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        ///   <c>true</c> if the message type is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="messageType"/> is <c>null</c>.</exception>
        public bool IsMessageRegistered(Type messageType, object tag = null)
        {
            Argument.IsNotNull("messageType", messageType);

            lock (_lockObject)
            {
                if (_registeredHandlers.TryGetValue(messageType, out var messageHandlers))
                {
                    return messageHandlers.Any(handlerInfo => TagHelper.AreTagsEqual(tag, handlerInfo.Tag));
                }

                return false;
            }
        }

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
        public bool Register<TMessage>(object recipient, Action<TMessage> handler, object tag = null)
        {
            Argument.IsNotNull("handler", handler);

            lock (_lockObject)
            {
                var messageType = typeof (TMessage);

                if (IsRegistered(recipient, handler, tag))
                {
                    Log.Warning("Same handler for message type '{0}' with tag '{1}' is already registered, skipping registration", messageType.Name, ObjectToStringHelper.ToString(tag));

                    return false;
                }

                if (!_registeredHandlers.ContainsKey(messageType))
                {
                    _registeredHandlers.Add(messageType, new List<WeakActionInfo>());
                }

                var handlerInfo = new WeakActionInfo
                {
                    Action = new WeakAction<TMessage>(recipient, handler),
                    Tag = tag
                };

                var list = _registeredHandlers[messageType];
                list.Add(handlerInfo);

                Log.Debug("Registered handler for message type '{0}' with tag '{1}'", messageType.Name, ObjectToStringHelper.ToString(tag));

                return true;
            }
        }

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
        public bool Unregister<TMessage>(object recipient, Action<TMessage> handler, object tag = null)
        {
            Argument.IsNotNull("handler", handler);

            lock (_lockObject)
            {
                var messageType = typeof (TMessage);

                if (_registeredHandlers.TryGetValue(messageType, out var messageHandlers))
                {
                    for (var i = 0; i < messageHandlers.Count; i++)
                    {
                        var handlerInfo = messageHandlers[i];
                        var weakAction = (IWeakAction<TMessage>) handlerInfo.Action;
                        
                        if (!ReferenceEquals(recipient, weakAction.Target)) 
                        {
                            continue;
                        }
                        
                        if (TagHelper.AreTagsEqual(tag, handlerInfo.Tag) && AreEqualHandlers(handler, weakAction))
                        {
                            messageHandlers.RemoveAt(i--);

                            Log.Debug("Unregistered handler for message type '{0}' with tag '{1}'", messageType.Name, ObjectToStringHelper.ToString(tag));

                            return true;
                        }
                    }
                }

                Log.Warning("Failed to unregister handler for message type '{0}' with tag '{1}' because handler was not registered", messageType.Name, ObjectToStringHelper.ToString(tag));

                return false;
            }
        }

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
        public bool UnregisterRecipient(object recipient, object tag = null)
        {
            return UnregisterRecipient(recipient, null, true);
        }

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
        public bool UnregisterRecipientAndIgnoreTags(object recipient)
        {
            return UnregisterRecipient(recipient, null, true);
        }

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
        public bool SendMessage<TMessage>(TMessage message, object tag = null)
        {
            Argument.IsNotNull("message", message);

            Log.Debug("Sending message of type '{0}' with tag '{1}'", message.GetType().FullName, ObjectToStringHelper.ToString(tag));

            var invokedHandlersCount = 0;

            lock (_lockObject)
            {
                var messageType = typeof (TMessage);

                if (_registeredHandlers.TryGetValue(messageType, out var messageHandlerList))
                {
                    // CTL-311: first convert to array, then handle messages
                    var messageHandlers = messageHandlerList.ToArray();
                    foreach (var handler in messageHandlers)
                    {
                        if (TagHelper.AreTagsEqual(tag, handler.Tag))
                        {
                            if (handler.Action.ExecuteWithObject(message))
                            {
                                invokedHandlersCount++;
                            }
                        }
                    }
                }
            }

            Log.Debug("Sent message to {0} recipients", invokedHandlersCount);

            CleanUp();

            return invokedHandlersCount != 0;
        }

        /// <summary>
        /// Cleans up the list of registered handlers. All handlers that are no longer alive
        /// are removed from the list.
        /// <para />
        /// This method is automatically invoked after each call to <see cref="SendMessage{TMessage}(TMessage, object)"/>, but
        /// can also be invoked manually.
        /// </summary>
        public void CleanUp()
        {
            Log.Debug("Cleaning up handlers");

            lock (_lockObject)
            {
                foreach (var handlerKeyPair in _registeredHandlers)
                {
                    var handlers = handlerKeyPair.Value;
                    for (var i = 0; i < handlers.Count; i++)
                    {
                        var handler = handlers[i];
                        if (!((IWeakReference) handler.Action).IsTargetAlive)
                        {
                            handlers.RemoveAt(i--);

                            Log.Debug("Removed handler for message type '{0}' with tag '{1}' because target is no longer alive",
                                      handlerKeyPair.Key.Name, ObjectToStringHelper.ToString(handler.Tag));
                        }
                    }
                }
            }

            Log.Debug("Cleaned up handlers");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Unregisters a specific recipient for all the (non-static) message the recipient is subscribed to.
        /// </summary>
        /// <param name="recipient">The recipient to unregister.</param>
        /// <param name="tag">The message tag.</param>
        /// <param name="ignoreTag">If set to <c>true</c>, tags are ignored.</param>
        /// <returns><c>true</c> if the handler is unregistered successfully; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="recipient"/> is <c>null</c>.</exception>
        /// <remarks>A handler cannot be unregistered when it is not registered first. If a handler is unregistered while it
        /// is not registered, this method will return <c>false</c>.</remarks>
        public bool UnregisterRecipient(object recipient, object tag, bool ignoreTag)
        {
            Argument.IsNotNull("recipient", recipient);

            lock (_lockObject)
            {
                var handlerCounter = 0;
                var keys = _registeredHandlers.Keys.ToList();
                foreach (var key in keys)
                {
                    var messageHandlers = _registeredHandlers[key];
                    for (var i = 0; i < messageHandlers.Count; i++)
                    {
                        var handlerInfo = messageHandlers[i];
                        var weakReference = (IWeakReference) handlerInfo.Action;
                        if (ignoreTag || TagHelper.AreTagsEqual(tag, handlerInfo.Tag))
                        {
                            if (ReferenceEquals(recipient, weakReference.Target))
                            {
                                messageHandlers.RemoveAt(i--);
                                handlerCounter++;

                                Log.Debug("Unregistered handler for message type '{0}' with tag '{1}'", key.Name, ObjectToStringHelper.ToString(tag));
                            }
                        }
                    }
                }

                Log.Debug("Unregistered '{0}' handlers for the recipient with tag '{1}'", handlerCounter, ObjectToStringHelper.ToString(tag));

                return true;
            }
        }

        /// <summary>
        /// Determines whether the specified recipient is registered.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="recipient">The recipient.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// 	<c>true</c> if the specified recipient is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public bool IsRegistered<TMessage>(object recipient, Action<TMessage> handler, object tag = null)
        {
            Argument.IsNotNull("handler", handler);

            lock (_lockObject)
            {
                var messageType = typeof (TMessage);

                if (_registeredHandlers.TryGetValue(messageType, out var messageHandlers))
                {
                    for (var i = messageHandlers.Count - 1; i >= 0; i--)
                    {
                        var handlerInfo = messageHandlers[i];
                        var weakAction = (IWeakAction<TMessage>) handlerInfo.Action;

                        if (!weakAction.IsTargetAlive)
                        {
                            messageHandlers.RemoveAt(i);
                            continue;
                        }

                        var target = weakAction.Target;
                        if (target != null)
                        {
                            if (!ReferenceEquals(recipient, target))
                            {
                                continue;
                            }
                        }

                        if (TagHelper.AreTagsEqual(tag, handlerInfo.Tag) && AreEqualHandlers(handler, weakAction))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the handler and the weak action are equal.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="handler">The handler to compare to the weak action.</param>
        /// <param name="weakAction">The weak action to compare to the handler.</param>
        /// <returns><c>true</c> if the handlers are equal; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="weakAction"/> is <c>null</c></exception>
        private bool AreEqualHandlers<TMessage>(Action<TMessage> handler, IWeakAction<TMessage> weakAction)
        {
            Argument.IsNotNull("handler", handler);
            Argument.IsNotNull("weakAction", weakAction);

#if NETFX_CORE || PCL
            return weakAction.Action == (Delegate)handler;
#else
            var handlerMethod = handler.Method.ToString();
            return string.CompareOrdinal(weakAction.MethodName, handlerMethod) == 0;
#endif
        }

        /// <summary>
        /// Gets all the registered handlers for the specified message type.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <returns>A list of handlers.</returns>
        internal List<IWeakAction<TMessage>> GetRegisteredHandlers<TMessage>()
        {
            lock (_lockObject)
            {
                var registeredHandlers = new List<IWeakAction<TMessage>>();

                var messageType = typeof (TMessage);

                if (_registeredHandlers.TryGetValue(messageType, out var messageHandlers))
                { 
                    for (var i = 0; i < messageHandlers.Count; i++)
                    {
                        registeredHandlers.Add((IWeakAction<TMessage>) messageHandlers[i].Action);
                    }
                }

                return registeredHandlers;
            }
        }
        #endregion
    }
}