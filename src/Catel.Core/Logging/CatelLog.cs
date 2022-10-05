namespace Catel.Logging
{
    using System;

    /// <summary>
    /// Logging class used internally for Catel. 
    /// </summary>
    internal class CatelLog : Log, ICatelLog
    {
        public CatelLog(Type type, bool alwaysLog)
            : base(type)
        {
            AlwaysLog = alwaysLog;
        }

        /// <summary>
        /// Gets a value indicating whether this log should always write logging statements regardless of log filter settings.
        /// </summary>
        public bool AlwaysLog { get; }

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
    }
}
