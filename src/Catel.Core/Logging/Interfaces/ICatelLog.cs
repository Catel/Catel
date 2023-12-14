namespace Catel.Logging
{
    /// <summary>
    /// Log interface used internally for Catel.
    /// </summary>
    internal interface ICatelLog : ILog
    {
        /// <summary>
        /// Gets a value indicating whether this log should always write logging statements regardless of log filter settings.
        /// </summary>
        bool AlwaysLog { get; }
    }
}
