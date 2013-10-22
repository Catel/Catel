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
    using System.Linq;
    using System.Text.RegularExpressions;

    using Catel.Logging;
    using Catel.Modules.Extensions;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The NuGet module type loader.
    /// </summary>
    public sealed class NuGetModuleTypeLoader : IModuleTypeLoader
    {
        #region Constants
        /// <summary>
        /// The type name pattern.
        /// </summary>
        private const string TypeNamePattern = "[^,]+,([^,]+).*";

        /// <summary>
        /// Module type must be specified using qualified name pattern error message
        /// </summary>
        private const string ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage = "The module type must be specified using a qualified name pattern";

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
        private readonly Dictionary<ModuleInfo, InstallPackageRequest> _installPackageRequests = new Dictionary<ModuleInfo, InstallPackageRequest>();

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
        /// The current implementation doesn't support this.
        /// </remarks>
        private void OnModuleDownloadProgressChanged(ModuleDownloadProgressChangedEventArgs e)
        {
            ModuleDownloadProgressChanged.SafeInvoke(this, e);
        }
        #endregion

        #endregion

        #region IModuleTypeLoader Members
        /// <summary>
        /// The can load module type.
        /// </summary>
        /// <param name="moduleInfo">The module Info.</param>
        /// <returns>The <see cref="bool" />.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
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
                    IPackageRepository repository = moduleCatalog.GetPackageRepository();
                    if (repository != null)
                    {
                        Log.Debug("Looking for package '{0}' with version '{1}' on repository '{2}'", packageName.Id, packageName.Version, moduleCatalog.PackageSource);

                        IPackage package;
                        if (repository.TryFindPackage(packageName.Id, packageName.Version, out package))
                        {
                            var supportedFrameworks = package.GetSupportedFrameworks();
                            if (supportedFrameworks != null && supportedFrameworks.Any(name => FrameworkIdentifierConversionMap.ContainsKey(name.FullName) && FrameworkIdentifierConversionMap[name.FullName].Equals(moduleCatalog.FrameworkNameIdentifier)))
                            {
                                Log.Debug("Creating install package request for '{0}' from '{1}'", package.GetFullName(), moduleCatalog.PackageSource);

                                canLoad = true;
                                _installPackageRequests.Add(moduleInfo, new InstallPackageRequest(moduleCatalog, package));
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

            if (_installPackageRequests.ContainsKey(moduleInfo))
            {
                InstallPackageRequest installPackageRequest = _installPackageRequests[moduleInfo];

                string url = "file://" + installPackageRequest.OutputDirectory.Replace("\\", "/");

                var packageManager = new PackageManager(installPackageRequest.PackageRepository, installPackageRequest.OutputDirectory);
                packageManager.InstallPackage(installPackageRequest.Package, installPackageRequest.IgnoreDependencies, installPackageRequest.AllowPrereleaseVersions);

                Match typeNameMatch = Regex.Match(moduleInfo.ModuleType, TypeNamePattern);
                if (!typeNameMatch.Success)
                {
                    Log.Error(ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage);

                    throw new InvalidOperationException(ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage);
                }

                string assemblyFileName = typeNameMatch.Groups[1].Value.Trim() + ".dll";
                string directoryName = installPackageRequest.Package.GetFullName().Replace(' ', '.');

                var fileModuleTypeLoader = new FileModuleTypeLoader();
                string assemblyFileRef = string.Format("{0}/{1}/lib/{2}/{3}", url, directoryName, installPackageRequest.FrameworkNameIdentifier, assemblyFileName);
                var fileModuleInfo = new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType) { Ref = assemblyFileRef, InitializationMode = moduleInfo.InitializationMode, DependsOn = moduleInfo.DependsOn };

                fileModuleTypeLoader.LoadModuleCompleted += (sender, args) =>
                    {
                        if (args.Error != null)
                        {
                            Log.Error(args.Error);
                        }
                 
                        moduleInfo.State = args.ModuleInfo.State;
                        OnLoadModuleCompleted(new LoadModuleCompletedEventArgs(moduleInfo, args.Error));
                    };

                fileModuleTypeLoader.LoadModuleType(fileModuleInfo);
            }
        }
        #endregion
    }
}