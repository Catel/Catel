// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// Exception factory.
    /// </summary>
    public static class ExceptionFactory
    {
        /// <summary>
        /// Creates the exception with the message and inner exception. If the exception does not support creation with
        /// inner exceptions, it will use the message only.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>The created exception or <c>null</c> if there was no valid constructor available.</returns>
        public static TException CreateException<TException>(string message, Exception innerException = null)
            where TException : Exception
        {
            return (TException)CreateException(typeof (TException), message, innerException);
        }

        /// <summary>
        /// Creates the exception with the message and inner exception. If the exception does not support creation with
        /// inner exceptions, it will use the message only.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>The created exception or <c>null</c> if there was no valid constructor available.</returns>
        public static Exception CreateException(Type exceptionType, string message, Exception innerException = null)
        {
            // Try 1: with inner exception
            if (innerException != null)
            {
                var argsWithInnerException = new object[] { message, innerException };
                var exceptionWithInnerException = CreateException(exceptionType, argsWithInnerException);
                if (exceptionWithInnerException != null)
                {
                    return exceptionWithInnerException;
                }
            }

            // try 2: without inner exception
            var args = new object[] { message };
            var exception = CreateException(exceptionType, args);
            if (exception != null)
            {
                return exception;
            }

            // try 3: without anything
            exception = CreateException(exceptionType, new object[] {});
            if (exception != null)
            {
                return exception;
            }

            return null;
        }

        /// <summary>
        /// Creates the exception with the specified arguments.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>The created exception or <c>null</c> if there was no valid constructor available.</returns>
        public static TException CreateException<TException>(object[] args)
            where TException : Exception
        {
            return (TException)CreateException(typeof (TException), args);
        }

        /// <summary>
        /// Creates the exception with the specified arguments.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The created exception or <c>null</c> if there was no valid constructor available.</returns>
        public static Exception CreateException(Type exceptionType, object[] args)
        {
            try
            {
                return (Exception)Activator.CreateInstance(exceptionType, args);
            }
#if !NETFX_CORE && !PCL
            catch (MissingMethodException)
#else
            catch (Exception)
#endif
            {

            }

            return null;
        }
    }
}