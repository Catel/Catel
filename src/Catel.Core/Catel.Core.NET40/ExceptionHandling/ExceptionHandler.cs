// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// This implements the exception treatment mechanics.
    /// </summary>
    internal class ExceptionHandler : IExceptionHandler
    {
        #region Fields
        private readonly Action<Exception> _action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ExceptionHandler(Type exceptionType, Action<Exception> action)
        {
            Argument.IsNotNull("exceptionType", exceptionType);
            Argument.IsNotNull("action", action);

            Exception = exceptionType;
            _action = action;
        }
        #endregion

        #region IExceptionHandler Members
        /// <summary>
        /// Gets the exception handled.
        /// </summary>
        public Type Exception { get; private set; }

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
}