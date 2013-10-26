// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.IO.Packaging;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Xml;
    using System.Xml.Linq;

    using Catel.Caching;
    using Catel.Caching.Policies;
    using Catel.Logging;
    using Catel.Modules.Extensions;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The nuget based module catelog.
    /// </summary>
    public sealed class NuGetBasedModuleCatalog : ModuleCatalog
    {
        #region Constants
        /// <summary>
        /// The relative Url Pattern
        /// </summary>
        public const string RelativeUrlPattern = "{0}/{1}/lib/{2}/{3}";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The framework identifier conversion map.
        /// </summary>
        private static readonly Dictionary<string, string> FrameworkIdentifierConversionMap = new Dictionary<string, string> { { ".NETFramework,Version=v4.0", "NET40" }, { ".NETFramework,Version=v4.5", "NET45" } };

        /// <summary>
        /// The package repository cache.
        /// </summary>
        private static readonly CacheStorage<string, IPackageRepository> PackageRepositoryCache = new CacheStorage<string, IPackageRepository>(storeNullValues: true);
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
        private readonly CacheStorage<string, IEnumerable<ModuleInfo>> _packagedModulesFilteredSearchCacheStorage = new CacheStorage<string, IEnumerable<ModuleInfo>>(() => ExpirationPolicy.Duration(TimeSpan.FromMinutes(2)), true);

        private string packagedModuleIdFilterExpression;
        #endregion

        #region Constructors
        /// <summary>
        /// The nuget based module catalog.
        /// </summary>
        public NuGetBasedModuleCatalog()
        {
            PackageSource = "https://nuget.org/api/v2/";
            OutputDirectory = "packages";
            AllowPrereleaseVersions = false;
            IgnoreDependencies = true;

#if NET40
            FrameworkNameIdentifier = "NET40";
#else 
            FrameworkNameIdentifier = "NET45";
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the package source.
        /// </summary>
        public string PackageSource { get; set; }

        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; }

        // TODO: throw an exception if a value is not compatible with the target platform. 
        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        /// <remarks>NuGet like framework name identifier string, for instance <c>NET35</c>, <c>NET40</c>, <c>NET45</c>, <c>SL4</c> and so on</remarks>
        public string FrameworkNameIdentifier { get; set; }

        /// <summary>
        /// Indicates whether the module catalog ignore the dependencies or not.
        /// </summary>
        public bool IgnoreDependencies { get; set; }

        /// <summary>
        /// Indicates whether the module catalog can download prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions { get; set; }

        /// <summary>
        /// Gets the full path to the output output directory.
        /// </summary>
        public string OutputDirectoryFullPath
        {
            get
            {
                string outputDirectory = OutputDirectory;
                if (!Path.IsPathRooted(outputDirectory))
                {
                    string mainModuleDirectory = IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    outputDirectory = IO.Path.Combine(mainModuleDirectory, OutputDirectory);
                }

                return outputDirectory;
            }
        }

        /// <summary>
        /// The output directory base Uri
        /// </summary>
        private string BaseUrl
        {
            get
            {
                return "file://" + OutputDirectoryFullPath.Replace("\\", "/");
            }
        }

        /// <summary>
        /// Enumerates the available modules.
        /// </summary>
        public override IEnumerable<ModuleInfo> Modules
        {
            get
            {
                foreach (var moduleInfo in base.Modules)
                {
                    yield return moduleInfo;
                }

                if (!string.IsNullOrWhiteSpace(PackagedModuleIdFilterExpression))
                {
                    foreach (var moduleInfo in PackagedModules)
                    {
                        yield return moduleInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates the available packaged modules.
        /// </summary>
        private IEnumerable<ModuleInfo> PackagedModules
        {
            get
            {
                return _packagedModulesFilteredSearchCacheStorage.GetFromCacheOrFetch(PackagedModuleIdFilterExpression, GetPackagedModules);
            }
        }

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        public string PackagedModuleIdFilterExpression
        {
            get
            {
                return packagedModuleIdFilterExpression;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    packagedModuleIdFilterExpression = value.Trim();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determine whether module is already installed.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <returns><c>true</c> if the module is installed otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        public bool IsModuleAssemblyInstalled(ModuleInfo moduleInfo)
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
        public string GetModuleAssemblyRef(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            return _assemblyRefCacheStorage.GetFromCacheOrFetch(moduleInfo, () =>
                {
                    PackageName packageName = moduleInfo.GetPackageName();
                    string directoryName = packageName.ToString().Replace(' ', '.');
                    return string.Format(CultureInfo.InvariantCulture, RelativeUrlPattern, BaseUrl, directoryName, FrameworkNameIdentifier, moduleInfo.GetAssemblyName());
                });
        }

        /// <summary>
        /// Gets the module assembly ref overriden the version number.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <param name="version">The version</param>
        /// <returns>The module assembly ref</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo" /> is <c>null</c>.</exception>
        public string GetModuleAssemblyRef(ModuleInfo moduleInfo, SemanticVersion version)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            var key = string.Format(CultureInfo.InvariantCulture, "ModuleName:{0}; ModuleType:{1}; Ref:{2}; Version:{3}", moduleInfo.ModuleName, moduleInfo.ModuleType, moduleInfo.Ref, version);
            return GetModuleAssemblyRef(_moduleInfoCacheStoreCacheStorage.GetFromCacheOrFetch(key, () => new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType) { Ref = string.Format("{0}, {1}", moduleInfo.GetPackageName().Id, version), DependsOn = moduleInfo.DependsOn }));
        }

        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module info.
        /// </param>
        /// <param name="installPackageRequest">
        /// The install package request.
        /// </param>
        /// <returns>
        /// <c>true</c> whether the install package request is created, otherwise <c>false</c>
        /// </returns>
        public bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest)
        {
            installPackageRequest = null;
            PackageName packageName = moduleInfo.GetPackageName();
            if (packageName.Version != null && IsModuleAssemblyInstalled(moduleInfo))
            {
                Log.Debug("Creating local install package request for '{0}'.", packageName.Name);

                installPackageRequest = new InstallPackageRequest(GetModuleAssemblyRef(moduleInfo));
            }
            else
            {
                IPackageRepository repository = GetPackageRepository();
                if (repository != null)
                {
                    Log.Debug("Looking for package '{0}' with version '{1}' on the repository '{2}'", packageName.Id, packageName.Version, PackageSource);

                    IPackage package;
                    if (repository.TryFindPackage(packageName.Id, packageName.Version, out package))
                    {
                        IEnumerable<FrameworkName> supportedFrameworks = package.GetSupportedFrameworks();
                        if (supportedFrameworks != null && supportedFrameworks.Any(name => FrameworkIdentifierConversionMap.ContainsKey(name.FullName) && FrameworkIdentifierConversionMap[name.FullName].Equals(FrameworkNameIdentifier)))
                        {
                            Log.Debug("Creating remote install package request for '{0}' from '{1}'", package.GetFullName(), PackageSource);

                            installPackageRequest = new RemoteInstallPackageRequest(this, package, GetModuleAssemblyRef(moduleInfo, package.Version));
                        }
                    }
                }
            }

            return installPackageRequest != null;
        }

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <returns>
        /// The <see cref="IPackageRepository" />.
        /// </returns>
        public IPackageRepository GetPackageRepository()
        {
            return PackageRepositoryCache.GetFromCacheOrFetch(PackageSource, () =>
                {
                    IPackageRepository packageRepository = null;
                    try
                    {
                        Log.Debug("Creating package repository with source '{0}'", PackageSource);

                        packageRepository = PackageRepositoryFactory.Default.CreateRepository(PackageSource);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    return packageRepository;
                });
        }

        /// <summary>
        /// Gets the available latest version of packaged modules removing.
        /// </summary>
        /// <summary>
        /// Gets the packaged modules from the repository.
        /// </summary>
        /// <returns>
        /// The packaged modules
        /// </returns>
        private IEnumerable<ModuleInfo> GetPackagedModules()
        {
            var packageRepository = GetPackageRepository();
            var packages = packageRepository.GetPackages().Where(package => package.Id.Contains(PackagedModuleIdFilterExpression)).ToList().GroupBy(package => package.Id).Select(packageGroup => packageGroup.ToList().OrderByDescending(package => package.Version).FirstOrDefault()).Where(package => package != null);
            foreach (var package in packages)
            {
                var moduleInfoFile = package.GetFiles().FirstOrDefault(file => file.Path.EndsWith("ModuleInfo.xml"));
                if (moduleInfoFile != null)
                {
                    string moduleType = string.Empty;
                    string moduleName = string.Empty;
                    try
                    {
                        var xDocument = XDocument.Load(new XmlTextReader(moduleInfoFile.GetStream()));
                        if (xDocument.Root != null)
                        {
                            var moduleNameElement = xDocument.Root.Element(XName.Get("ModuleName"));
                            if (moduleNameElement != null)
                            {
                                moduleName = moduleNameElement.Value;
                            }

                            var moduleTypeElement = xDocument.Root.Element(XName.Get("ModuleType"));
                            if (moduleTypeElement != null)
                            {
                                moduleType = moduleTypeElement.Value;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    if (!string.IsNullOrWhiteSpace(moduleName) && !string.IsNullOrWhiteSpace(moduleType))
                    {
                        var @ref = string.Format("{0}, {1}", package.Id, package.Version);
                        var key = string.Format(CultureInfo.InvariantCulture, "ModuleName:{0}; ModuleType:{1}; Ref:{2}; Version:{3}", moduleName, moduleType, @ref, package.Version);
                        yield return _moduleInfoCacheStoreCacheStorage.GetFromCacheOrFetch(key, () => new ModuleInfo(moduleName, moduleType) { Ref = @ref, InitializationMode = InitializationMode.OnDemand });
                    }
                }
            }
        }
        #endregion
    }
}