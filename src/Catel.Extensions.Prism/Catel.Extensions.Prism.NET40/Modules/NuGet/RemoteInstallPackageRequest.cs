namespace Catel.Modules
{
    using Catel.Logging;
 
    using NuGet;

    /// <summary>
    /// The install package request.
    /// </summary>
    internal class RemoteInstallPackageRequest : InstallPackageRequest
    {
        /// <summary>
        /// The Log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #region Fields
        /// <summary>
        /// The module catalog.
        /// </summary>
        private readonly NuGetBasedModuleCatalog _moduleCatalog;
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
        public RemoteInstallPackageRequest(NuGetBasedModuleCatalog moduleCatalog, IPackage package, string assemblyFileRef) : base(assemblyFileRef)
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
                return _moduleCatalog.OutputDirectoryFullPath;
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

        public override void Execute()
        {
            var package = Package;
            var packageManager = new PackageManager(PackageRepository, OutputDirectory);

            Log.Debug("Installing package '{0}' with version '{1}'", package.Id, package.Version);

            packageManager.InstallPackage(package, IgnoreDependencies, AllowPrereleaseVersions);
        }
        #endregion
    }
}