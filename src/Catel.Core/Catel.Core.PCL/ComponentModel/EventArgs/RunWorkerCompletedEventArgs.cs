// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunWorkerCompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace System.ComponentModel
{
    using System.Reflection;

    /// <summary>
    /// Provides data for the <see cref="BackgroundWorker.RunWorkerCompleted"/> event.
    /// </summary>
    public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
    {
        #region Fields
        private readonly object _result;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RunWorkerCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="result">The result of an asynchronous operation.</param>
        /// <param name="error">Any error that occurred during the asynchronous operation.</param>
        /// <param name="cancelled">A value indicating whether the asynchronous operation was cancelled.</param>
        public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
            : base(error, cancelled, null)
        {
            _result = result;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value that represents the result of an asynchronous operation.
        /// </summary>
        /// <value>The result.</value>
        /// <exception cref="TargetInvocationException"><see cref="AsyncCompletedEventArgs.Error" /> is not <see langword="null" />. The <see cref="Exception.InnerException" /> property holds a reference to <see cref="AsyncCompletedEventArgs.Error" />.</exception>
        /// <exception cref="InvalidOperationException"><see cref="AsyncCompletedEventArgs.Cancelled" /> is <see langword="true" />.</exception>
        public object Result
        {
            get
            {
                RaiseExceptionIfNecessary();

                return _result;
            }
        }

        /// <summary>
        /// Gets a value that represents the user state.
        /// </summary>
        /// <value>An <see cref="object" /> representing the user state.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new object UserState
        {
            get { return base.UserState; }
        }
        #endregion
    }
}