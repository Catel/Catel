// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteInstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NET

namespace Catel.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;

    using NuGet;

    /// <summary>
    /// The install package request.
    /// </summary>
    internal class RemoteInstallPackageRequest : InstallPackageRequest
    {
        #region Constants
        /// <summary>
        /// The Log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        /// <summary>
        /// The module catalog.
        /// </summary>
        private readonly INuGetBasedModuleCatalog _moduleCatalog;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteInstallPackageRequest" /> class.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <param name="package">The package.</param>
        /// <param name="moduleAssemblyRef"></param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="package" /> is <c>null</c>.</exception>
        public RemoteInstallPackageRequest(INuGetBasedModuleCatalog moduleCatalog, IPackage package, ModuleAssemblyRef moduleAssemblyRef)
            : base(moduleAssemblyRef)
        {
            Argument.IsNotNull(() => moduleCatalog);
            Argument.IsNotNull(() => package);

            _moduleCatalog = moduleCatalog;
            Package = package;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether allow prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get { return _moduleCatalog.AllowPrereleaseVersions; }
        }

        /// <summary>
        /// Gets a value indicating whether ignore dependencies.
        /// </summary>
        public bool IgnoreDependencies
        {
            get { return _moduleCatalog.IgnoreDependencies; }
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get { return _moduleCatalog.OutputDirectoryFullPath; }
        }

        /// <summary>
        /// Gets the package.
        /// </summary>
        public IPackage Package { get; private set; }
        #endregion

        #region Methods
        public override void Execute()
        {
            var package = Package;
            var packageRepositories = _moduleCatalog.GetAllPackageRepositories(true).ToList();

            var versionSpec = new VersionSpec
            {
                IsMinInclusive = true,
                MinVersion = package.Version,
                IsMaxInclusive = true,
                MaxVersion = package.Version
            };

            var packagesToDownload = ResolveDependencies(package.Id, versionSpec, packageRepositories);

            foreach (var packageToDownload in packagesToDownload)
            {
                Log.Debug("Installing package '{0}' from source '{1}'", packageToDownload.Package.Id, packageToDownload.PackageRepository.Source);

                var packageManager = new PackageManager(packageToDownload.PackageRepository, OutputDirectory);
                packageManager.InstallPackage(packageToDownload.Package, IgnoreDependencies, AllowPrereleaseVersions);
            }
        }

        private List<NuGetPackageInfo> ResolveDependencies(string packageId, IVersionSpec packageVersionSpec, IEnumerable<IPackageRepository> packageRepositories)
        {
            Log.Debug("Resolving dependencies for package '{0}'", packageId);

            var packagesToRetrieve = new List<NuGetPackageInfo>();
            IPackage packageToInstall = null;

            foreach (var packageRepository in packageRepositories)
            {
                try
                {
                    var query = packageRepository.GetPackages().Where(x => x.Id == packageId);

                    // TODO: optimize performance here
                    //if (packageVersionSpec.MinVersion != null)
                    //{
                    //    foreach (var version in x.Version.Get)

                    //    query = query.Where(x => x.Version.GetComparableVersionStrings() >= packageVersionSpec.MinVersion.V);
                    //}

                    //if (packageVersionSpec.MaxVersion != null)
                    //{
                    //    query = query.Where(x => x.Version <= packageVersionSpec.MaxVersion);
                    //}

                    // Note: FirstOrDefault not supported on GetPackages()
                    var possiblePackages = query.ToList().OrderBy(x => x.Version);

                    packageToInstall = (from possiblePackage in possiblePackages
                                        where (packageVersionSpec.MinVersion == null || possiblePackage.Version >= packageVersionSpec.MinVersion) &&
                                              (packageVersionSpec.MaxVersion == null || possiblePackage.Version <= packageVersionSpec.MaxVersion)
                                        select possiblePackage).FirstOrDefault();

                    if (packageToInstall != null)
                    {
                        packagesToRetrieve.Add(new NuGetPackageInfo(packageToInstall, packageRepository));
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to check if package '{0}' is available on package repository '{1}'", packageId, packageRepository.Source);
                }
            }

            if (packageToInstall == null)
            {
                Log.ErrorAndThrowException<NotSupportedException>("Package '{0}' is not found in any of the sources, cannot install package", packageId);
                return packagesToRetrieve;
            }

            if (IgnoreDependencies)
            {
                Log.Debug("Dependencies are ignored, only returning list of requested packages");
                return packagesToRetrieve;
            }

            foreach (var dependencySet in packageToInstall.DependencySets)
            {
                // TODO: check framework name
                //if (dependencySet.TargetFramework == new FrameworkName() )

                foreach (var dependency in dependencySet.Dependencies)
                {
                    var resolvedDependencies = ResolveDependencies(dependency.Id, dependency.VersionSpec, packageRepositories);
                    for (var i = resolvedDependencies.Count - 1; i >= 0; i--)
                    {
                        var resolvedDependency = resolvedDependencies[i];
                        if (!packagesToRetrieve.Any(x => string.Equals(x.Package.Id, resolvedDependency.Package.Id)))
                        {
                            packagesToRetrieve.Insert(0, resolvedDependency);
                        }
                    }
                }
            }

            return packagesToRetrieve;
        }
        #endregion
    }
}

#endif