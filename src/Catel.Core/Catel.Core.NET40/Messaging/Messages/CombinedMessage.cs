// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedMessage.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Messaging
{
    using System;

    /// <summary>
    /// Implements a message transferring a boolean value and a custom property.
    /// </summary>
    public class CombinedMessage : MessageBase<CombinedMessage, bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedMessage"/> class.
        /// </summary>
        /// <remarks>
        /// Required by the base class.
        /// </remarks>
        public CombinedMessage()
        {
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CombinedMessage"/> class from being created.
        /// </summary>
        /// <param name="data">The boolean data.</param>
        /// <param name="exception">The exception.</param>
        private CombinedMessage(bool data, Exception exception)
            : base(data)
        {
            Exception = exception;
        }

        /// <summary>
        /// Send a <see cref="CombinedMessage"/> with the given payload data.
        /// </summary>
        /// <param name="data">The boolean payload Data to be sent with.</param>
        /// <param name="exception">The exception payload Data to be sent with.</param>
        /// <param name="tag">The optional Catel mediator tag to be used.</param>
        public static void SendWith(bool data, Exception exception, object tag = null)
        {
            var message = new CombinedMessage(data, exception);
            Send(message, tag);
        }

        /// <summary>
        /// Provides access to the additional exception payload data of the message.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}