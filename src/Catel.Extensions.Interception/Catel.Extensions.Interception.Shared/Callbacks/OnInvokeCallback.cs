// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnInvokeCallback.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the on invoke call back implementation.
    /// </summary>
    public class OnInvokeCallback : Callback
    {
        #region Fields
        /// <summary>
        /// The callback
        /// </summary>
        private readonly Action<IInvocation> _callback;

        /// <summary>
        /// The callback with return
        /// </summary>
        private readonly Func<IInvocation, object> _callbackWithReturn;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnInvokeCallback" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnInvokeCallback(Action<IInvocation> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callback = callback;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnInvokeCallback" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnInvokeCallback(Func<IInvocation, object> callback)
        {
            Argument.IsNotNull("callback", callback);

            _callbackWithReturn = callback;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        public object Intercept(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            if (!ObjectHelper.IsNull(_callbackWithReturn))
            {
                return _callbackWithReturn(invocation);
            }
            _callback(invocation);
            return null;
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