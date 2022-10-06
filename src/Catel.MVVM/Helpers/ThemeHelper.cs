namespace Catel
{
    using System;
    using Caching;
    using Catel.Logging;
    using System.Windows;

    /// <summary>
    /// Theme helper to ensure themes are loaded upon usage.
    /// </summary>
    public static class ThemeHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ICacheStorage<Uri, bool> ThemeLoadedCache = new CacheStorage<Uri, bool>();

        /// <summary>
        /// Ensures that the Catel.MVVM theme is loaded.
        /// </summary>
        public static void EnsureCatelMvvmThemeIsLoaded()
        {
            EnsureThemeIsLoaded(new Uri("/Catel.MVVM;component/themes/generic.xaml", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Ensures that the specified theme is loaded.
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        public static void EnsureThemeIsLoaded(Uri resourceUri)
        {
            EnsureThemeIsLoaded(resourceUri, () =>
            {
                var application = Application.Current;
                if (application is null)
                {
                    return false;
                }

                var value = ThemeLoadedCache.GetFromCacheOrFetch(resourceUri, () => ContainsDictionary(application.Resources, resourceUri));

                // CTL-893: don't store "false" values, we are only interested in cached "true" values
                if (!value)
                {
                    ThemeLoadedCache.Remove(resourceUri);
                }

                return value;
            });
        }

        /// <summary>
        /// Ensures that the specified theme is loaded.
        /// </summary>
        /// <param name="resourceUri">The resource URI.</param>
        /// <param name="predicate">The predicate.</param>
        public static void EnsureThemeIsLoaded(Uri resourceUri, Func<bool> predicate)
        {
            Argument.IsNotNull("resourceUri", resourceUri);
            ArgumentNullException.ThrowIfNull(predicate);

            try
            {
                var application = Application.Current;
                if (application is not null)
                {
                    var resources = application.Resources;

                    if (!predicate())
                    {
                        Log.Info("Loading resource dictionary '{0}'", resourceUri.ToString());

                        resources.MergedDictionaries.Add(new ResourceDictionary
                        {
                            Source = resourceUri
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to resource dictionary '{0}'", resourceUri.ToString());
            }
        }

        private static bool ContainsDictionary(ResourceDictionary resourceDictionary, Uri resourceUri)
        {
            var source = resourceDictionary.Source;
            if (source is not null && source.ToString().EqualsIgnoreCase(resourceUri.ToString()))
            {
                return true;
            }

            foreach (var mergedDictionary in resourceDictionary.MergedDictionaries)
            {
                if (ContainsDictionary(mergedDictionary, resourceUri))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
