// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel
{
    using System.Diagnostics;

    /// <summary>
    /// Process extensions.
    /// </summary>
    public static class ProcessExtensions
    {
        #region Methods
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int) processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int) parentId.NextValue());
        }

        /// <summary>
        /// Gets the parent process of the specified process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns>Process.</returns>
        public static Process GetParent(this Process process)
        {
            var findIndexedProcessName = FindIndexedProcessName(process.Id);
            return FindPidFromIndexedProcessName(findIndexedProcessName);
        }
        #endregion
    }
}

#endif