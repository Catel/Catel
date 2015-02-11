// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyInitializationException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception throwed when the <see cref="ProxyFactory"/> class can not create a proxy type.
    /// </summary>
    [Serializable]
    public class ProxyInitializationException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInitializationException" /> class.
        /// </summary>
        public ProxyInitializationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInitializationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ProxyInitializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInitializationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ProxyInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyInitializationException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ProxyInitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }
}