// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetModuleTypeLoader.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NET

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    using Catel.Logging;
    using Catel.Windows.Threading;

    using Microsoft.Practices.Prism.Modularity;

    /// <summary>
    /// The NuGet module type loader.
    /// </summary>
    public sealed class NuGetModuleTypeLoader : IModuleTypeLoader
    {
        #region Constants

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields

        /// <summary>
        /// The module catalog
        /// </summary>
        private IModuleCatalog _moduleCatalog;

        /// <summary>
        /// The sync obj.
        /// </summary>
        private readonly object _syncObj = new object();

        #endregion

        #region Properties
        /// <summary>
        /// The module catalogs.
        /// </summary>
        private ReadOnlyCollection<INuGetBasedModuleCatalog> NuGetBasedModuleCatalogs
        {
            get
            {
                lock (_syncObj)
                {
                    ReadOnlyCollection<INuGetBasedModuleCatalog> moduleCatalogs = null;

                    if (_moduleCatalog is INuGetBasedModuleCatalog)
                    {
                        moduleCatalogs = new List<INuGetBasedModuleCatalog> { _moduleCatalog as INuGetBasedModuleCatalog }.AsReadOnly();
                    }

                    if (_moduleCatalog is CompositeModuleCatalog)
                    {
                        var compositeModuleCatalog = _moduleCatalog as CompositeModuleCatalog;

                        moduleCatalogs = compositeModuleCatalog.LeafCatalogs.OfType<INuGetBasedModuleCatalog>().ToList().AsReadOnly();
                    }

                    if (moduleCatalogs == null || moduleCatalogs.Count == 0)
                    {
                        Log.Warning("There are no NuGet based module catalogs available");
                    }

                    return moduleCatalogs;
                }
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetModuleTypeLoader" /> class.
        /// </summary>
        /// <param name="moduleCatalog">
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">There are no available any NuGet based module catalog.</exception>
        public NuGetModuleTypeLoader(IModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull(() => moduleCatalog);

            _moduleCatalog = moduleCatalog;

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The load module completed event.
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        /// <summary>
        /// The module download progress changed event.
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        #region Methods
        /// <summary>
        /// Called when the load module is completed.
        /// </summary>
        /// <param name="e">The event argument.</param>
        private void OnLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            LoadModuleCompleted.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the module download progress changed.
        /// </summary>
        /// <param name="e">The event argument.</param>
        /// <remarks>The current implementation doesn't support</remarks>
        private void OnModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            ModuleDownloadProgressChanged.SafeInvoke(this, e);
        }
        #endregion

        #endregion

        #region IModuleTypeLoader Members

        /// <summary>
        /// Can load module type.
        /// </summary>
        /// <param name="moduleInfo">The module Info.</param>
        /// <returns>The <see cref="bool" />.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        /// <exception cref="System.InvalidOperationException">The <see cref="ModuleInfo.ModuleType" /> property of <paramref name="moduleInfo" /> parameter doesn't speciefied usign qualified name pattern.</exception>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            return moduleInfo.GetPackageName() != null;
        }

        /// <summary>
        /// Load the module type.
        /// </summary>
        /// <param name="moduleInfo">The module Info.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            var currentDispatcher = DispatcherHelper.CurrentDispatcher;

            Log.Debug("Loading module type '{0}' from package '{1}'", moduleInfo.ModuleType, moduleInfo.Ref);

            var thread = new Thread(() =>
            {
                InstallPackageRequest installPackageRequest;
                var packageName = moduleInfo.GetPackageName();
                if (packageName != null && TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest))
                {
                    currentDispatcher.BeginInvoke(() => OnModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, 0, 1)));

                    var installed = false;

                    try
                    {
                        installPackageRequest.Execute();
                        installed = true;
                    }
                    catch (Exception ex)
                    {
                        currentDispatcher.BeginInvoke(() => OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, ex)));
                    }

                    if (installed)
                    {
                        var fileModuleTypeLoader = new FileModuleTypeLoader();
                        var fileModuleInfo = new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType)
                        {
                            Ref = installPackageRequest.ModuleAssemblyRef.Ref,
                            InitializationMode = moduleInfo.InitializationMode,
                            DependsOn = moduleInfo.DependsOn
                        };

                        fileModuleTypeLoader.ModuleDownloadProgressChanged += (sender, args) =>
                        {
                            moduleInfo.State = args.ModuleInfo.State;
                            currentDispatcher.BeginInvoke(() => OnModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, args.BytesReceived, args.TotalBytesToReceive)));
                        };

                        fileModuleTypeLoader.LoadModuleCompleted += (sender, args) =>
                        {
                            moduleInfo.State = args.ModuleInfo.State;
                            currentDispatcher.BeginInvoke(() => OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, args.Error)));
                        };

                        fileModuleTypeLoader.LoadModuleType(fileModuleInfo);

                        Log.Info("Module '{0}' is installed and loaded", moduleInfo.ModuleName);
                    }
                }
                else
                {
                    currentDispatcher.BeginInvoke(() => OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, new ModuleNotFoundException(moduleInfo.ModuleName, string.Format(CultureInfo.InvariantCulture, "The package '{0}' for module '{1}' was not found", moduleInfo.Ref, moduleInfo.ModuleName)))));
                }
            });

            thread.Start();
        }

        /// <summary>
        /// Raised when the resolution of an assembly fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requestingAssembly = args.RequestingAssembly;

            var moduleCatalogs = NuGetBasedModuleCatalogs;

            if (!_moduleCatalog.Modules.Any(info => !string.IsNullOrWhiteSpace(info.Ref) && info.Ref.StartsWith(args.Name)) && (requestingAssembly == null && moduleCatalogs.Count > 0))
            {
                Log.Debug("Trying to resolve '{0}'", args.Name);

                var frameworkIdentifierPath = string.Format("\\{0}\\", Platforms.CurrentPlatform).ToLower();

                var assemblyName = args.Name.Split(',')[0].Trim();

                var outputDirectoryFullPaths = moduleCatalogs.Select(catalog => catalog.OutputDirectoryFullPath).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

                int i = 0;
                while (requestingAssembly == null && i < outputDirectoryFullPaths.Length)
                {
                    var outputDirectoryFullPath = outputDirectoryFullPaths[i++];
                    if (Directory.Exists(outputDirectoryFullPath))
                    {
                        var assemblyFilePath = string.Empty;
                        try
                        {
                            var expectedAssemblyFileName = string.Format("{0}.dll", assemblyName);

                            // Search with framework identifiers first
                            assemblyFilePath = Directory.EnumerateFiles(outputDirectoryFullPath, expectedAssemblyFileName, SearchOption.AllDirectories).FirstOrDefault(path => path.ToLower().Contains(frameworkIdentifierPath));
                            if (string.IsNullOrWhiteSpace(assemblyFilePath))
                            {
                                assemblyFilePath = Directory.EnumerateFiles(outputDirectoryFullPath, expectedAssemblyFileName, SearchOption.AllDirectories).FirstOrDefault();
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to search for assembly '{0}'", assemblyName);
                        }

                        if (!string.IsNullOrWhiteSpace(assemblyFilePath))
                        {
                            Log.Warning("Resolved '{0}' at '{1}'", args.Name, assemblyFilePath);

                            requestingAssembly = Assembly.LoadFile(assemblyFilePath);
                        }
                        else
                        {
                            Log.Warning("Could not resolve '{0}'", args.Name);
                        }
                    }
                }
            }

            return requestingAssembly;
        }

        /// <summary>
        /// Tries create install package request.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <param name="installPackageRequest">The install package request</param>
        /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
        private bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            var i = 0;
            installPackageRequest = null;
            var moduleCatalogs = NuGetBasedModuleCatalogs;
            while (i < moduleCatalogs.Count && !moduleCatalogs[i].TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest))
            {
                i++;
            }

            return installPackageRequest != null;
        }
        #endregion
    }
}

#endif