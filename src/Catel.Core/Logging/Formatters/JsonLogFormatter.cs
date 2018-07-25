// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonLogFormatter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Reflection;

    /// <summary>
    /// The formatter which formats all log info to Json.
    /// </summary>
    public class JsonLogFormatter : IJsonLogFormatter
    {
        #region Constants
        /// <summary>
        /// The literal writes.
        /// </summary>
        protected static readonly IDictionary<Type, Action<object, bool, TextWriter>> LiteralWriters;

        /// <summary>
        /// The log event strings.
        /// </summary>
        protected static readonly Dictionary<LogEvent, string> LogEventStrings;

        private static readonly string ApplicationName;
        private static readonly string ApplicationVersion;
        #endregion

        #region Constructors
        static JsonLogFormatter()
        {
            LiteralWriters = new Dictionary<Type, Action<object, bool, TextWriter>>
            {
                {typeof (bool), (v, q, w) => WriteBoolean((bool) v, w)},
                {typeof (char), (v, q, w) => WriteString(((char) v).ToString(CultureInfo.InvariantCulture), w)},
                {typeof (byte), WriteToString},
                {typeof (sbyte), WriteToString},
                {typeof (short), WriteToString},
                {typeof (ushort), WriteToString},
                {typeof (int), WriteToString},
                {typeof (uint), WriteToString},
                {typeof (long), WriteToString},
                {typeof (ulong), WriteToString},
                {typeof (float), WriteToString},
                {typeof (double), WriteToString},
                {typeof (decimal), WriteToString},
                {typeof (string), (v, q, w) => WriteString(v, w)},
                {typeof (DateTime), (v, q, w) => WriteDateTime((DateTime) v, w)},
                {typeof (DateTimeOffset), (v, q, w) => WriteOffset((DateTimeOffset) v, w)},
            };

            LogEventStrings = new Dictionary<LogEvent, string>
            {
                {LogEvent.Debug, "Debug"},
                {LogEvent.Info, "Information"},
                {LogEvent.Warning, "Warning"},
                {LogEvent.Error, "Error"}
            };

            var assembly = AssemblyHelper.GetEntryAssembly();

            ApplicationName = assembly.Title();
            ApplicationVersion = assembly.Version();
        }
        #endregion

        #region IJsonLogFormatter Members
        /// <summary>
        /// Formats the log infos.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="logEvent"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="log"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="message" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="logEvent"/> is <c>null</c>.</exception>
        public string FormatLogEvent(ILog log, string message, LogEvent logEvent, DateTime time)
        {
            Argument.IsNotNull("log", log);
            Argument.IsNotNullOrWhitespace("message", message);
            Argument.IsNotNull("logEvent", logEvent);

            var textWriter = new StringWriter();

            const string delimStart = "";
            textWriter.Write(delimStart);

            textWriter.Write("{");

            var delim = string.Empty;
            WriteJsonProperty("Timestamp", time, ref delim, textWriter);
            WriteJsonProperty("Level", LogEventStrings[logEvent], ref delim, textWriter);
            WriteJsonProperty("MessageTemplate", message, ref delim, textWriter);

            textWriter.Write(",\"Properties\":{");
            var pdelim = string.Empty;
            WriteJsonProperty("ApplicationName", ApplicationName, ref pdelim, textWriter);
            WriteJsonProperty("ApplicationVersion", ApplicationVersion, ref pdelim, textWriter);
            WriteJsonProperty("Name", log.Name, ref pdelim, textWriter);
            WriteJsonProperty("TargetType", log.TargetType?.FullName ?? string.Empty, ref pdelim, textWriter);

            textWriter.Write("}");

            textWriter.Write("}");

            return textWriter.ToString();
        }
        #endregion

        #region Methods
        private static void WriteJsonProperty(string name, object value, ref string precedingDelimiter, TextWriter textWriter)
        {
            textWriter.Write(precedingDelimiter);
            WritePropertyName(name, textWriter);
            WriteLiteral(value, textWriter);
            precedingDelimiter = ",";
        }

        private static void WritePropertyName(string name, TextWriter textWriter)
        {
            textWriter.Write("\"");
            textWriter.Write(name);
            textWriter.Write("\":");
        }

        private static void WriteString(object value, TextWriter textWriter)
        {
            textWriter.Write("\"");
            textWriter.Write(value.ToString());
            textWriter.Write("\"");
        }

        private static void WriteLiteral(object value, TextWriter textWriter, bool forceQuotation = false)
        {
            if (value == null)
            {
                textWriter.Write("null");
                return;
            }

            if (LiteralWriters.TryGetValue(value.GetType(), out var writer))
            {
                writer(value, forceQuotation, textWriter);
                return;
            }

            WriteString(value.ToString(), textWriter);
        }

        private static void WriteToString(object number, bool quote, TextWriter textWriter)
        {
            if (quote)
            {
                textWriter.Write('"');
            }

            var formattable = number as IFormattable;
            textWriter.Write(formattable != null ? formattable.ToString(null, CultureInfo.InvariantCulture) : number.ToString());

            if (quote)
            {
                textWriter.Write('"');
            }
        }

        private static void WriteBoolean(bool value, TextWriter textWriter)
        {
            textWriter.Write(value ? "true" : "false");
        }

        private static void WriteOffset(DateTimeOffset value, TextWriter textWriter)
        {
            textWriter.Write("\"");
            textWriter.Write(value.ToString("o"));
            textWriter.Write("\"");
        }

        private static void WriteDateTime(DateTime value, TextWriter textWriter)
        {
            textWriter.Write("\"");
            textWriter.Write(value.ToString("o"));
            textWriter.Write("\"");
        }
        #endregion
    }
}

#endif
