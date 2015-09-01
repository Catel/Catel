// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#if NET

namespace Catel.Modules
{
    using System.Collections.Generic;
    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The INuGetBasedModuleCatalog interface.
    /// </summary>
    public interface INuGetBasedModuleCatalog : IModuleCatalog
    {
        #region Methods
        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo" />.
        /// </summary>
        /// <param name="moduleInfo">The module info.</param>
        /// <param name="installPackageRequest">The install package request.</param>
        /// <returns><c>true</c> whether the install package request is created, otherwise <c>false</c></returns>
        bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest);
        #endregion

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        string PackagedModuleIdFilterExpression { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prerelease versions are allowed.
        /// </summary>
        /// <value><c>true</c> if prerelease versions are allowed; otherwise, <c>false</c>.</value>
        bool AllowPrereleaseVersions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether dependencies should be ignored.
        /// </summary>
        /// <value><c>true</c> if dependencies should be ignored; otherwise, <c>false</c>.</value>
        bool IgnoreDependencies { get; set; }

        /// <summary>
        /// Gets the output directory full path.
        /// </summary>
        /// <value>The output directory full path.</value>
        string OutputDirectoryFullPath { get; }

        /// <summary>
        ///  Gets or sets the output directory.
        /// </summary>
        string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the parent nuget based module catalog.
        /// </summary>
        INuGetBasedModuleCatalog Parent { get; set; }

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <returns>The <see cref="IPackageRepository" />.</returns>
        IEnumerable<IPackageRepository> GetPackageRepositories();
    }
}

#endif