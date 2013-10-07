// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallPackageRequest.cs" company="">
//   
// </copyright>
// <summary>
//   The install package request.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.ModuleManager
{
    using System.Diagnostics;
    using System.IO;

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
        private readonly NuGetBasedModuleCatalog moduleCatalog;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallPackageRequest"/> class.
        /// </summary>
        /// <param name="packageRepository">
        /// The package repository.
        /// </param>
        /// <param name="package">
        /// The package.
        /// </param>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        public InstallPackageRequest(
            IPackageRepository packageRepository, 
            IPackage package, 
            NuGetBasedModuleCatalog moduleCatalog)
        {
            this.PackageRepository = packageRepository;
            this.Package = package;
            this.moduleCatalog = moduleCatalog;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether allow prerelease versions.
        /// </summary>
        public bool AllowPrereleaseVersions
        {
            get
            {
                return this.moduleCatalog.AllowPrereleaseVersions;
            }
        }

        /// <summary>
        /// Gets the framework name identifier.
        /// </summary>
        public string FrameworkNameIdentifier
        {
            get
            {
                return this.moduleCatalog.FrameworkNameIdentifier;
            }
        }

        /// <summary>
        /// Gets a value indicating whether ignore dependencies.
        /// </summary>
        public bool IgnoreDependencies
        {
            get
            {
                return this.moduleCatalog.IgnoreDependencies;
            }
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                string outputDirectory = this.moduleCatalog.OutputDirectory;
                if (!Path.IsPathRooted(outputDirectory))
                {
                    string mainModuleDirectory = IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    outputDirectory = IO.Path.Combine(mainModuleDirectory, this.moduleCatalog.OutputDirectory);
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
        public IPackageRepository PackageRepository { get; private set; }

        #endregion
    }
}