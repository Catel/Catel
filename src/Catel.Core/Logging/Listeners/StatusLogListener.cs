// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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