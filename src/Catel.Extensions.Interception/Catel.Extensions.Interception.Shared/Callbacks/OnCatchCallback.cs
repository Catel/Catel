// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnCatchCallback.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the on catch call back implementation.
    /// </summary>
    public class OnCatchCallback : Callback
    {
        #region Fields
        /// <summary>
        ///     The callback
        /// </summary>
        private readonly Action<IInvocation, Exception> _callback;

        /// <summary>
        ///     The callback with exception
        /// </summary>
        private readonly Action<Exception> _callbackWithException;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="OnCatchCallback" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnCatchCallback(Action<Exception> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callbackWithException = callback;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnCatchCallback" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="callback" /> is <c>null</c>.
        /// </exception>
        public OnCatchCallback(Action<IInvocation, Exception> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callback = callback;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="invocation" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="exception" /> is <c>null</c>.
        /// </exception>
        public void Intercept(IInvocation invocation, Exception exception)
        {
            Argument.IsNotNull("invocation", invocation);
            Argument.IsNotNull("exception", exception);

            if (_callback != null)
            {
                _callback(invocation, exception);
            }
            else
            {
                _callbackWithException(exception);
            }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="visitor"/> is <c>null</c>.</exception>
        public override void Accept(ICallbackVisitor visitor)
        {
            Argument.IsNotNull("visitor", visitor);

            visitor.Visit(this);
        }
        #endregion
    }
}