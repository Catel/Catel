// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogEvent.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;

    /// <summary>
    ///   Different types of logging events.
    /// </summary>
    [Flags]
    public enum LogEvent
    {
        /// <summary>
        ///   Debug message.
        /// </summary>
        Debug = 1,

        /// <summary>
        ///   Info message.
        /// </summary>
        Info = 2,

        /// <summary>
        ///   Warning message.
        /// </summary>
        Warning = 4,

        /// <summary>
        ///   Error message.
        /// </summary>
        Error = 8
    }
}