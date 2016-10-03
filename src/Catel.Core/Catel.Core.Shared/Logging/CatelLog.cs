// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelLog.cs" company="Catel development team">
//   Copyright (c) 2016 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;


namespace Catel.Logging
{
    /// <summary>
    /// Logging class used internally for Catel. 
    /// </summary>
    internal class CatelLog : Log, ICatelLog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatelLog"/> class.
        /// </summary>
        /// <param name="name">The name of this logger.</param>
        /// <param name="alwaysLog">Flag indicating whether this log should always write logging statements regardless of log filter settings.</param>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        public CatelLog(string name, bool alwaysLog)
            : base(name, null)
        {
            AlwaysLog = alwaysLog;
        }

        #region Implementation of ICatelLog

        /// <summary>
        /// Gets a value indicating whether this log should always write logging statements regardless of log filter settings.
        /// </summary>
        public bool AlwaysLog { get; }

        #endregion

        #region Overrides of Log

        /// <summary>
        /// Gets a value indicating whether this logger is a Catel logger.
        /// <para />
        /// This value can be useful to exclude Catel logging for external listeners.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a Catel logger; otherwise, <c>false</c>.
        /// </value>
        public override bool IsCatelLogging
        {
            get { return true; }
        }

        #endregion
    }
}
