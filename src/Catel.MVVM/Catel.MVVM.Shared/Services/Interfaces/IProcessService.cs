// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// Delegate to invoke when a process of the <see cref="IProcessService"/> is completed.
    /// </summary>
    public delegate void ProcessCompletedDelegate(int exitCode);

    /// <summary>
    /// Interface for the Process service.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line arguments.
        /// </summary>
        /// <param name="fileName">The name of an application file to run in the process.</param>
        /// <param name="arguments">Command-line arguments to pass when starting the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentException">The <paramref name="fileName"/> is <c>null</c> or whitespace.</exception>
        void StartProcess(string fileName, string arguments = "", ProcessCompletedDelegate processCompletedCallback = null);
    }
}
