namespace Catel.Services
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Catel.Logging;

    /// <summary>
    /// Process service to run files or start processes from a view model.
    /// </summary>
    public class ProcessService : IProcessService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Starts a process and returns an awaitable task which will end once the application is closed.
        /// </summary>
        /// <param name="processContext">The process context of an application file to run in the process.</param>
        /// <returns>The <see cref="ProcessResult"/> containing details about the execution.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="processContext"/> is <c>null</c>.</exception>
        public virtual async Task<ProcessResult> RunAsync(ProcessContext processContext)
        {
            Argument.IsNotNull(nameof(processContext), processContext);
            Argument.IsNotNullOrWhitespace(nameof(processContext.FileName), processContext.FileName);

            var fileName = processContext.FileName;

            var arguments = processContext.Arguments;
            if (arguments is null)
            {
                arguments = string.Empty;
            }

            var tcs = new TaskCompletionSource<int>();

            Log.Debug($"Running '{processContext.FileName}'");

            try
            {
                var processStartInfo = new ProcessStartInfo(fileName, arguments)
                {
                    Verb = processContext.Verb,
                    UseShellExecute = processContext.UseShellExecute
                };

                // Note for debuggers: whenever you *inspect* processStartInfo *and* use UseShellExecute = true,
                // the debugger will evaluate the Environment and EnvironmentVariables properties and instantiate them. 
                // This will result in Process.Start to throw exceptions. The solution is *not* to inspect processStartInfo
                // when UseShellExecute is true

                if (!string.IsNullOrWhiteSpace(processContext.Verb) &&
                    !processStartInfo.UseShellExecute)
                {
                    Log.Warning($"Verb is specified, this requires UseShellExecute to be set to true");

                    processStartInfo.UseShellExecute = true;
                }

                if (!string.IsNullOrWhiteSpace(processContext.WorkingDirectory))
                {
                    processStartInfo.WorkingDirectory = processContext.WorkingDirectory;
                }

#pragma warning disable IDISP001 // Dispose created
                var process = Process.Start(processStartInfo);
#pragma warning restore IDISP001 // Dispose created
                if (process is null)
                {
                    Log.Debug($"Process is already completed, cannot wait for it to complete");

                    tcs.SetResult(0);
                }
                else
                {
                    process.EnableRaisingEvents = true;
                    process.Exited += (sender, e) => tcs.SetResult(process.ExitCode);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occurred while running '{processContext.FileName}'");

                tcs.SetException(ex);
            }

            await tcs.Task;

            return new ProcessResult(processContext)
            {
                ExitCode = tcs.Task?.Result ?? 0
            };
        }

        /// <summary>
        /// Starts a process resource by specifying the name of an application and a set of command-line arguments.
        /// </summary>
        /// <param name="processContext">The process context of an application file to run in the process.</param>
        /// <param name="processCompletedCallback">The process completed callback, invoked only when the process is started successfully and completed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="processContext"/> is <c>null</c>.</exception>
        /// <exception cref="Win32Exception">An error occurred when opening the associated file.</exception>
        public virtual void StartProcess(ProcessContext processContext, ProcessCompletedDelegate processCompletedCallback = null)
        {
            var task = RunAsync(processContext);

            if (processCompletedCallback is not null)
            {
                task.ContinueWith(x => processCompletedCallback(processContext, task.Result.ExitCode));
            }
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
