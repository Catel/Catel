// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.Extensions
{
    using Catel.Caching;

    using NuGet;

    /// <summary>
    /// NuGet based module catalog extensions.
    /// </summary>
    internal static class NuGetBasedModuleCatalogExtensions
    {
        #region Constants
        /// <summary>
        /// The package repository cache.
        /// </summary>
        private static readonly CacheStorage<string, IPackageRepository> PackageRepositoryCache = new CacheStorage<string, IPackageRepository>();
        #endregion

        #region Methods
        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        /// <returns>
        /// The <see cref="IPackageRepository"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleCatalog"/> is <c>null</c>.</exception>
        public static IPackageRepository GetPackageRepository(this NuGetBasedModuleCatalog moduleCatalog)
        {
            Argument.IsNotNull(() => moduleCatalog);

            return PackageRepositoryCache.GetFromCacheOrFetch(moduleCatalog.PackageSource, () => PackageRepositoryFactory.Default.CreateRepository(moduleCatalog.PackageSource));
        }
        #endregion
    }
}