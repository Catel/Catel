// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnFinallyCallback.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the on finally call back implementation.
    /// </summary>
    public class OnFinallyCallback : CallbackBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnFinallyCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnFinallyCallback(Action callback)
            : base(callback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnFinallyCallback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public OnFinallyCallback(Action<IInvocation> callback)
            : base(callback)
        {
        }
        #endregion

        #region Methods
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