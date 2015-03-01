// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Services
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

#if NETFX_CORE
    using global::Windows.System;
#endif

    /// <summary>
    /// Process service to run files or start processes from a view model.
    /// </summary>
    public class ProcessService : IProcessService
    {
        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line arguments.
        /// </summary>
        /// <param name="fileName">The name of an application file to run in the process.</param>
        /// <param name="arguments">Command-line arguments to pass when starting the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentException">The <paramref name="fileName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
        public virtual void StartProcess(string fileName, string arguments = "", ProcessCompletedDelegate processCompletedCallback = null)
        {
            Argument.IsNotNullOrWhitespace("fileName", fileName);

            if (arguments == null)
            {
                arguments = string.Empty;
            }

#if NETFX_CORE
            var launcher = Launcher.LaunchUriAsync(new Uri(fileName));
            if (processCompletedCallback != null)
            {
                launcher.Completed += (sender, e) => processCompletedCallback(0);
            }
#else
            var process = Process.Start(fileName, arguments);
            if (processCompletedCallback != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => processCompletedCallback(process.ExitCode);
            }
#endif
        }
    }
}

#endif