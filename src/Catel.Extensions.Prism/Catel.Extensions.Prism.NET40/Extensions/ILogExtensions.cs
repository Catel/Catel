// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    using Logging;

    using Microsoft.Practices.Prism.Logging;

    /// <summary>
    /// The <see cref="ILog"/> extensions methods.
    /// </summary>
    public static class ILogExtensions
    {
        #region Constants

        /// <summary>
        /// The priority prefix pattern.
        /// </summary>
        private const string PriorityPrefixPattern = "[{0}]";
        #endregion

        #region Methods

        /// <summary>
        /// Writes the specified message as debug message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Debug(this ILog @this, Priority priority, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Debug(string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified exception as debug message followed by the specified message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="priority">
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Debug(this ILog @this, Exception exception, Priority priority = Priority.None, string messageFormat = "", params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("exception", exception);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Debug(exception, string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as info message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="priority">
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Info(this ILog @this, Priority priority, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Info(string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified exception as info message followed by the specified message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        public static void Info(this ILog @this, Exception exception, Priority priority = Priority.None, string messageFormat = "", params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("exception", exception);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Info(exception, string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as warning message.
        /// </summary>
        /// <param name="this">
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Warning(this ILog @this, Priority priority, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Warning(string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified exception as warning message followed by the specified message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Warning(this ILog @this, Exception exception, Priority priority = Priority.None, string messageFormat = "", params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("exception", exception);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Warning(exception, string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Error(this ILog @this, Priority priority, string messageFormat, params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Error(string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        /// <summary>
        /// Writes the specified exception as error message followed by the specified message.
        /// </summary>
        /// <param name="this">
        /// The log
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <param name="priority">
        /// The priority
        /// </param>
        /// <param name="messageFormat">
        /// The message format.
        /// </param>
        /// <param name="args">
        /// The formatting arguments.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="exception"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="messageFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static void Error(this ILog @this, Exception exception, Priority priority = Priority.None, string messageFormat = "", params object[] args)
        {
            Argument.IsNotNull("@this", @this);
            Argument.IsNotNull("exception", exception);
            Argument.IsNotNull("messageFormat", messageFormat);
            @this.Error(exception, string.Format(PriorityPrefixPattern, priority)  + " " + messageFormat, args);
        }

        #endregion
    }
}