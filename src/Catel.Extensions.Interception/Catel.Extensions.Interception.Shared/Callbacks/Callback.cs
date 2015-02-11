// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Callback.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// Represents the call back implementation.
    /// </summary>
    public abstract class Callback
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Callback" /> class.
        /// </summary>
        protected Callback()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Callback"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        protected Callback(Action callback)
        {
            Argument.IsNotNull("callback", callback);

            Action = callback;
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the action.
        /// </summary>
        /// <value>
        ///     The action.
        /// </value>
        protected Action Action { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        ///     Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(ICallbackVisitor visitor);
        #endregion
    }
}