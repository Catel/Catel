// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallbackBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the call back base class.
    /// </summary>
    public abstract class CallbackBase : Callback
    {
        #region Fields
        /// <summary>
        ///     The callback
        /// </summary>
        private readonly Action<IInvocation> _callback;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="CallbackBase" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        protected CallbackBase(Action callback)
            : base(callback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackBase"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        protected CallbackBase(Action<IInvocation> callback)
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
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        public virtual void Intercept(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            if (_callback != null)
            {
                _callback(invocation);
            }
            else
            {
                Action();
            }
        }
        #endregion
    }
}