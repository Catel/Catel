// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInfoExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Modules.Extensions
{
    using System.Linq;

    using Catel.Caching;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The module info extensions.
    /// </summary>
    internal static class ModuleInfoExtensions
    {
        #region Constants
        /// <summary>
        /// The package name cache storage.
        /// </summary>
        private static readonly CacheStorage<ModuleInfo, PackageName> PackageNameCacheStorage = new CacheStorage<ModuleInfo, PackageName>();
        #endregion

        #region Methods
        /// <summary>
        /// The get package name.
        /// </summary>
        /// <param name="moduleInfo">
        /// The module info.
        /// </param>
        /// <returns>
        /// The <see cref="PackageName" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        public static PackageName GetPackageName(this ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);

            return PackageNameCacheStorage.GetFromCacheOrFetch(moduleInfo, () =>
                {
                    string packageId = string.Empty;
                    string packageVersion = string.Empty;
                    string @ref = moduleInfo.Ref;
                    if (@ref.Contains(','))
                    {
                        string[] strings = @ref.Split(',');
                        packageId = strings[0].Trim();
                        packageVersion = strings[1].Trim();
                    }

                    PackageName packageName = null;
                    if (!string.IsNullOrEmpty(packageId))
                    {
                        packageName = new PackageName(packageId, new SemanticVersion(packageVersion));
                    }

                    return packageName;
                });
        }
        #endregion
    }
}