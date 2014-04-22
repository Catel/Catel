// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SILVERLIGHT || NETFX_CORE

namespace Catel.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Caching;
    using Logging;
    using Reflection;

#if NETFX_CORE
    using ResourceManager = Windows.ApplicationModel.Resources.ResourceLoader;
#else
    using ResourceManager = System.Resources.ResourceManager;
#endif

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
        protected override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            var source = languageSource.GetSource();
            var resourceLoader = GetResourceManager(source);

            if (resourceLoader != null)
            {
#if NETFX_CORE
                return resourceLoader.GetString(resourceName);
#else
                return resourceLoader.GetString(resourceName, cultureInfo);
#endif
            }

            return null;
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        /// <param name="source">The source.</param>
        private ResourceManager GetResourceManager(string source)
        {
            Func<ResourceManager> retrievalFunc = () =>
            {
                try
                {
#if NETFX_CORE && !WIN81
                    var resourceLoader = new ResourceManager(source);
#elif WIN81
                    var resourceLoader = ResourceManager.GetForCurrentView(source);
#else
                    var splittedString = source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var assemblyName = splittedString[1].Trim();
                    var containingAssemblyName = string.Format("{0},", assemblyName);

                    var loadedAssemblies = AssemblyHelper.GetLoadedAssemblies();

                    // Invert so design-time will always pick the latest version
                    loadedAssemblies.Reverse();

                    var assembly = loadedAssemblies.FirstOrDefault(x => x.FullName.StartsWith(containingAssemblyName));
                    if (assembly == null)
                    {
                        return null;
                    }

                    string resourceFile = splittedString[0];
                    var resourceLoader = new ResourceManager(resourceFile, assembly);
#endif

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

#endif