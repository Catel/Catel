// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetBasedModuleCatalog.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.Interfaces
{
    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The INuGetBasedModuleCatalog interface.
    /// </summary>
    public interface INuGetBasedModuleCatalog : IModuleCatalog
    {
        #region Methods
        /// <summary>
        /// Tries to create and install package request from the <paramref name="moduleInfo"/>.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module info.
        /// </param>
        /// <param name="installPackageRequest">
        /// The install package request.
        /// </param>
        /// <returns>
        /// <c>true</c> whether the install package request is created, otherwise <c>false</c>
        /// </returns>
        bool TryCreateInstallPackageRequest(ModuleInfo moduleInfo, out InstallPackageRequest installPackageRequest);
        #endregion

        /// <summary>
        /// The package module id filter expression.
        /// </summary>
        string PackagedModuleIdFilterExpression { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool AllowPrereleaseVersions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IgnoreDependencies { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
        /// <returns>
        /// The <see cref="IPackageRepository" />.
        /// </returns>
        IPackageRepository GetPackageRepository();
    }
}