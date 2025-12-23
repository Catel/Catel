namespace Catel.Logging
{
    using System;
    using Catel.Data;
    using Microsoft.Extensions.Logging;
    using Reflection;

    /// <summary>
    /// Extensions to the <see cref="ILogger" /> interface.
    /// </summary>
    public static partial class ILoggerExtensions
    {
        /// <summary>
        /// Exception data key used to indicates whether the exception was already processed by Catel log system.
        /// </summary>
        private const string AlreadyProcessedByCatelLogSystemExceptionDataKey = "AlreadyProcessedByCatelLogSystem";

        /// <summary>
        /// Logs the product info with version information.
        /// </summary>
        /// <param name="logger">The log.</param>
        public static void LogProductInfo(this ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            
            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "**************************************************************************");
            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "PRODUCT INFO");
            logger.Log(LogLevel.Information, string.Empty);

            var assembly = AssemblyHelper.GetEntryAssembly();
            if (assembly is not null)
            {
                logger.Log(LogLevel.Information, "Assembly:              {0}", assembly.Title() ?? string.Empty);
                logger.Log(LogLevel.Information, "Version:               {0}", assembly.Version());

                try
                {
                    logger.Log(LogLevel.Information, "Informational version: {0}", assembly.InformationalVersion() ?? string.Empty);
                }
                catch (Exception)
                {
                    // Ignore
                }

                logger.Log(LogLevel.Information, string.Empty);
                logger.Log(LogLevel.Information, "Company:               {0}", assembly.Company() ?? string.Empty);
                logger.Log(LogLevel.Information, "Copyright:             {0}", assembly.Copyright() ?? string.Empty);
            }

            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "**************************************************************************");
            logger.Log(LogLevel.Information, string.Empty);
        }

        /// <summary>
        /// Logs the device info.
        /// </summary>
        /// <param name="logger">The log.</param>
        public static void LogDeviceInfo(this ILogger logger)
        {
            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "**************************************************************************");
            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "DEVICE INFO");
            logger.Log(LogLevel.Information, string.Empty);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            logger.Log(LogLevel.Information, "Platform:              {0}", Environment.OSVersion.Platform);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            logger.Log(LogLevel.Information, "OS Version:            {0}", Environment.OSVersion.Version);

            logger.Log(LogLevel.Information, "64-bit OS:             {0}", Environment.Is64BitOperatingSystem.ToString());
            logger.Log(LogLevel.Information, "64-bit process:        {0}", Environment.Is64BitProcess.ToString());
            logger.Log(LogLevel.Information, "Processor count:       {0}", Environment.ProcessorCount.ToString());
            logger.Log(LogLevel.Information, "System page size:      {0}", Environment.SystemPageSize.ToString());

            logger.Log(LogLevel.Information, string.Empty);
            logger.Log(LogLevel.Information, "**************************************************************************");
            logger.Log(LogLevel.Information, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, string messageFormat, object? arg1)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, (Exception?)null, string.Format(messageFormat, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <param name="arg2">Formatting argument 2.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, string messageFormat, object? arg1, object? arg2)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, (Exception?)null, string.Format(messageFormat, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Formatting argument 1.</param>
        /// <param name="arg2">Formatting argument 2.</param>
        /// <param name="arg3">Formatting argument 3.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, string messageFormat, object? arg1, object? arg2, object arg3)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, (Exception?)null, string.Format(messageFormat, arg1, arg2, arg3));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, string messageFormat, params object[] args)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, (Exception?)null, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, string.Format(messageFormat, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, string.Format(messageFormat, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Func<string, TException> createExceptionCallback, string messageFormat, object? arg1, object? arg2, object? arg3)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, string.Format(messageFormat, arg1, arg2, arg3));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Exception innerException, string messageFormat, object? arg0)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, innerException, string.Format(messageFormat, arg0));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <param name="arg1">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Exception innerException, string messageFormat, object? arg0, object? arg1)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, innerException, string.Format(messageFormat, arg0, arg1));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="arg0">The formatting argument.</param>
        /// <param name="arg1">The formatting argument.</param>
        /// <param name="arg2">The formatting argument.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Exception innerException, string messageFormat, object? arg0, object? arg1, object? arg2)
            where TException : Exception
        {
            return LogErrorAndCreateException<TException>(logger, innerException, string.Format(messageFormat, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException" /> does not have a constructor accepting a string.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Exception? innerException, string messageFormat, params object?[] args)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(logger);

            return LogErrorAndCreateException<TException>(logger, innerException, msg =>
            {
                var exception = ExceptionFactory.CreateException<TException>(msg, innerException);
                if (exception is null)
                {
                    var error = $"Exception type '{typeof(TException).Name}' does not have a constructor accepting a string";

                    logger.LogError(error);

                    throw new NotSupportedException(error);
                }

                return exception;
            }, messageFormat, args);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para />
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="logger">The log.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="createExceptionCallback">The create exception callback.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <returns>Exception.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para /><![CDATA[
        /// throw Log.ErrorAndCreateException<NotSupportedException>("This action is not supported");
        /// ]]></code>
        /// </example>
        public static Exception LogErrorAndCreateException<TException>(this ILogger logger, Exception? innerException, Func<string, TException> createExceptionCallback, string messageFormat, params object?[] args)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(logger);

            var message = messageFormat ?? string.Empty;
            if (args is not null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            var exception = createExceptionCallback(message);
            if (exception is null)
            {
                var error = $"Exception type '{typeof(TException).Name}' does not have a constructor accepting a string";

                logger.LogError(error);

                throw new NotSupportedException(error);
            }

            return exception;
        }

        /// <summary>
        /// Formats the exception for logging with an additional message.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <returns>Formatted string.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        private static string FormatException(Exception exception, string message)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var formattedException = $"[{exception.GetType().Name}] {exception}";

            if (string.IsNullOrEmpty(message))
            {
                return formattedException;
            }

            return $"{message} | {formattedException}";
        }
    }
}
