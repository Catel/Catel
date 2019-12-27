// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProcessService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate to invoke when a process of the <see cref="IProcessService"/> is completed.
    /// </summary>
    public delegate void ProcessCompletedDelegate(ProcessContext processorContext, int exitCode);

    /// <summary>
    /// Interface for the Process service.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Starts a process and returns an awaitable task which will end once the application is closed.
        /// </summary>
        /// <param name="processContext">The process context of an application file to run in the process.</param>
        /// <returns>The <see cref="ProcessResult"/> containing details about the execution.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="processContext"/> is <c>null</c>.</exception>
        Task<ProcessResult> RunAsync(ProcessContext processContext);

        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line arguments.
        /// </summary>
        /// <param name="processContext">The process context of an application file to run in the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="processContext"/> is <c>null</c>.</exception>
        void StartProcess(ProcessContext processContext, ProcessCompletedDelegate processCompletedCallback = null);

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
