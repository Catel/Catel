// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlLocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;

    using Catel.Services;

    using Logging;
    using Reflection;

#if XAMARIN

#else
    using Windows;
#endif

    /// <summary>
    /// Locator for urls.
    /// </summary>
    public class UrlLocator : LocatorBase, IUrlLocator
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Registers the specified url in the local cache. This cache will also be used by the <see cref="ResolveUrl"/>
        /// method.
        /// </summary>
        /// <param name="viewModelType">The view model to resolve the url for.</param>
        /// <param name="url">The resolved url.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="url"/> is <c>null</c> or whitespace.</exception>
        public void Register(Type viewModelType, string url)
        {
            Argument.IsNotNull("viewModelType", viewModelType);
            Argument.IsNotNullOrWhitespace("url", url);

            var typeName = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);

            Register(typeName, url);
        }

        /// <summary>
        /// Resolves an url by the view model and the registered <see cref="ILocator.NamingConventions"/>.
        /// </summary>
        /// <param name="viewModelType">Type of the view model to resolve the url for.</param>
        /// <param name="ensurePageExists">If set to <c>true</c>, the method checks whether the page resource actually exists.</param>
        /// <returns>The resolved url or <c>null</c> if the view could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModelType"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="ILocator.NamingConventions"/> are changed. If the <see cref="ILocator.NamingConventions"/> are changed,
        /// the cache must be cleared manually.
        /// </remarks>
        public virtual string ResolveUrl(Type viewModelType, bool ensurePageExists = true)
        {
            Argument.IsNotNull("viewModelType", viewModelType);

            string assembly = TypeHelper.GetAssemblyName(viewModelType.AssemblyQualifiedName);
            string viewModelTypeName = viewModelType.Name;

            var viewModelTypeNameWithAssembly = TypeHelper.GetTypeNameWithAssembly(viewModelType.AssemblyQualifiedName);

            var itemInCache = GetItemFromCache(viewModelTypeNameWithAssembly);
            if (itemInCache != null)
            {
                return itemInCache;
            }

            foreach (var convention in NamingConventions)
            {
                string viewUri = NamingConvention.ResolveViewByViewModelName(assembly, viewModelTypeName, convention);

                if (!ensurePageExists)
                {
                    AddItemToCache(viewModelTypeNameWithAssembly, viewUri);
                    return viewUri;
                }

#if XAMARIN

                throw new MustBeImplementedException();
#else
                string shortAssemblyName = TypeHelper.GetAssemblyNameWithoutOverhead(viewModelType.GetAssemblyFullNameEx());
                var viewAsResourceUri = ResourceHelper.GetResourceUri(viewUri, shortAssemblyName);

                if (ResourceHelper.XamlPageExists(viewAsResourceUri))
                {
                    Log.Debug("Found view '{0}' for '{1}' via naming convention '{2}'", viewUri, viewModelTypeName, convention);
                    AddItemToCache(viewModelTypeNameWithAssembly, viewUri);
                    return viewUri;
                }
#endif
            }

            Log.Warning("Tried resolving the view for '{0}' via all naming conventions, but it did not succeed", viewModelTypeName);
            return null;
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        /// <param name="assembly">The assembly name.</param>
        /// <param name="typeToResolveName">The full type name of the type to resolve.</param>
        /// <param name="namingConvention">The naming convention to use for resolving.</param>
        /// <returns>Nothing, this method throws a <see cref="NotSupportedException"/>.</returns>
        protected override string ResolveNamingConvention(string assembly, string typeToResolveName, string namingConvention)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the default naming conventions.
        /// </summary>
        /// <returns>An enumerable of default naming conventions.</returns>
        protected override IEnumerable<string> GetDefaultNamingConventions()
        {
            var namingConventions = new List<string>();

            namingConventions.Add(string.Format("/Views/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Views/{0}View.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Views/{0}Control.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Views/{0}Page.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Views/{0}Window.xaml", NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("/Controls/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Controls/{0}Control.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Pages/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Pages/{0}Page.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Windows/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/Windows/{0}Window.xaml", NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("/UI.Views/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Views/{0}View.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Views/{0}Control.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Views/{0}Page.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Views/{0}Window.xaml", NamingConvention.ViewModelName));

            namingConventions.Add(string.Format("/UI.Controls/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Controls/{0}Control.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Pages/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Pages/{0}Page.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Windows/{0}.xaml", NamingConvention.ViewModelName));
            namingConventions.Add(string.Format("/UI.Windows/{0}Window.xaml", NamingConvention.ViewModelName));

            return namingConventions;
        }
    }
}