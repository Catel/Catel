// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    using Catel.Caching;
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
        #endregion

        #region Fields

        /// <summary>
        /// Assembly ref cache storage
        /// </summary>
        private CacheStorage<ModuleInfo, string> _assemblyRefCacheStorage = new CacheStorage<ModuleInfo, string>(storeNullValues: true);

        /// <summary>
        /// Module info cache storage.
        /// </summary>
        private CacheStorage<string, ModuleInfo> _moduleInfoCacheStoreCacheStorage = new CacheStorage<string, ModuleInfo>(storeNullValues: true);
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
        #endregion

        #region Methods
        /// <summary>
        /// Determine whether module is already installed.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <returns><c>true</c> if the module is installed otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public string GetModuleAssemblyRef(ModuleInfo moduleInfo)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            return _assemblyRefCacheStorage.GetFromCacheOrFetch(moduleInfo, () =>
                {
                    var packageName = moduleInfo.GetPackageName();
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
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public string GetModuleAssemblyRef(ModuleInfo moduleInfo, SemanticVersion version)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            Argument.IsNotNull(() => moduleInfo);

            var key = string.Format(CultureInfo.InvariantCulture, "ModuleName:{0}; ModuleType:{1}; Ref:{2}; Version:{3}", moduleInfo.ModuleName, moduleInfo.ModuleType, moduleInfo.Ref, version);
            return GetModuleAssemblyRef(_moduleInfoCacheStoreCacheStorage.GetFromCacheOrFetch(key, () => new ModuleInfo(moduleInfo.ModuleName, moduleInfo.ModuleType) { Ref = string.Format("{0}, {1}", moduleInfo.GetPackageName().Id, version), DependsOn = moduleInfo.DependsOn }));
        }
        #endregion
    }
}