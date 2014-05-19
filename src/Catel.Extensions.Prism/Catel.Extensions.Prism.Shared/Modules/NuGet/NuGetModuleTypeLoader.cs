// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetModuleTypeLoader.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows.Threading;

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
        /// The module catalogs.
        /// </summary>
        private readonly ReadOnlyCollection<INuGetBasedModuleCatalog> _moduleCatalogs;
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

            if (moduleCatalog is INuGetBasedModuleCatalog)
            {
                _moduleCatalogs = new List<INuGetBasedModuleCatalog> { moduleCatalog as INuGetBasedModuleCatalog }.AsReadOnly();
            }

            if (moduleCatalog is CompositeModuleCatalog)
            {
                var compositeModuleCatalog = moduleCatalog as CompositeModuleCatalog;
                
                _moduleCatalogs = compositeModuleCatalog.LeafCatalogs.OfType<INuGetBasedModuleCatalog>().ToList().AsReadOnly();
            }

            if (_moduleCatalogs == null || _moduleCatalogs.Count == 0)
            {
                const string error = "There are no NuGet based module catalogs available";
                Log.Error(error);
                throw new InvalidOperationException(error);
            }
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

            Dispatcher currentDispatcher = DispatcherHelper.CurrentDispatcher;

            Log.Debug("Loading module type '{0}' from package '{1}'", moduleInfo.ModuleType, moduleInfo.Ref);

            new Thread(() =>
                {
                    InstallPackageRequest installPackageRequest;
                    var packageName = moduleInfo.GetPackageName();
                    if (packageName != null && this.TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest))
                    {
                        currentDispatcher.BeginInvoke(() => OnModuleDownloadProgressChanged(new ModuleDownloadProgressChangedEventArgs(moduleInfo, 0, 1)));

                        bool installed = false;
                        try
                        {
                            installPackageRequest.Execute();
                            installed = true;
                        }
                        catch (Exception e)
                        {
                            currentDispatcher.BeginInvoke(() => OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, e)));
                        }

                        if (installed)
                        {
                            var fileModuleTypeLoader = new FileModuleTypeLoader();
                            var fileModuleInfo = new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType)
                            {
                                Ref = installPackageRequest.AssemblyFileRef,
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
                        }
                    }
                    else
                    {
                        currentDispatcher.BeginInvoke(() => OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, new ModuleNotFoundException(moduleInfo.ModuleName, string.Format(CultureInfo.InvariantCulture, "The package '{0}' for module '{1}' was not found", moduleInfo.Ref, moduleInfo.ModuleName)))));
                    }
                }).Start();
        }

        /// <summary>
        /// Tries create install package request.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <param name="installPackageRequest">The install package request</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            int i = 0;
            installPackageRequest = null;
            while (i < _moduleCatalogs.Count && !_moduleCatalogs[i].TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest))
            {
                i++;
            }

            return installPackageRequest != null;
        }
        #endregion
    }
}

#endif