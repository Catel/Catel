namespace Catel.Modules
{
    /// <summary>
    /// The nuget based module catelog.
    /// </summary>
    public sealed class NuGetBasedModuleCatalog : ModuleCatalog
    {
        #region Constructors
        /// <summary>
        /// The nuget based module catalog.
        /// </summary>
        public NuGetBasedModuleCatalog()
        {
            this.PackageSource = "https://nuget.org/api/v2/";
            this.OutputDirectory = "packages";
            this.AllowPrereleaseVersions = false;
            this.IgnoreDependencies = true;
#if NET40
            this.FrameworkNameIdentifier = "NET40";
#elif NET45 
            this.FrameworkNameIdentifier = "NET45";
#elif SL4
            this.FrameworkNameIdentifier = "SL4";
#else
            this.FrameworkNameIdentifier = "SL5";
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the package source
        /// </summary>
        public string PackageSource { get; set; }

        /// <summary>
        ///  Gets or sets the output directory
        /// </summary>
        public string OutputDirectory { get; set; }

        // TODO: throw an exception if a value is not compatible with the target platform. 
        /// <summary>
        ///  Gets or sets the output directory
        /// </summary>
        /// <remarks>NuGet like framework name identifier string, for instance <c>NET35</c>, <c>NET40</c>, <c>NET45</c>, <c>SL4</c> and so on</remarks>
        public string FrameworkNameIdentifier { get; set; }

        /// <summary>
        /// </summary>
        public bool IgnoreDependencies { get; set; }

        /// <summary>
        /// </summary>
        public bool AllowPrereleaseVersions { get; set; }
        #endregion
    }
}