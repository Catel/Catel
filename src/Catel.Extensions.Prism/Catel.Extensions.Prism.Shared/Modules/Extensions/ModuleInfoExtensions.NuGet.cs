// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInfoExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Modules
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Catel.Caching;
    using Catel.Logging;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    /// <summary>
    /// The module info extensions.
    /// </summary>
    internal static partial class ModuleInfoExtensions
    {
        /// <summary>
        /// The type name pattern.
        /// </summary>
        private const string TypeNamePattern = "[^,]+,([^,]+).*";

        /// <summary>
        /// Module type must be specified using qualified name pattern error message
        /// </summary>
        private const string ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage = "The module type must be specified using a qualified name pattern";

        #region Constants
        /// <summary>
        /// The package name cache storage.
        /// </summary>
        private static readonly CacheStorage<ModuleInfo, PackageName> PackageNameCacheStorage = new CacheStorage<ModuleInfo, PackageName>(storeNullValues: true);

        /// <summary>
        /// The type name regex.
        /// </summary>
        private static readonly Regex TypeNameRegex = new Regex(TypeNamePattern, RegexOptions.Compiled);

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
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
                    string packageId;
                    string packageVersion = string.Empty;
                    string @ref = moduleInfo.Ref;
                    if (@ref.Contains(','))
                    {
                        var indexOf = @ref.IndexOf(',');
                        packageId = @ref.Substring(0, indexOf).Trim();
                        packageVersion = @ref.Substring(indexOf + 1).Trim();
                    }
                    else
                    {
                        packageId = @ref.Trim();
                    }

                    PackageName packageName = null;
                    if (!string.IsNullOrEmpty(packageId))
                    {
                        try
                        {
                            Log.Debug("Initializing package name from Id:'{0}' and Version:'{1}'", packageId, packageVersion);

                            packageName = new PackageName(packageId, string.IsNullOrEmpty(packageVersion) ? null : new SemanticVersion(packageVersion));
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }

                    return packageName;
                });
        }

        /// <summary>
        /// Gets the assembly name from the module info.
        /// </summary>
        /// <param name="moduleInfo">The module info</param>
        /// <returns>The assembly name</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="moduleInfo"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the module type doesn't match with the quelified name pattern.</exception>
        public static string GetAssemblyName(this ModuleInfo moduleInfo)
        {
            Argument.IsNotNull(() => moduleInfo);
          
            Match typeNameMatch = TypeNameRegex.Match(moduleInfo.ModuleType);
            if (!typeNameMatch.Success)
            {
                Log.Error(ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage);

                throw new InvalidOperationException(ModuleTypeMustBeSpecifiedUsingQualifiedNamePatternErrorMessage);
            }

            var assemblyName = typeNameMatch.Groups[1].Value.Trim();
            string assemblyFileName = assemblyName.Trim() + ".dll";

            return assemblyFileName;
        }
        #endregion
    }
}

#endif