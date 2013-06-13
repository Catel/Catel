// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExceptionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Interface that describes a single Exception handler.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Gets the exception handled.
        /// </summary>
        Type Exception { get; }

        /// <summary>
        /// Handles the exception using the action that was passed into the constructor.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        void Handle(Exception exception);
    }
}