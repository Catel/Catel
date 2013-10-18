// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICallbackVisitor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;

    /// <summary>
    /// The call back visitor interface.
    /// </summary>
    public interface ICallbackVisitor
    {
        #region Methods
        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnBeforeCallback callback);

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnInvokeCallback callback);

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnCatchCallback callback);

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnFinallyCallback callback);

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnAfterCallback callback);

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        void Visit(OnReturnCallback callback);
        #endregion
    }
}