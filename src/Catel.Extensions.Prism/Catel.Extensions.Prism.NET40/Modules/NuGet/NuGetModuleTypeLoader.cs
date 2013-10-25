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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Catel.Logging;
    using Catel.Modules.Extensions;

    using Microsoft.Practices.Prism.Modularity;
    using Microsoft.Win32;

    using NuGet;

    /// <summary>
    /// The NuGet module type loader.
    /// </summary>
    public sealed class NuGetModuleTypeLoader : IModuleTypeLoader
    {
        #region Constants
       
        /// <summary>
        /// The framework identifier conversion map.
        /// </summary>
        private static readonly Dictionary<string, string> FrameworkIdentifierConversionMap = new Dictionary<string, string> { { ".NETFramework,Version=v4.0", "NET40" }, { ".NETFramework,Version=v4.5", "NET45" } };

     

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
        private readonly ReadOnlyCollection<NuGetBasedModuleCatalog> _moduleCatalogs;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetModuleTypeLoader" /> class.
        /// </summary>
        /// <param name="moduleCatalog">
        /// </param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog" /> is <c>null</c>.</exception>
        public NuGetModuleTypeLoader(IModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull(() => moduleCatalog);

            if (moduleCatalog is NuGetBasedModuleCatalog)
            {
                _moduleCatalogs = new List<NuGetBasedModuleCatalog> { moduleCatalog as NuGetBasedModuleCatalog }.AsReadOnly();
            }

            if (moduleCatalog is CompositeModuleCatalog)
            {
                var compositeModuleCatalog = moduleCatalog as CompositeModuleCatalog;
                _moduleCatalogs = compositeModuleCatalog.QueryItems.ToList().OfType<NuGetBasedModuleCatalog>().ToList().AsReadOnly();
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
                    NuGetBasedModuleCatalog moduleCatalog = _moduleCatalogs[i++];

                    if (packageName.Version != null && moduleCatalog.IsModuleAssemblyInstalled(moduleInfo))
                    {
                        Log.Debug("Creating install package request for '{0}' that is actually installed.", packageName.Name);

                        canLoad = true;

                        _installPackageRequest.Add(moduleInfo, new InstallPackageRequest(moduleCatalog.GetModuleAssemblyRef(moduleInfo)));
                    } 
                    else
                    {
                        IPackageRepository repository = moduleCatalog.GetPackageRepository();
                        if (repository != null)
                        {
                            Log.Debug("Looking for package '{0}' with version '{1}' on the repository '{2}'", packageName.Id, packageName.Version, moduleCatalog.PackageSource);

                            IPackage package;
                            if (repository.TryFindPackage(packageName.Id, packageName.Version, out package))
                            {
                                var supportedFrameworks = package.GetSupportedFrameworks();
                                if (supportedFrameworks != null && supportedFrameworks.Any(name => FrameworkIdentifierConversionMap.ContainsKey(name.FullName) && FrameworkIdentifierConversionMap[name.FullName].Equals(moduleCatalog.FrameworkNameIdentifier)))
                                {
                                    Log.Debug("Creating install package request for '{0}' from '{1}'", package.GetFullName(), moduleCatalog.PackageSource);

                                    canLoad = true;

                                    _installPackageRequest.Add(moduleInfo, new RemoteInstallPackageRequest(moduleCatalog, package, moduleCatalog.GetModuleAssemblyRef(moduleInfo, package.Version)));
                                }
                            }
                        }     
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

            if (_installPackageRequest.ContainsKey(moduleInfo))
            {
                InstallPackageRequest installPackageRequest = _installPackageRequest[moduleInfo];
                installPackageRequest.Execute();

                var fileModuleTypeLoader = new FileModuleTypeLoader();
                var fileModuleInfo = new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType) { Ref = installPackageRequest.AssemblyFileRef, InitializationMode = moduleInfo.InitializationMode, DependsOn = moduleInfo.DependsOn };
                fileModuleTypeLoader.LoadModuleCompleted += (sender, args) =>
                    {
                        if (args.Error != null)
                        {
                            Log.Error(args.Error);
                        }
                 
                        moduleInfo.State = args.ModuleInfo.State;
                        OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, args.Error));
                    };

                Log.Debug("Loading file module assembly '{0}'", fileModuleInfo.Ref);

                fileModuleTypeLoader.LoadModuleType(fileModuleInfo);
            }
        }
        #endregion
    }
}