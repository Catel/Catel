// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml.Linq;

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
            while (ex is not null)
            {
                if (ex is OutOfMemoryException ||
                    ex is BadImageFormatException || 
                    ex is AppDomainUnloadedException ||
                    ex is CannotUnloadAppDomainException ||
                    ex is InvalidProgramException ||
                    ex is ThreadAbortException ||
                    ex is StackOverflowException
                    )
                {
                    return true;
                }

                ex = ex.InnerException;
            }

            return false;
        }

        #region Methods
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
            Argument.IsNotNull("exception", exception);

            var stringBuilder = new StringBuilder(message);

            var currentException = exception;
            while (!ObjectHelper.IsNull(currentException))
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
            Argument.IsNotNull("exception", exception);

            var exceptions = new List<Exception>();

            while (!ObjectHelper.IsNull((exception = exception.InnerException)))
            {
                exceptions.Add(exception);
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
        public static TException Find<TException>(this Exception exception)
            where TException : Exception
        {
            Argument.IsNotNull("exception", exception);

            while (!ObjectHelper.IsNull(exception) && !(exception is TException))
            {
                exception = exception.InnerException;
            }

            return (TException)exception;
        }

        /// <summary>
        /// Returns the Exception message as XML document.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>An XDocument of the Exception object.</returns>
        /// <exception cref="ArgumentNullException">The <param ref="exception"> is <c>null</c>.</param></exception>
        public static XDocument ToXml(this Exception exception)
        {
            Argument.IsNotNull("exception", exception);

            var root = new XElement(exception.GetType().ToString());

            if (!string.IsNullOrEmpty(exception.Message))
            {
                root.Add(new XElement("Message", exception.Message));
            }

            if (!ObjectHelper.IsNull(exception.StackTrace))
            {
                root.Add
                (
                    new XElement("StackTrace",
                        from frame in exception.StackTrace.Split(new[] { '\n' })
                        let prettierFrame = frame.Substring(6).Trim()
                        select new XElement("Frame", prettierFrame))
                );
            }

            if (exception.Data.Count > 0)
            {
                root.Add
                (
                    new XElement("Data",
                        from entry in exception.Data.Cast<DictionaryEntry>()
                        let key = entry.Key.ToString()
                        let value = (ObjectHelper.IsNull(entry.Value)) ? "null" : entry.Value.ToString()
                        select new XElement(key, value))
                );
            }

            if (!ObjectHelper.IsNull(exception.InnerException))
            {
                root.Add
                (
                    exception.InnerException.ToXml()
                );
            }

            return new XDocument(root);
        }
        #endregion
    }
}
