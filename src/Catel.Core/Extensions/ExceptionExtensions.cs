namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Determines whether the specified exception is critical (meaning the application should shut down).
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns><c>true</c> if the specified exception is critical; otherwise, <c>false</c>.</returns>
        public static bool IsCritical(this Exception ex)
        {
            var currentException = ex;
            while (currentException is not null)
            {
                if (currentException is OutOfMemoryException ||
                    currentException is BadImageFormatException || 
                    currentException is AppDomainUnloadedException ||
                    currentException is CannotUnloadAppDomainException ||
                    currentException is InvalidProgramException ||
                    currentException is ThreadAbortException ||
                    currentException is StackOverflowException)
                {
                    return true;
                }

                currentException = currentException.InnerException;
            }

            return false;
        }

        /// <summary>
        /// Flattens the specified exception and inner exception data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> include stack trace.</param>
        /// <returns>The flatten message.</returns>
        /// <exception cref="ArgumentNullException">The <param ref="exception"> is <c>null</c>.</param></exception>
        public static string Flatten(this Exception exception, string message = "", bool includeStackTrace = false)
        {
            var stringBuilder = new StringBuilder(message);

            var currentException = exception;
            while (currentException is not null)
            {
                stringBuilder.AppendLine(currentException.Message);
                if (includeStackTrace)
                {
                    stringBuilder.Append(exception.StackTrace);
                }

                currentException = currentException.InnerException;
                if (includeStackTrace)
                {
                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets all inner exceptions.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The inner exceptions collection.</returns>
        /// <exception cref="ArgumentNullException">The <param ref="exception"> is <c>null</c>.</param></exception>
        public static IEnumerable<Exception> GetAllInnerExceptions(this Exception exception)
        {
            var exceptions = new List<Exception>();

            var processingException = exception;
            while (processingException is not null)
            {
                exceptions.Add(processingException);

                processingException = processingException.InnerException;
            }

            return exceptions.ToArray();
        }

        /// <summary>
        /// Finds the specified exception in all inner exceptions.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns>The found exception.</returns>
        /// <exception cref="ArgumentNullException">The <param ref="exception"> is <c>null</c>.</param></exception>
        public static TException? Find<TException>(this Exception exception)
            where TException : Exception
        {
            Exception? foundException = exception;

            while (foundException is not null && !(foundException is TException))
            {
                foundException = foundException.InnerException;
            }

            return foundException as TException;
        }
    }
}
