// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Caching;
    using Logging;
    using Reflection;

#if UWP
    using ResourceManager = Windows.ApplicationModel.Resources.ResourceLoader;
    using Windows.ApplicationModel.Resources.Core;
    using Windows.Storage;
#else
    using ResourceManager = System.Resources.ResourceManager;
#endif

    public partial class LanguageService
    {
#if UWP
        private static readonly string StartupAssemblyName = AssemblyHelper.GetEntryAssembly().GetName().Name;
#endif

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
        public override string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
        {
            Argument.IsNotNull("languageSource", languageSource);
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);
            Argument.IsNotNull("cultureInfo", cultureInfo);
            
            string value = null;
            var source = languageSource.GetSource();
            var resourceLoader = GetResourceManager(source);

            if (resourceLoader is not null)
            {
#if UWP
                var resourceContainer = GetResourceContainer(source);

                // Try the language specific first
                var neutralSource = string.Format("{0}", resourceContainer);
                var cultureName = cultureInfo.Name;
                var languageSpecificSource = string.Format("{0}.{1}", resourceContainer, cultureName);

                var currentResourceManager = Windows.ApplicationModel.Resources.Core.ResourceManager.Current;

                var finalResourceMap = (from resourceMap in currentResourceManager.AllResourceMaps
                                        let rm = resourceMap.Value.GetSubtree(languageSpecificSource)
                                        where rm != null
                                        select rm).FirstOrDefault();

                if ((finalResourceMap is null) && !cultureInfo.IsNeutralCulture)
                {
                    cultureName = cultureInfo.Parent.Name;
                    languageSpecificSource = string.Format("{0}.{1}", resourceContainer, cultureName);

                    finalResourceMap = (from resourceMap in currentResourceManager.AllResourceMaps
                                        let rm = resourceMap.Value.GetSubtree(languageSpecificSource)
                                        where rm != null
                                        select rm).FirstOrDefault();
                }

                if (finalResourceMap is null)
                {
                    finalResourceMap = (from resourceMap in currentResourceManager.AllResourceMaps
                                        let rm = resourceMap.Value.GetSubtree(neutralSource)
                                        where rm != null
                                        select rm).FirstOrDefault();
                }

                if (finalResourceMap != null)
                {
                    var resourceContext = ResourceContext.GetForViewIndependentUse();
                    resourceContext.Languages = new[] { cultureName };

                    var resourceCandidate = finalResourceMap.GetValue(resourceName, resourceContext);
                    if (resourceCandidate != null)
                    {
                        value = resourceCandidate.ValueAsString;
                    }
                }
#else
                value = resourceLoader.GetString(resourceName, cultureInfo);
#endif
            }

            return value;
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
#if UWP
                    var resourceContainer = GetResourceContainer(source);
                    var resourceLoader = ResourceManager.GetForViewIndependentUse(resourceContainer);
#else
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

#if UWP
        private string GetResourceContainer(string source)
        {
            var splittedString = source.Split(new[] { "|" }, StringSplitOptions.None);
            var assemblyName = splittedString[0];
            var sourceName = splittedString[1];

            // Note: important to remove the current assembly, when reading from the current assembly, we cannot prefix [AssemblyName]/
            var resourceContainer = string.Empty;
            if (!string.Equals(assemblyName, StartupAssemblyName))
            {
                resourceContainer = string.Format("{0}/", assemblyName);
            }

            resourceContainer = string.Format("{0}{1}", resourceContainer, sourceName);

            return resourceContainer;
        }
#endif
    }
}

#endif
