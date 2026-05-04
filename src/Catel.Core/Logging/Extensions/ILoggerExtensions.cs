namespace Catel.Logging;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Reflection;

/// <summary>
/// Extensions to the <see cref="ILogger" /> interface.
/// </summary>
public static partial class ILoggerExtensions
{
    private static readonly bool IsDebuggerAttached;

    static ILoggerExtensions()
    {
        IsDebuggerAttached = Debugger.IsAttached;
    }

    public static void LogDebugIfAttached(this ILogger log, string message)
    {
        if (IsDebuggerAttached)
        {
            log.LogDebug(message);
        }
    }

    public static void LogDebugIfAttached(this ILogger log, string message, params object?[] args)
    {
        if (IsDebuggerAttached)
        {
            log.LogDebug(message, args);
        }
    }

    /// <summary>
    /// Logs the product info with version information.
    /// </summary>
    /// <param name="logger">The log.</param>
    public static void LogProductInfo(this ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);


        logger.LogInformation(string.Empty);
        logger.LogInformation("**************************************************************************");
        logger.LogInformation(string.Empty);
        logger.LogInformation("PRODUCT INFO");
        logger.LogInformation(string.Empty);

        var assembly = AssemblyHelper.GetEntryAssembly();
        if (assembly is not null)
        {
            logger.LogInformation("Assembly:              {Assembly}", assembly.Title() ?? string.Empty);
            logger.LogInformation("Version:               {Version}", assembly.Version());

            try
            {
                logger.LogInformation("Informational version: {InformationalVersion}", assembly.InformationalVersion() ?? string.Empty);
            }
            catch (Exception)
            {
                // Ignore
            }

            logger.LogInformation(string.Empty);
            logger.LogInformation("Company:               {Company}", assembly.Company() ?? string.Empty);
            logger.LogInformation("Copyright:             {Copyright}", assembly.Copyright() ?? string.Empty);
        }

        logger.LogInformation(string.Empty);
        logger.LogInformation("**************************************************************************");
        logger.LogInformation(string.Empty);
    }

    /// <summary>
    /// Logs the device info.
    /// </summary>
    /// <param name="logger">The log.</param>
    public static void LogDeviceInfo(this ILogger logger)
    {
        logger.LogInformation(string.Empty);
        logger.LogInformation("**************************************************************************");
        logger.LogInformation(string.Empty);
        logger.LogInformation("DEVICE INFO");
        logger.LogInformation(string.Empty);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
        logger.LogInformation("Platform:              {Platform}", Environment.OSVersion.Platform);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        logger.LogInformation("OS Version:            {OsVersion}", Environment.OSVersion.Version);

        logger.LogInformation("64-bit OS:             {Is64BitOs}", Environment.Is64BitOperatingSystem.ToString());
        logger.LogInformation("64-bit process:        {Is64BitProcess}", Environment.Is64BitProcess.ToString());
        logger.LogInformation("Processor count:       {ProcessorCount}", Environment.ProcessorCount.ToString());
        logger.LogInformation("System page size:      {SystemPageSize}", Environment.SystemPageSize.ToString());

        logger.LogInformation(string.Empty);
        logger.LogInformation("**************************************************************************");
        logger.LogInformation(string.Empty);
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
        return LogErrorAndCreateException<TException>(logger, (Exception?)null, messageFormat, [arg1]);
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
        return LogErrorAndCreateException<TException>(logger, (Exception?)null, messageFormat, [arg1, arg2]);
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
        return LogErrorAndCreateException<TException>(logger, (Exception?)null, messageFormat, [arg1, arg2, arg3]);
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
        return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, messageFormat, [arg1]);
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
        return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, messageFormat, [arg1, arg2]);
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
        return LogErrorAndCreateException<TException>(logger, null, createExceptionCallback, messageFormat, [arg1, arg2, arg3]);
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
        return LogErrorAndCreateException<TException>(logger, innerException, messageFormat, [arg0]);
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
        return LogErrorAndCreateException<TException>(logger, innerException, messageFormat, [arg0, arg1]);
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
        return LogErrorAndCreateException<TException>(logger, innerException, messageFormat, [arg0, arg1, arg2]);
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
            if (message.Contains("{0}"))
            {
                message = string.Format(message, args);
            }
            else
            {
                var indexCounter = 0;

                var renderedMessage = LogMessageRegEx().Replace(message, m =>
                {
                    return args[indexCounter++]?.ToString() ?? "null";
                });

                message = renderedMessage;
            }
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

    [GeneratedRegex("{(.*?)}")]
    private static partial Regex LogMessageRegEx();
}
