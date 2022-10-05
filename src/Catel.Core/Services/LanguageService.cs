namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Caching;
    using Logging;

    /// <summary>
    /// Service to implement the retrieval of language services.
    /// </summary>
    public partial class LanguageService : LanguageServiceBase, ILanguageService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<ILanguageSource> _languageSources = new List<ILanguageSource>();

        private readonly ICacheStorage<LanguageResourceKey, string?> _stringCache = new CacheStorage<LanguageResourceKey, string?>();

        private CultureInfo _fallbackCulture;
        private CultureInfo _preferredCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService"/> class.
        /// </summary>
        public LanguageService()
        {
            // Note: we don't have resources in Catel.Core at the moment
            //_languageSources.Add(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Resources"));
            //_languageSources.Add(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Exceptions"));

            _languageSources.Add(new LanguageResourceSource("Catel.MVVM", "Catel.MVVM.Properties", "Resources"));
            _languageSources.Add(new LanguageResourceSource("Catel.MVVM", "Catel.MVVM.Properties", "Exceptions"));

            _fallbackCulture = CultureInfo.CurrentUICulture;
            _preferredCulture = CultureInfo.CurrentUICulture;
            CacheResults = true;
        }

        /// <summary>
        /// Gets or sets the fallback culture.
        /// </summary>
        /// <value>The fallback culture.</value>
        public CultureInfo FallbackCulture
        {
            get { return _fallbackCulture; }
            set
            {
                _fallbackCulture = value;
                LanguageUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the preferred culture.
        /// </summary>
        /// <value>The preferred culture.</value>
        public CultureInfo PreferredCulture
        {
            get { return _preferredCulture; }
            set
            {
                _preferredCulture = value;
                LanguageUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the results should be cached.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the results should be cached; otherwise, <c>false</c>.</value>
        public bool CacheResults { get; set; }

        /// <summary>
        /// Occurs when the <see cref="FallbackCulture"/> or <see cref="PreferredCulture"/> are updated.
        /// </summary>
        public event EventHandler<EventArgs>? LanguageUpdated;
        
        /// <summary>
        /// Preloads the language sources to provide optimal performance.
        /// </summary>
        public virtual void PreloadLanguageSources()
        {
            Log.Debug("Preloading language sources");

            lock (_languageSources)
            {
                foreach (var languageSource in _languageSources)
                {
                    PreloadLanguageSource(languageSource);
                }
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
            ArgumentNullException.ThrowIfNull(languageSource);

            lock (_languageSources)
            {
                _languageSources.Insert(0, languageSource);
            }
        }

        /// <summary>
        /// Clears the language resources.
        /// </summary>
        public void ClearLanguageResources()
        {
            lock (_languageSources)
            {
                _languageSources.Clear();
            }
        }

        /// <summary>
        /// Gets the string with the <see cref="PreferredCulture" />. If the preferred language cannot be
        /// found, this method will use the <see cref="FallbackCulture" /> to retrieve the
        /// string.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>The string or <c>null</c> if the resource cannot be found.</returns>
        /// <exception cref="ArgumentException">The <paramref name="resourceName" /> is <c>null</c>.</exception>
        public string? GetString(string resourceName)
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
        public string? GetString(string resourceName, CultureInfo cultureInfo)
        {
            Argument.IsNotNullOrWhitespace("resourceName", resourceName);

            if (CacheResults)
            {
                var resourceKey = new LanguageResourceKey(resourceName, cultureInfo);
                return _stringCache.GetFromCacheOrFetch(resourceKey, () => GetStringInternal(resourceName, cultureInfo));
            }

            return GetStringInternal(resourceName, cultureInfo);
        }

        private string? GetStringInternal(string resourceName, CultureInfo cultureInfo)
        {
            lock (_languageSources)
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
            }

            return null;
        }
    }
}
