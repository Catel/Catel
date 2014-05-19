// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectCallbackVisitor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Callbacks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the select call back visitor implementation.
    /// </summary>
    public class SelectCallbackVisitor : ICallbackVisitor
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="SelectCallbackVisitor" /> class.
        /// </summary>
        public SelectCallbackVisitor()
        {
            OnReturnCallbackCollection = new List<OnReturnCallback>();
            OnAfterCallbackCollection = new List<OnAfterCallback>();
            OnFinallyCallbackCollection = new List<OnFinallyCallback>();
            OnCatchCallbackCollection = new List<OnCatchCallback>();
            OnInvokeCallbackCollection = new List<OnInvokeCallback>();
            OnBeforeCallbackCollection = new List<OnBeforeCallback>();
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the on before callback collection.
        /// </summary>
        /// <value>
        ///     The on before callback collection.
        /// </value>
        public IList<OnBeforeCallback> OnBeforeCallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the on invoke callback collection.
        /// </summary>
        /// <value>
        ///     The on invoke callback collection.
        /// </value>
        public IList<OnInvokeCallback> OnInvokeCallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the on catch callback collection.
        /// </summary>
        /// <value>
        ///     The on catch callback collection.
        /// </value>
        public IList<OnCatchCallback> OnCatchCallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the on finally callback collection.
        /// </summary>
        /// <value>
        ///     The on finally callback collection.
        /// </value>
        public IList<OnFinallyCallback> OnFinallyCallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the on after callback collection.
        /// </summary>
        /// <value>
        ///     The on after callback collection.
        /// </value>
        public IList<OnAfterCallback> OnAfterCallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the on return callback collection.
        /// </summary>
        /// <value>
        ///     The on return callback collection.
        /// </value>
        public IList<OnReturnCallback> OnReturnCallbackCollection { get; private set; }
        #endregion

        #region ICallbackVisitor Members
        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnBeforeCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnBeforeCallbackCollection.Add(callback);
        }

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnInvokeCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnInvokeCallbackCollection.Add(callback);
        }

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnCatchCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnCatchCallbackCollection.Add(callback);
        }

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnFinallyCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnFinallyCallbackCollection.Add(callback);
        }

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnAfterCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnAfterCallbackCollection.Add(callback);
        }

        /// <summary>
        /// Visits the specified callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        public void Visit(OnReturnCallback callback)
        {
            Argument.IsNotNull("callback", callback);

            OnReturnCallbackCollection.Add(callback);
        }
        #endregion
    }
}