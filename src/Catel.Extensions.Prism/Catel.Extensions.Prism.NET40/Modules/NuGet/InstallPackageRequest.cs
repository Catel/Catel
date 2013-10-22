// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPackageRequest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules
{
    using System.Diagnostics;
    using System.IO;

    using Catel.Modules.Extensions;

    using NuGet;

    /// <summary>
    /// The install package request.
    /// </summary>
    internal class InstallPackageRequest
    {
        #region Fields
        /// <summary>
        /// The module catalog.
        /// </summary>
        private readonly NuGetBasedModuleCatalog _moduleCatalog;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPackageRequest" /> class.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <param name="package">The package.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="package" /> is <c>null</c>.</exception>
        public InstallPackageRequest(NuGetBasedModuleCatalog moduleCatalog, IPackage package)
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
            get
            {
                return _moduleCatalog.AllowPrereleaseVersions;
            }
        }

        /// <summary>
        /// Gets the framework name identifier.
        /// </summary>
        public string FrameworkNameIdentifier
        {
            get
            {
                return _moduleCatalog.FrameworkNameIdentifier;
            }
        }

        /// <summary>
        /// Gets a value indicating whether ignore dependencies.
        /// </summary>
        public bool IgnoreDependencies
        {
            get
            {
                return _moduleCatalog.IgnoreDependencies;
            }
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                string outputDirectory = _moduleCatalog.OutputDirectory;
                if (!Path.IsPathRooted(outputDirectory))
                {
                    string mainModuleDirectory = IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    outputDirectory = IO.Path.Combine(mainModuleDirectory, _moduleCatalog.OutputDirectory);
                }

                return outputDirectory;
            }
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
            get
            {
                return _moduleCatalog.GetPackageRepository();
            }
        }
        #endregion
    }
}