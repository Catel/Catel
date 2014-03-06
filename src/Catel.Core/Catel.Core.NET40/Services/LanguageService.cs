// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Catel.Caching;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Services.Models;

#if NETFX_CORE
    using ResourceManager = Windows.ApplicationModel.Resources.ResourceLoader;
#else
    using ResourceManager = System.Resources.ResourceManager;
#endif

    /// <summary>
    /// Service to implement the retrieval of language services.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<ILanguageSource> _languageSources = new List<ILanguageSource>();

        private readonly ICacheStorage<string, ResourceManager> _resourceFileCache = new CacheStorage<string, ResourceManager>(storeNullValues: true);
        private readonly ICacheStorage<LanguageResourceKey, string> _stringCache = new CacheStorage<LanguageResourceKey, string>();

        private CultureInfo _fallbackCulture;
        private CultureInfo _preferredCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        public LanguageService()
        {
            _languageSources.Add(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Resources"));
            _languageSources.Add(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Exceptions"));

            _languageSources.Add(new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Resources"));
            _languageSources.Add(new LanguageResourceSource("Catel.MVVM", "Catel.Properties", "Exceptions"));

            _languageSources.Add(new LanguageResourceSource("Catel.Extensions.Controls", "Catel.Properties", "Resources"));
            _languageSources.Add(new LanguageResourceSource("Catel.Extensions.Controls", "Catel.Properties", "Exceptions"));

            FallbackCulture = CultureInfo.CurrentUICulture;
            PreferredCulture = CultureInfo.CurrentUICulture;
            CacheResults = true;
        }

        #region Properties
        /// <summary>
        /// Gets or sets the fallback culture.
        /// </summary>
        /// <value>The fallback culture.</value>
        public CultureInfo FallbackCulture
        {
            get
            {
                return _fallbackCulture;
            }
            set
            {
                _fallbackCulture = value;
                LanguageUpdated.SafeInvoke(this);
            }
        }

        /// <summary>
        /// Gets or sets the preferred culture.
        /// </summary>
        /// <value>The preferred culture.</value>
        public CultureInfo PreferredCulture
        {
            get
            {
                return _preferredCulture;
            }
            set
            {
                _preferredCulture = value;
                LanguageUpdated.SafeInvoke(this);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the results should be cached.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the results should be cached; otherwise, <c>false</c>.</value>
        public bool CacheResults { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the <see cref="FallbackCulture"/> or <see cref="PreferredCulture"/> are updated.
        /// </summary>
        public event EventHandler<EventArgs> LanguageUpdated;
        #endregion

        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        public virtual void PreloadLanguageSources()
        {
            Log.Debug("Preloading language sources");

            foreach (var languageSource in _languageSources)
            {
                GetResourceManager(languageSource.GetSource());
            }

            Log.Debug("Preloaded language sources");
        }

        /// <summary>
        /// Registers the language source.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="languageSource" /> is <c>null</c> or whitespace.</exception>
        public void RegisterLanguageSource(ILanguageSource languageSource)
        {
            Argument.IsNotNull(() => languageSource);

            _languageSources.Insert(0, languageSource);
        }

        /// <summary>
        /// Clears the language resources.
        /// </summary>
        public void ClearLanguageResources()
        {
            _languageSources.Clear();
        }

        /// <summary>
        /// Gets the string with the <see cref="PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="FallbackCulture" /> to retrieve the
        /// string.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public string GetString(string resourceName)
        {
            var preferredString = GetString(resourceName, PreferredCulture);
            if (string.IsNullOrWhiteSpace(preferredString))
            {
                preferredString = GetString(resourceName, FallbackCulture);
            }

            return preferredString;
        }

        /// <summary>
        /// Gets the string with the specified culture.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="cultureInfo" /> is <c>null</c>.</exception>
        public string GetString(string resourceName, CultureInfo cultureInfo)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);
            Argument.IsNotNull("cultureInfo", cultureInfo);

            if (CacheResults)
            {
                var resourceKey = new LanguageResourceKey(resourceName, cultureInfo);
                return _stringCache.GetFromCacheOrFetch(resourceKey, () => GetStringInternal(resourceName, cultureInfo));
            }

            return GetStringInternal(resourceName, cultureInfo);
        }

        private string GetStringInternal(string resourceName, CultureInfo cultureInfo)
        {
            foreach (var resourceFile in _languageSources)
            {
                try
                {
                    var value = GetString(resourceFile, resourceName, cultureInfo);
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to get string for resource name '{0}' from resource file '{1}'", resourceName, resourceFile);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the string from the specified resource file with the current culture.
        /// </summary>
        /// <param name="languageSource">The language source.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>The string or <c>null</c> if the string cannot be found.</returns>
        protected virtual string GetString(ILanguageSource languageSource, string resourceName, CultureInfo cultureInfo)
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

                        var assembly = loadedAssemblies.FirstOrDefault(x => x.FullName.Contains(containingAssemblyName));
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