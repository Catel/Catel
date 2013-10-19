// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetBasedModuleCatalogExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The nu get based module catalog extensions.
// </summary>
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
        #region Static Fields

        /// <summary>
        /// The package repository cache.
        /// </summary>
        private static readonly CacheStorage<string, IPackageRepository> PackageRepositoryCache = new CacheStorage<string, IPackageRepository>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the package repository.
        /// </summary>
        /// <param name="moduleCatalog">
        /// The module catalog.
        /// </param>
        /// <returns>
        /// The <see cref="IPackageRepository"/>.
        /// </returns>
        public static IPackageRepository GetPackageRepository(this NuGetBasedModuleCatalog moduleCatalog)
        {
            return PackageRepositoryCache.GetFromCacheOrFetch(moduleCatalog.PackageSource, () => PackageRepositoryFactory.Default.CreateRepository(moduleCatalog.PackageSource));
        }

        #endregion
    }
}