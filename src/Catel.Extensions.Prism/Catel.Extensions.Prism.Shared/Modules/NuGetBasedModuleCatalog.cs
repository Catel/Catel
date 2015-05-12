// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The nuget based module catelog.
    /// </summary>
    public class NuGetBasedModuleCatalog : ModuleCatalog, INuGetBasedModuleCatalog
    {
        #region Constants
        /// <summary>
        /// The relative Url Pattern
        /// </summary>
        public const string RelativeUrlPattern = "{0}/{1}/lib/{2}/{3}";

        /// <summary>
        /// The module descriptor pattern.
        /// </summary>
        private const string ModuleDescriptorPattern = @"ModuleName\s*=([^;]+);\s*ModuleType\s*=([^;]+)(;\s*DependsOn=\s*\{(\s*[^}]+)\})?";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /*
        /// <summary>
        /// The framework identifier conversion map.
        /// </summary>
        private static readonly Dictionary<string, string> FrameworkIdentifierConversionMap = new Dictionary<string, string> { { ".NETFramework,Version=v4.0", "NET40" }, { ".NETFramework,Version=v4.5", "NET45" } };
        */

        /// <summary>
        /// The package repository cache.
        /// </summary>
        private static readonly CacheStorage<string, IPackageRepository> PackageRepositoriesCache = new CacheStorage<string, IPackageRepository>(storeNullValues: true);

        /// <summary>
        /// The module descriptor regular expresssion.
        /// </summary>
        private static readonly Regex ModuleDescriptorRegex = new Regex(ModuleDescriptorPattern, RegexOptions.Compiled);
        #endregion

        #region Fields
        /// <summary>
        /// Assembly ref cache storage
        /// </summary>
        private readonly CacheStorage<ModuleInfo, string> _assemblyRefCacheStorage = new CacheStorage<ModuleInfo, string>(storeNullValues: true);

        /// <summary>
        /// Module info cache storage.
        /// </summary>
        private readonly CacheStorage<string, ModuleInfo> _moduleInfoCacheStoreCacheStorage = new CacheStorage<string, ModuleInfo>(storeNullValues: true);

        /// <summary>
        /// Packaged module search cache storage.
        /// </summary>
        private readonly CacheStorage<string, IEnumerable<ModuleInfo>> _packagedModulesFilteredSearchCacheStorage = new CacheStorage<string, IEnumerable<ModuleInfo>>(() => ExpirationPolicy.Duration(TimeSpan.FromMinutes(2)), true, null);

        /// <summary>
        /// The nuget based module catalog parent child behavior
        /// </summary>
        private readonly NuGetBasedModuleCatalogParentChildBehavior _behavior;

        /// <summary>
        ///  NuGet like framework name identifier string, for instance <c>NET40</c> or <c>NET45</c>.
        /// </summary>
        private readonly string _frameworkNameIdentifier;

        #endregion

        #region Constructors
        /// <summary>
        /// The nuget based module catalog constructor.
        /// </summary>
        public NuGetBasedModuleCatalog()
        {
            _behavior = new NuGetBasedModuleCatalogParentChildBehavior(this);
            PackageSource = "https://nuget.org/api/v2/";

#if NET40
            _frameworkNameIdentifier = "NET40";
#else 
            _frameworkNameIdentifier = "NET45";
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the package source.
        /// </summary>
        public string PackageSource { get; set; }

        /// <summary>
        /// The output directory base Uri
        /// </summary>
        private string BaseUrl
        {
            get { return "file://" + OutputDirectoryFullPath.Replace("\\", "/"); }
        }

        /// <summary>
        /// Enumerates the available packaged modules.
        /// </summary>
        private IEnumerable<ModuleInfo> PackagedModules
        {
            get { return _packagedModulesFilteredSearchCacheStorage.GetFromCacheOrFetch(PackagedModuleIdFilterExpression, GetPackagedModules); }
        }

        /// <summary>
        /// Gets or sets the default initialization mode.
        /// </summary>
        /// <value>The default initialization mode.</value>
        public InitializationMode DefaultInitializationMode { get; set; }
        #endregion

        #region INuGetBasedModuleCatalog Members

        /// <summary>
        /// Gets or sets the parent nuget based catalog.
        /// </summary>
        /// <value>The parent.</value>
        public INuGetBasedModuleCatalog Parent { get; set; }

        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get { return _behavior.OutputDirectory; }
            set { _behavior.OutputDirectory = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog ignore the dependencies or not.
        /// </summary>
        public bool IgnoreDependencies
        {
            get { return _behavior.IgnoreDependencies; }
            set { _behavior.IgnoreDependencies = value; }
        }

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        public string PackagedModuleIdFilterExpression
        {
            get { return _behavior.PackagedModuleIdFilterExpression; }
            set { _behavior.PackagedModuleIdFilterExpression = value; }
        }

        /// <summary>
        /// Indicates whether the module catalog can download prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get { return _behavior.AllowPrereleaseVersions; }
            set { _behavior.AllowPrereleaseVersions = value; }
        }

        /// <summary>
        /// Gets the full path to the output output directory.
        /// </summary>
        public string OutputDirectoryFullPath
        {
            get
            {
                var outputDirectory = OutputDirectory;
                if (!Path.IsPathRooted(outputDirectory))
                {
                    var mainModuleDirectory = IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    outputDirectory = IO.Path.Combine(mainModuleDirectory, OutputDirectory);
                }

                return outputDirectory;
            }
        }

        /// <summary>
        /// Enumerates the available modules.
        /// </summary>
        public override IEnumerable<ModuleInfo> Modules
        {
            get
            {
                var moduleInfos = base.Modules.ToList();
                foreach (var packageModuleInfo in this.PackagedModules)
                {
                    var moduleInfo = moduleInfos.FirstOrDefault(info => info.ModuleName == packageModuleInfo.ModuleName);
                    if (moduleInfo == null)
                    {
                        Log.Info("Adding module info for '{0}' module with type '{1}'", packageModuleInfo.ModuleName, packageModuleInfo.ModuleType);

                        moduleInfos.Add(packageModuleInfo);
                    }
                    else if (moduleInfo.ModuleType == packageModuleInfo.ModuleType && (moduleInfo.Ref == null || !moduleInfo.Ref.Contains(',')))
                    {
                        Log.Info("Replacing module info for '{0}' module with type '{1}' to ensure load the lastest version from repository", moduleInfo.ModuleName, moduleInfo.ModuleType);

                        packageModuleInfo.InitializationMode = moduleInfo.InitializationMode;
                        moduleInfos[moduleInfos.IndexOf(moduleInfo)] = packageModuleInfo;
                    }
                    else
                    {
                        Log.Warning("Ignored module info for '{0}' module with type '{1}' because is already registered", moduleInfo.ModuleName, moduleInfo.ModuleType);
                    }
                }

                return moduleInfos;
            }
        }

        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo" />.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="installPackageRequest">The install package request.</param>
        /// <returns><c>true</c> whether the install package request is created, otherwise <c>false</c></returns>
        public bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            installPackageRequest = null;
            var packageName = moduleInfo.GetPackageName();
            if (packageName.Version != null && IsModuleAssemblyInstalled(moduleInfo))
            {
                Log.Debug("Creating local install package request for '{0}'.", packageName.Name);

                installPackageRequest = new InstallPackageRequest(GetModuleAssemblyRef(moduleInfo));
            }
            else
            {
                var repository = this.GetPackageRepository();
                if (repository != null)
                {
                    Log.Debug("Looking for package '{0}' with version '{1}' on the repository '{2}'", packageName.Id, packageName.Version, PackageSource);

                    IPackage package;
                    if (repository.TryFindPackage(packageName.Id, packageName.Version, out package))
                    {
                        /*
                        IEnumerable<FrameworkName> supportedFrameworks = package.GetSupportedFrameworks();
                        if (supportedFrameworks != null && supportedFrameworks.Any(name => FrameworkIdentifierConversionMap.ContainsKey(name.FullName) && FrameworkIdentifierConversionMap[name.FullName].Equals(_frameworkNameIdentifier)))
                        {
                            Log.Debug("Creating remote install package request for '{0}' from '{1}'", package.GetFullName(), PackageSource);
                        }
                        */

                        installPackageRequest = new RemoteInstallPackageRequest(this, package, GetModuleAssemblyRef(moduleInfo, package.Version));
                    }
                }
            }

            return installPackageRequest != null;
        }

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <returns>The <see cref="IPackageRepository" />.</returns>
        public virtual IPackageRepository GetInnerPackageRepository()
        {
            return PackageRepositoriesCache.GetFromCacheOrFetch(PackageSource, () =>
            {
                IPackageRepository packageRepository = null;
                try
                {
                    Log.Debug("Creating package repository with source '{0}'", PackageSource);

                    packageRepository = PackageRepositoryFactory.Default.CreateRepository(PackageSource);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return packageRepository;
            });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determine whether module is already installed.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <returns><c>true</c> if the module is installed otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        private bool IsModuleAssemblyInstalled(ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            return !string.IsNullOrWhiteSpace(GetModuleAssemblyRef(moduleInfo)) && File.Exists(new Uri(GetModuleAssemblyRef(moduleInfo)).LocalPath);
        }

        /// <summary>
        /// Gets the module assembly ref.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <returns>The module assembly ref</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        private string GetModuleAssemblyRef(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            return _assemblyRefCacheStorage.GetFromCacheOrFetch(moduleInfo, () =>
            {
                var packageName = moduleInfo.GetPackageName();
                var directoryName = packageName.ToString().Replace(' ', '.');

                return string.Format(CultureInfo.InvariantCulture, RelativeUrlPattern, BaseUrl, directoryName, _frameworkNameIdentifier, moduleInfo.GetAssemblyName());
            });
        }

        /// <summary>
        /// Gets the module assembly ref overriden the version number.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <param name="version">The version</param>
        /// <returns>The module assembly ref</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        private string GetModuleAssemblyRef(ModuleInfo moduleInfo, SemanticVersion version)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            var key = string.Format(CultureInfo.InvariantCulture, "ModuleName:{0}; ModuleType:{1}; Ref:{2}; Version:{3}", moduleInfo.ModuleName, moduleInfo.ModuleType, moduleInfo.Ref, version);
            return GetModuleAssemblyRef(_moduleInfoCacheStoreCacheStorage.GetFromCacheOrFetch(key, () => new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType) { Ref = string.Format("{0}, {1}", moduleInfo.GetPackageName().Id, version), DependsOn = moduleInfo.DependsOn }));
        }

        /// <summary>
        /// Gets the filtered packaged modules.
        /// <para />
        /// Override this method to customize the package filtering behavior.
        /// </summary>
        /// <param name="queryablePackages">The queryable packages.</param>
        /// <returns>IEnumerable&lt;IPackage&gt;.</returns>
        protected virtual IEnumerable<IPackage> GetFilteredPackagedModules(IQueryable<IPackage> queryablePackages)
        {
            Expression<Func<IPackage, bool>> filterExpression;
            if (!string.IsNullOrWhiteSpace(PackagedModuleIdFilterExpression))
            {
                filterExpression = package => (package.Id.Contains(PackagedModuleIdFilterExpression) && package.Description != null && package.Description.StartsWith("ModuleName") && package.Description.Contains("ModuleType"));
            }
            else
            {
                filterExpression = package => (package.Description != null) && package.Description.StartsWith("ModuleName") && package.Description.Contains("ModuleType");
            }

            var packages = queryablePackages.Where(filterExpression).ToList().GroupBy(package => package.Id).Select(packageGroup => packageGroup.ToList().OrderByDescending(package => package.Version).FirstOrDefault()).Where(package => package != null).ToList();
            return packages;
        }

        /// <summary>
        /// Gets the initialization mode for the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>InitializationMode.</returns>
        protected virtual InitializationMode GetPackageInitializationMode(IPackage package)
        {
            return DefaultInitializationMode;
        }

        /// <summary>
        /// Creates the package module based on a NuGet package.
        /// <para />
        /// Override this method to customize the module creation behavior.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>ModuleInfo.</returns>
        protected virtual ModuleInfo CreatePackageModule(IPackage package)
        {
            var match = ModuleDescriptorRegex.Match(package.Description);
            if (match.Success)
            {
                var moduleName = match.Groups[1].Value.Trim();
                var moduleType = match.Groups[2].Value.Trim();
                if (!string.IsNullOrWhiteSpace(moduleName) && !string.IsNullOrWhiteSpace(moduleType))
                {
                    var @ref = string.Format(CultureInfo.InvariantCulture, "{0}, {1}", package.Id, package.Version);
                    var dependsOn = new Collection<string>();
                    if (match.Groups.Count == 5)
                    {
                        var dependencies = match.Groups[4].Value.Trim();
                        if (!string.IsNullOrWhiteSpace(dependencies))
                        {
                            dependsOn.AddRange(dependencies.Split(',').Select(dependency => dependency.Trim()));
                        }
                    }

                    return new ModuleInfo(moduleName, moduleType)
                    {
                        Ref = @ref,
                        InitializationMode = GetPackageInitializationMode(package),
                        DependsOn = dependsOn
                    };
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the available latest version of packaged modules removing.
        /// </summary>
        /// <returns>The packaged modules.</returns>
        private IEnumerable<ModuleInfo> GetPackagedModules()
        {
            var packageRepositories = new List<IPackageRepository>(new[] { this.GetPackageRepository() });

            var moduleInfos = new List<ModuleInfo>();

            foreach (var packageRepository in packageRepositories)
            {
                Log.Debug("Retrieving packaged modules from '{0}'", packageRepository.Source);

                try
                {
                    var queryablePackages = packageRepository.GetPackages();
                    var packages = GetFilteredPackagedModules(queryablePackages);

                    foreach (var package in packages)
                    {
                        moduleInfos.Add(_moduleInfoCacheStoreCacheStorage.GetFromCacheOrFetch(package.Id, () => CreatePackageModule(package)));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to retrieve packaged modules from '{0}'", packageRepository.Source);
                }
            }

            return moduleInfos;
        }

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        public IPackageRepository GetPackageRepository()
        {
            return _behavior.GetPackageRepository();
        }
        #endregion
    }
}

#endif