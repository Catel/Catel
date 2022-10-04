namespace Catel.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Caching;
    using Logging;
    using Reflection;
    using ResourceManager = System.Resources.ResourceManager;

    public partial class LanguageService
    {
        private readonly ICacheStorage<string, ResourceManager> _resourceFileCache = new CacheStorage<string, ResourceManager>(storeNullValues: true);

        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        /// <param name="languageSource">The language source.</param>    
        protected override void PreloadLanguageSource(ILanguageSource languageSource)
        {
            GetResourceManager(languageSource.GetSource());
        }

        /// <summary>
        /// Gets the string from the specified resource file with the current culture.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the string cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="languageSource" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        public override string? GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);
               
            string? value = null;
            var source = languageSource.GetSource();
            var resourceLoader = GetResourceManager(source);

            if (resourceLoader is not null)
            {
                value = resourceLoader.GetString(resourceName, cultureInfo);
            }

            return value;
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <param name="source">The source.</param>
        private ResourceManager? GetResourceManager(string source)
        {
            Func<ResourceManager> retrievalFunc = () =>
            {
                try
                {
                    var splittedString = source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var assemblyName = splittedString[1].Trim();
                    var containingAssemblyName = string.Format("{0},", assemblyName);
                    var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies();

                    // Invert so design-time will always pick the latest version
                    loadedAssemblies.Reverse();

                    var assembly = loadedAssemblies.FirstOrDefault(x => x.FullName.StartsWith(containingAssemblyName));
                    if (assembly is null)
                    {
                        return null;
                    }

                    var resourceFile = splittedString[0];
                    var resourceLoader = new ResourceManager(resourceFile, assembly);

                    return resourceLoader;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to get the resource manager for source '{0}'", source);
                    return null;
                }
            };

            if (CacheResults)
            {
                return _resourceFileCache.GetFromCacheOrFetch(source, retrievalFunc);
            }

            return retrievalFunc();
        }
    }
}
