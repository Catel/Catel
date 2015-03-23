// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;
    using Reflection;

    /// <summary>
    /// Extensions to the <see cref="ILog" /> interface.
    /// </summary>
    public static partial class LogExtensions
    {
        /// <summary>
        /// Logs the product info with version information.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void LogProductInfo(this ILog log)
        {
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "PRODUCT INFO");
            Write(log, LogEvent.Info, string.Empty);

#if NETFX_CORE
            var appPackage = Windows.ApplicationModel.Package.Current;
            var packageId = appPackage.Id;

            Write(log, LogEvent.Info, "Package full name:     {0}", packageId.FullName);
            Write(log, LogEvent.Info, "Package name:          {0}", packageId.Name);
            Write(log, LogEvent.Info, "Package family name:   {0}", packageId.FamilyName);
            Write(log, LogEvent.Info, "Publisher:             {0}", packageId.Publisher);
            Write(log, LogEvent.Info, "Publisher Id:          {0}", packageId.PublisherId);
            Write(log, LogEvent.Info, "Version:               {0}", packageId.Version);
#else
            var assembly = AssemblyHelper.GetEntryAssembly();
            Write(log, LogEvent.Info, "Assembly:              {0}", assembly.Title());
            Write(log, LogEvent.Info, "Version:               {0}", assembly.Version());
            //Write(log, LogEvent.Info, "Informational version: {0}", assembly.InformationalVersion());
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "Company:               {0}", assembly.Company());
            Write(log, LogEvent.Info, "Copyright:             {0}", assembly.Copyright());
#endif
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
        }

        /// <summary>
        /// Logs the device info.
        /// </summary>
        /// <param name="log">The log.</param>
        public static void LogDeviceInfo(this ILog log)
        {
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "DEVICE INFO");
            Write(log, LogEvent.Info, string.Empty);

#if !PCL && !NETFX_CORE
            Write(log, LogEvent.Info, "Platform:              {0}", Environment.OSVersion.Platform);
            Write(log, LogEvent.Info, "OS Version:            {0}", Environment.OSVersion.Version);
#endif

#if NET
            Write(log, LogEvent.Info, "64-bit OS:             {0}", Environment.Is64BitOperatingSystem);
            Write(log, LogEvent.Info, "64-bit process:        {0}", Environment.Is64BitProcess);
            Write(log, LogEvent.Info, "Processor count:       {0}", Environment.ProcessorCount);
            Write(log, LogEvent.Info, "System page size:      {0}", Environment.SystemPageSize);
#endif

#if WINDOWS_PHONE && !NETFX_CORE
            Write(log, LogEvent.Info, "Device name:           {0}", Microsoft.Phone.Info.DeviceStatus.DeviceName);
            Write(log, LogEvent.Info, "Device ID:             {0}", Windows.Phone.System.Analytics.HostInformation.PublisherHostId);
#endif

#if NETFX_CORE
            var appPackage = Windows.ApplicationModel.Package.Current;
            var packageId = appPackage.Id;

            Write(log, LogEvent.Info, "Architecture:          {0}", packageId.Architecture);
#endif

            Write(log, LogEvent.Info, string.Empty);
            Write(log, LogEvent.Info, "**************************************************************************");
            Write(log, LogEvent.Info, string.Empty);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void Write(this ILog log, LogEvent logEvent, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (log == null)
            {
                return;
            }

            var message = messageFormat ?? string.Empty;
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            log.WriteWithData(message, null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as the specified log event.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void Write(this ILog log, LogEvent logEvent, Exception exception, string messageFormat, params object[] args)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            Argument.IsNotNull("exception", exception);

            if (log == null)
            {
                return;
            }

            string message = messageFormat ?? string.Empty;
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            log.WriteWithData(exception, message, null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        public static void WriteWithData(this ILog log, Exception exception, string message, object extraData, LogEvent logEvent)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && log.IsCatelLogging)
            {
                return;
            }

            log.WriteWithData(FormatException(exception, message), extraData, logEvent);
        }

        /// <summary>
        /// Writes the specified message as error message and then throws the specified exception.
        /// <para/>
        /// The specified exception must have a constructor that accepts a single string as message.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="log">The log.</param>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The args.</param>
        /// <example>
        ///   <code>
        /// This example logs an error and immediately throws the exception:<para/>
        ///   <![CDATA[
        /// Log.ErrorAndThrowException<NotSupportedException>("This action is not supported");
        /// ]]>
        ///   </code>
        ///   </example>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <typeparamref name="TException"/> does not have a constructor accepting a string.</exception>
        public static void ErrorAndThrowException<TException>(this ILog log, string messageFormat, params object[] args)
            where TException : Exception
        {
            if (log == null)
            {
                return;
            }

            var message = messageFormat ?? string.Empty;
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            log.ErrorWithData(message);

            Exception exception;

            try
            {
                exception = (Exception)Activator.CreateInstance(typeof(TException), message);
            }
#if !NETFX_CORE && !PCL
            catch (MissingMethodException)
#else
            catch (Exception)
#endif
            {
                var error = string.Format("Exception type '{0}' does not have a constructor accepting a string", typeof(TException).Name);
                log.Error(error);
                throw new NotSupportedException(error);
            }

            throw exception;
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
            Argument.IsNotNull("exception", exception);

            var formattedException = string.Format("[{0}] {1}", exception.GetType().Name, exception);
            if (string.IsNullOrEmpty(message))
            {
                return formattedException;
            }

            return string.Format("{0} | {1}", message, formattedException);
        }
    }
}