// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnReturnCallback.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the on return call back implementation.
    /// </summary>
    public class OnReturnCallback : Callback
    {
        #region Fields
        /// <summary>
        /// The callback
        /// </summary>
        private readonly Func<IInvocation, object, object> _callback;

        /// <summary>
        /// The callback with return
        /// </summary>
        private readonly Func<object> _callbackWithReturn;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnReturnCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnReturnCallback(Func<object> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callbackWithReturn = callback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnReturnCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnReturnCallback(Func<IInvocation, object, object> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callback = callback;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        public object Intercept(IInvocation invocation, object result)
        {
            Argument.IsNotNull("invocation", invocation);

            return !ObjectHelper.IsNull(_callback) ? _callback(invocation, result) : _callbackWithReturn();
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