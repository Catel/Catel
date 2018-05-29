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
        /// <param name="processContext">The process context of an application file to run in the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="processContext"/> is <c>null</c>.</exception>
        /// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
        public virtual void StartProcess(ProcessContext processContext, ProcessCompletedDelegate processCompletedCallback = null)
        {
            Argument.IsNotNull(nameof(processContext), processContext);
            Argument.IsNotNullOrWhitespace(nameof(processContext.FileName), processContext.FileName);

            var fileName = processContext.FileName;

            var arguments = processContext.Arguments;
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
            var processStartInfo = new ProcessStartInfo(fileName, arguments)
            {
                Verb = processContext.Verb
            };
            
            if (!string.IsNullOrWhiteSpace(processContext.WorkingDirectory))
            {
                processStartInfo.WorkingDirectory = processContext.WorkingDirectory;
            }

            var process = Process.Start(processStartInfo);
            if (processCompletedCallback != null)
            {
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => processCompletedCallback(process.ExitCode);
            }
#endif
        }

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
            StartProcess(new ProcessContext
            {
                FileName = fileName,
                Arguments = arguments
            }, processCompletedCallback);
        }
    }
}

#endif