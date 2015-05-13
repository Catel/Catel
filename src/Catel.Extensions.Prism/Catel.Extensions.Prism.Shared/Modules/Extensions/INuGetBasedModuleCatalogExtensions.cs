// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetBasedModuleCatalogExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet;

    /// <summary>
    /// INuGetBasedModuleCatalog eExtensions.
    /// </summary>
    public static class INuGetBasedModuleCatalogExtensions
    {
        /// <summary>
        /// Gets all package sources.
        /// </summary>
        /// <param name="moduleCatalog">The module catalog.</param>
        /// <param name="allowParentPackageSources">if set to <c>true</c>, allow other packages sources fetched via the parent.</param>
        /// <returns>IEnumerable&lt;IPackageSource&gt;.</returns>
        public static IEnumerable<IPackageRepository> GetAllPackageRepositories(this INuGetBasedModuleCatalog moduleCatalog, bool allowParentPackageSources)
        {
            Argument.IsNotNull("moduleCatalog", moduleCatalog);

            var packageRepositories = new List<IPackageRepository>();

            packageRepositories.AddRange(moduleCatalog.GetPackageRepositories());

            if (allowParentPackageSources)
            {
                var compositeNuGetBasedModuleCatalog = moduleCatalog.Parent as CompositeNuGetBasedModuleCatalog;
                if (compositeNuGetBasedModuleCatalog != null)
                {
                    var parentPackageRepositories = compositeNuGetBasedModuleCatalog.GetAllPackageRepositories(allowParentPackageSources).ToList();

                    // Reverse so we have the first parent first and the final parent last
                    parentPackageRepositories.Reverse();

                    foreach (var parentPackageRepository in parentPackageRepositories)
                    {
                        if (!packageRepositories.Contains(parentPackageRepository))
                        {
                            packageRepositories.Add(parentPackageRepository);
                        }
                    }
                }
            }

            return packageRepositories;
        }
    }
}

#endif