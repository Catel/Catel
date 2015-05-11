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
    using System.Runtime.Versioning;
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
        /// <param name="assemblyFileRef"></param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="package" /> is <c>null</c>.</exception>
        public RemoteInstallPackageRequest(INuGetBasedModuleCatalog moduleCatalog, IPackage package, string assemblyFileRef)
            : base(assemblyFileRef)
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

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        public IEnumerable<IPackageRepository> PackageRepositories
        {
            get
            {
                var packageRepositories = new List<IPackageRepository>();

                packageRepositories.AddRange(_moduleCatalog.GetPackageRepositories());

                var parent = _moduleCatalog.Parent;
                if (parent != null)
                {
                    var parentPackagesRepositories = parent.GetPackageRepositories();
                    foreach (var parentPackagesRepository in parentPackagesRepositories)
                    {
                        if (!packageRepositories.Any(x => string.Equals(x.Source, parentPackagesRepository.Source)))
                        {
                            packageRepositories.Add(parentPackagesRepository);
                        }
                    }
                }

                return packageRepositories;
            }
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var package = Package;
            var packageRepositories = PackageRepositories.ToList();

            var packagesToDownload = ResolveDependencies(package.Id, packageRepositories);

            foreach (var packageToDownload in packagesToDownload)
            {
                Log.Debug("Installing package '{0}' from source '{1}'", packageToDownload.Package.Id, packageToDownload.PackageRepository.Source);

                var packageManager = new PackageManager(packageToDownload.PackageRepository, OutputDirectory);
                packageManager.InstallPackage(packageToDownload.Package, IgnoreDependencies, AllowPrereleaseVersions);
            }
        }

        private List<NuGetPackageInfo> ResolveDependencies(string packageId, IEnumerable<IPackageRepository> packageRepositories)
        {
            Log.Debug("Resolving dependencies for package '{0}'", packageId);

            var packagesToRetrieve = new List<NuGetPackageInfo>();
            IPackage packageToInstall = null;

            foreach (var packageRepository in packageRepositories)
            {
                try
                {
                    packageToInstall = packageRepository.GetPackages().FirstOrDefault(x => x.Id == packageId);
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
                    var resolvedDependencies = ResolveDependencies(dependency.Id, packageRepositories);
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