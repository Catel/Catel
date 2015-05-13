// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteInstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NET

namespace Catel.Modules
{
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
        public IPackageRepository PackageRepository
        {
            get { return _moduleCatalog.GetPackageRepository(); }
        }
        #endregion

        #region Methods
        public override void Execute()
        {
            var packageManager = new PackageManager(PackageRepository, OutputDirectory);
            packageManager.InstallPackage(Package, IgnoreDependencies, AllowPrereleaseVersions);
        }
        #endregion
    }
}

#endif