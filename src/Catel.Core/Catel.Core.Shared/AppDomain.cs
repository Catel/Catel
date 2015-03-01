// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomain.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE || PCL

namespace System
{
    using Catel.Logging;

    using Collections.Generic;
    using Reflection;

#if NETFX_CORE
    using global::Windows.ApplicationModel;
#endif

    /// <summary>
    /// WinRT implementation of the AppDomain class.
    /// </summary>
    public sealed class AppDomain
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// List of loaded assemblies.
        /// </summary>
        private List<Assembly> _loadedAssemblies;

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="AppDomain" /> class.
        /// </summary>
        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current domain.
        /// </summary>
        /// <value>The current domain.</value>
        public static AppDomain CurrentDomain { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the assemblies in the current application domain.
        /// </summary>
        /// <returns></returns>
        public Assembly[] GetAssemblies()
        {
            if (_loadedAssemblies == null)
            {
                _loadedAssemblies = new List<Assembly>();

#if NETFX_CORE
                var folder = Package.Current.InstalledLocation;

                var operation = folder.GetFilesAsync();
                var task = operation.AsTask();
                task.Wait();

                foreach (var file in task.Result)
                {
                    if (file.FileType == ".dll" || file.FileType == ".exe")
                    {
                        try
                        {
                            var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                            var name = new AssemblyName { Name = filename };
                            var asm = Assembly.Load(name);
                            _loadedAssemblies.Add(asm);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "Failed to load assembly '{0}'", file.Name);
                        }
                    }
                }
#else
                // TODO: Write
#endif
            }

            return _loadedAssemblies.ToArray();
        }
        #endregion
    }
}

#endif