// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonLogFormatter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;

    /// <summary>
    /// The formatter which formats all log info to Json.
    /// </summary>
    public interface IJsonLogFormatter
    {
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
        string FormatLogEvent(ILog log, string message, LogEvent logEvent, DateTime time);
    }
}
