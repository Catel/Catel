// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Messaging
{
    using System;
    using IoC;

    /// <summary>
    /// Base class for messages distributed via the Catel MessageMediator subsystem. Inherit from this class
    /// to define individual message types.
    /// <para/>
    /// For most subclasses the only thing to code is an empty class body including the type parameters.
    /// <para/>
    /// For the payload data you can choose betweeen the following options:
    /// <list type="bullet">
    /// 		<item><description>The Data property provided within this base class of type TData using simple types like int or string.</description></item>
    /// 		<item><description>The Data property provided within this base class of type TData using userdefined data types.</description></item>
    /// 		<item><description>Define properties on the derived message class itself.</description></item>
    /// 		<item><description>A combination of the previous options.</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TMessage">The actual type of the message.</typeparam>
    /// <typeparam name="TData">The type of payload data to be carried with the message.</typeparam>
    public abstract class MessageBase<TMessage, TData>
        where TMessage : MessageBase<TMessage, TData>, new()
    {
        // stores a reference to the Catels MessageMediator
        private static readonly IMessageMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        static MessageBase()
        {
            var serviceLocator = IoCConfiguration.DefaultServiceLocator;
            if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
            {
                serviceLocator.RegisterInstance(MessageMediator.Default);
            }

            _mediator = serviceLocator.ResolveType<IMessageMediator>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// <para/>
        /// Necessary for two reasons:
        /// <list type="number">
        /// 		<item><description>Create an instance of the Message class via the TMessage type parameter used by the With() method.</description></item>
        /// 		<item><description>Allow derived classes to be defined using an empty class body with the implicit default constructor.</description></item>
        /// 	</list>
        /// </summary>
        protected MessageBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBase&lt;TMessage, TData&gt;"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        protected MessageBase(TData data)
        {
            Data = data;
        }

        /// <summary>
        /// Provides access to the payload data.
        /// </summary>
        public TData Data { get; protected set; }

        /// <summary>
        /// Use <see cref="SendWith">MessageClass.SendWith(data)</see> to send a new message via the mediator service.
        /// </summary>
        /// <param name="data">The payload data.</param>
        /// <param name="tag">The optional Catel mediator tag to be used.</param>
        public static void SendWith(TData data, object tag = null)
        {
            var message = With(data);
            Send(message, tag);
        }

        /// <summary>
        /// Send the message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="tag">The optional Catel mediator tag to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message"/> is <c>null</c>.</exception>
        protected static void Send(TMessage message, object tag = null)
        {
            Argument.IsNotNull("message", message);

            _mediator.SendMessage(message, tag);
        }

        /// <summary>
        /// Convenient helper method to subscribe to this Message type.
        /// <para/>
        /// Usage:
        /// <list type="bullet">
        /// 		<item><description>MessageClass.Register(this, msg =&gt; Handler) if the handler has the signature void Handler(MessageClass message)</description></item>
        /// 		<item><description>MessageClass.Register(this, msg =&gt; Handler(msg.Data)) if the handler has the signature void Handler(TData data)</description></item>
        /// 	</list>
        /// </summary>
        /// <param name="recipient">The instance which registers to the messages. Is most cases this will be <c>this</c>.</param>
        /// <param name="handler">A delegate handling the incoming message. For example: msg =&gt; Handler(msg.Data).</param>
        /// <param name="tag">The optional Catel mediator tag to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void Register(object recipient, Action<TMessage> handler, object tag = null)
        {
            Argument.IsNotNull("handler", handler);

            _mediator.Register(recipient, handler, tag);
        }

        /// <summary>
        /// Convenient helper method to unsubscribe from this Message type.
        /// <para/>
        /// Usage:
        /// <list type="bullet">
        /// 		<item><description>MessageClass.Register(this, msg =&gt; Handler) if the handler has the signature void Handler(MessageClass message)</description></item>
        /// 		<item><description>MessageClass.Register(this, msg =&gt; Handler(msg.Data)) if the handler has the signature void Handler(TData data)</description></item>
        /// 	</list>
        /// </summary>
        /// <param name="recipient">The instance which unregisters from the messages. Is most cases this will be <c>this</c>.</param>
        /// <param name="handler">A delegate handling the incoming message. For example: msg =&gt; Handler(msg.Data).</param>
        /// <param name="tag">The optional Catel mediator tag to be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void Unregister(object recipient, Action<TMessage> handler, object tag = null)
        {
            Argument.IsNotNull("handler", handler);

            _mediator.Unregister(recipient, handler, tag);
        }

        /// <summary>
        /// Returns an instance of the MessageClass populated with payload Data.<br/>
        /// <para />
        /// Most times used internally by the <see cref="SendWith"/> method.
        /// </summary>
        /// <param name="data">The payload data.</param>
        /// <returns>An instance of the MessageClass populated with the given payload data.</returns>
        public static TMessage With(TData data)
        {
            return new TMessage
            {
                Data = data
            };
        }
    }
}
