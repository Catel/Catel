// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplashScreenException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;

    /// <summary>
    /// Class SplashScreenException.
    /// </summary>
    public class SplashScreenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SplashScreenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public SplashScreenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}