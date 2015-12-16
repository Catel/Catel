// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// This implements the exception treatment mechanics.
    /// </summary>
    public class ExceptionHandler : IExceptionHandler
    {
        #region Fields
        private readonly Action<Exception> _action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler" /> class.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public ExceptionHandler(Type exceptionType, Action<Exception> action)
        {
            Argument.IsNotNull("exceptionType", exceptionType);
            Argument.IsNotNull("action", action);

            ExceptionType = exceptionType;
            _action = action;
        }
        #endregion

        #region IExceptionHandler Members
        /// <summary>
        /// Gets the type of the handled exception.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets or sets the buffer policy.
        /// </summary>
        /// <value>
        /// The buffer policy.
        /// </value>
        public IBufferPolicy BufferPolicy { get; set; }

        /// <summary>
        /// Gets or sets the retry policy.
        /// </summary>
        /// <value>
        /// The retry policy.
        /// </value>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Handles the exception using the action that was passed into the constructor.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public void Handle(Exception exception)
        {
            Argument.IsNotNull("exception", exception);

            _action(exception);
        }
        #endregion
    }

    /// <summary>
    /// This implements the exception treatment mechanics.
    /// </summary>
    /// <typeparam name="TException">The exception type.</typeparam>
    public abstract class ExceptionHandler<TException> : IExceptionHandler<TException> where TException : Exception
    {
        #region Fields
        private Action<Exception> _action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler" /> class.
        /// </summary>
        protected ExceptionHandler()
        {
            ExceptionType = typeof(TException);
        }
        #endregion

        #region IExceptionHandler Members
        /// <summary>
        /// The action to do on an exception of defined type occurs.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public abstract void OnException(TException exception);


        /// <summary>
        /// Gets the type of the handled exception.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets or sets the buffer policy.
        /// </summary>
        /// <value>
        /// The buffer policy.
        /// </value>
        public IBufferPolicy BufferPolicy { get; set; }

        /// <summary>
        /// Gets or sets the retry policy.
        /// </summary>
        /// <value>
        /// The retry policy.
        /// </value>
        public IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Handles the exception using the action that was defined.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        public void Handle(Exception exception)
        {
            Argument.IsNotNull("exception", exception);

            _action = ex => OnException((TException)exception);

            _action(exception);
        }
        #endregion
    }
}