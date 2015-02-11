// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThemeHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel
{
    using System;
    using Catel.Logging;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Theme helper to ensure themes are loaded upon usage.
    /// </summary>
    public static class ThemeHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
            Argument.IsNotNull(() => resourceUri);

            try
            {
                var application = Application.Current;
                if (application != null)
                {
                    bool containsCatelDictionary = false;

                    var resources = application.Resources;
                    foreach (var resource in resources.MergedDictionaries)
                    {
                        var source = resource.Source;
                        if (source != null)
                        {
                            if (source.ToString().Contains("Catel"))
                            {
                                containsCatelDictionary = true;
                                break;
                            }
                        }
                    }

                    if (!containsCatelDictionary)
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
    }
}

#endif