namespace Catel.Modules.Extensions
{
    using System.Linq;

    using Catel.Caching;

    using Microsoft.Practices.Prism.Modularity;

    using NuGet;

    internal static class ModuleInfoExtensions
    {
        #region Constants
        private static readonly CacheStorage<ModuleInfo, PackageName> PackageNameCacheStorage = new CacheStorage<ModuleInfo, PackageName>();
        #endregion

        #region Methods
        public static PackageName GetPackageName(this ModuleInfo moduleInfo)
        {
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