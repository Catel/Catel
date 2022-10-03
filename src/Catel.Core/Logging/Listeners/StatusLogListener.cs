namespace Catel.Logging
{
    /// <summary>
    /// Log listener for status messages.
    /// </summary>
    public class StatusLogListener : LogListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusLogListener"/> class.
        /// </summary>
        public StatusLogListener()
        {
            IsDebugEnabled = false;
            IsInfoEnabled = false;
            IsWarningEnabled = false;
            IsErrorEnabled = false;
            IsStatusEnabled = true;
        }
    }
}
