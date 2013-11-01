// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetModuleTypeLoader.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    using Catel.Logging;
    using Catel.Modules.Extensions;
    using Catel.Modules.Interfaces;
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
        /// The install package requests.
        /// </summary>
        private readonly Dictionary<ModuleInfo, InstallPackageRequest> _installPackageRequest = new Dictionary<ModuleInfo, InstallPackageRequest>();

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
                throw new InvalidOperationException("There are no available any NuGet based module catalog");
            }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// The load module completed event
        /// </summary>
        public event EventHandler<LoadModuleCompletedEventArgs> LoadModuleCompleted;

        /// <summary>
        /// The module download progress changed event
        /// </summary>
        public event EventHandler<ModuleDownloadProgressChangedEventArgs> ModuleDownloadProgressChanged;

        #region Methods
        /// <summary>
        /// Called when the load module is completed.
        /// </summary>
        /// <param name="e">
        /// The event argument.
        /// </param>
        private void OnLoadModuleCompleted(LoadModuleCompletedEventArgs e)
        {
            LoadModuleCompleted.SafeInvoke(this, e);
        }

        /// <summary>
        /// Called when the module download progress changed.
        /// </summary>
        /// <param name="e">
        /// The event argument.
        /// </param>
        /// <remarks>
        /// The current implementation doesn't support 
        /// </remarks>
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
        /// <exception cref="System.InvalidOperationException">The <see cref="ModuleInfo.ModuleType"/> property of <paramref name="moduleInfo"/> parameter doesn't speciefied usign qualified name pattern.</exception>
        public bool CanLoadModuleType(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            bool canLoad = false;
            var packageName = moduleInfo.GetPackageName();
            if (packageName != null)
            {
                int i = 0;
                while (!canLoad && i < _moduleCatalogs.Count)
                {
                    INuGetBasedModuleCatalog moduleCatalog = _moduleCatalogs[i++];

                    InstallPackageRequest installPackageRequest;
                    if (moduleCatalog.TryCreateInstallPackageRequest(moduleInfo, out installPackageRequest))
                    {
                        Log.Debug("Queuing install package request");

                        canLoad = true;
                      _installPackageRequest.Add(moduleInfo, installPackageRequest);
                    }
                }
            }

            return canLoad;
        }

        /// <summary>
        /// Load the module type.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module Info.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="moduleInfo" /> is <c>null</c>.
        /// </exception>
        public void LoadModuleType(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            Log.Debug("Loading module type '{0}' from package '{1}'", moduleInfo.ModuleType, moduleInfo.Ref);

            if (_installPackageRequest.ContainsKey(moduleInfo))
            {
                InstallPackageRequest installPackageRequest = _installPackageRequest[moduleInfo];
                new Thread(() =>
                    {
                        Dispatcher currentDispatcher = DispatcherHelper.CurrentDispatcher;
                        
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
                    }).Start();
            }
        }
        #endregion
    }
}