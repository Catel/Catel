// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

#if NET || NETSTANDARD
    using System.Diagnostics;
#else
    using System.ComponentModel;
#endif

    /// <summary>
    /// Helper class for environment information.
    /// </summary>
    public static class EnvironmentHelper
    {
        private static readonly Lazy<bool> _hostedByVisualStudio = new Lazy<bool>(() => IsProcessCurrentlyHostedByVisualStudio(false));
        private static readonly Lazy<bool> _hostedBySharpDevelop = new Lazy<bool>(() => IsProcessCurrentlyHostedBySharpDevelop(false));
        private static readonly Lazy<bool> _hostedByExpressionBlend = new Lazy<bool>(() => IsProcessCurrentlyHostedByExpressionBlend(false));

        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByVisualStudio
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedByVisualStudio == null)
                {
                    return false;
                }

                return _hostedByVisualStudio.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by sharp develop.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by sharp develop; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedBySharpDevelop
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedBySharpDevelop == null)
                {
                    return false;
                }

                return _hostedBySharpDevelop.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByExpressionBlend
        {
            get
            {
                // This is required because the logging checks for this when creating the Lazy class
                if (_hostedByExpressionBlend == null)
                {
                    return false;
                }

                return _hostedByExpressionBlend.Value;
            }
        }

        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// </summary>
        /// <returns><c>true</c> if the process is hosted by any tool, such as visual studio or blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessHostedByTool
        {
            get { return IsProcessHostedByVisualStudio || IsProcessHostedByExpressionBlend; }
        }

        /// <summary>
        /// Determines whether the process is hosted by visual studio.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByVisualStudio" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by visual studio; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByVisualStudio(bool checkParentProcesses = false)
        {
#if NET || NETSTANDARD
            return IsHostedByProcess("devenv", checkParentProcesses);
#elif XAMARIN
            return false;
#elif NETFX_CORE
            return global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
            return DesignerProperties.IsInDesignTool;
#endif
        }

        /// <summary>
        /// Determines whether the process is hosted by sharp develop.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByExpressionBlend" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by sharp develop; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedBySharpDevelop(bool checkParentProcesses = false)
        {
#if NET || NETSTANDARD
            return IsHostedByProcess("sharpdevelop", checkParentProcesses);
#elif XAMARIN
            return false;
#elif NETFX_CORE
            return global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
            return DesignerProperties.IsInDesignTool;
#endif
        }

        /// <summary>
        /// Determines whether the process is hosted by expression blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByExpressionBlend" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the process is hosted by expression blend; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByExpressionBlend(bool checkParentProcesses = false)
        {
#if NET || NETSTANDARD
            return IsHostedByProcess("blend", checkParentProcesses);
#elif XAMARIN
            return false;
#elif NETFX_CORE
            return global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#else
            return DesignerProperties.IsInDesignTool;
#endif
        }

        /// <summary>
        /// Determines whether the process is hosted by any tool, such as visual studio or blend.
        /// <para />
        /// This methods executes the logic every time it is called. To get a cached value, use the
        /// <see cref="IsProcessHostedByTool" /> instead.
        /// </summary>
        /// <param name="checkParentProcesses">if set to <c>true</c>, the parent processes will also be checked.</param>
        /// <returns><c>true</c> if the current process is hosted by any tool; otherwise, <c>false</c>.</returns>
        public static bool IsProcessCurrentlyHostedByTool(bool checkParentProcesses = false)
        {
            if (IsProcessCurrentlyHostedByVisualStudio(checkParentProcesses))
            {
                return true;
            }

            if (IsProcessCurrentlyHostedBySharpDevelop(checkParentProcesses))
            {
                return true;
            }

            if (IsProcessCurrentlyHostedByExpressionBlend(checkParentProcesses))
            {
                return true;
            }

            return false;
        }

#if NET || NETSTANDARD
        private static bool IsHostedByProcess(string processName, bool supportParentProcesses = false)
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                if (currentProcess == null)
                {
                    return false;
                }

                var currentProcessName = currentProcess.ProcessName;
                if (supportParentProcesses && currentProcessName.ContainsIgnoreCase("vshost"))
                {
#if NET
                    currentProcess = currentProcess.GetParent();
                    if (currentProcess == null)
                    {
                        return false;
                    }

                    currentProcessName = currentProcess.ProcessName;
#else
                    return false;
#endif
                }

                var isHosted = currentProcessName.StartsWithIgnoreCase(processName);
                return isHosted;
            }
            catch (Exception)
            {
                // Ignore
                return false;
            }
        }
#endif
    }
}
