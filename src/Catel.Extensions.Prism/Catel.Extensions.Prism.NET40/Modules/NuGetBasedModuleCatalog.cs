// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
            PackageSource = "https://nuget.org/api/v2/";
            OutputDirectory = "packages";
            AllowPrereleaseVersions = false;
            IgnoreDependencies = true;

#if NET40
            FrameworkNameIdentifier = "NET40";
#elif NET45 
            FrameworkNameIdentifier = "NET45";
#else
            FrameworkNameIdentifier = "SL5";
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
        #endregion
    }
}