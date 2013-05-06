// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data
{
    using System;
    using System.Collections.Generic;

    using Catel.Caching;

    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Helper class for dependency properties.
    /// </summary>
    public static class DependencyPropertyHelper
    {
        /// <summary>
        /// Cache containing all dependency properties of a specific type.
        /// </summary>
        private static readonly Dictionary<Type, List<DependencyPropertyInfo>> _cacheByParentType = new Dictionary<Type, List<DependencyPropertyInfo>>();

        /// <summary>
        /// Cache containing a dependency property based on the type + propertyname, where the key is generated using the
        /// <see cref="GetDependencyPropertyCacheKey"/> method.
        /// </summary>
        private static readonly Dictionary<string, DependencyProperty> _cacheByPropertyName = new Dictionary<string, DependencyProperty>();

        /// <summary>
        /// Cache containing the names of all found dependency properties, required because it is not possible to get the name of a 
        /// dependency property in Silverlight and WP7.
        /// </summary>
        private static readonly Dictionary<DependencyProperty, string> _cacheByDependencyProperty = new Dictionary<DependencyProperty, string>();

        /// <summary>
        /// The cache for the cache keys.
        /// </summary>
        private static readonly ICacheStorage<Type, string> _cacheKeyCache = new CacheStorage<Type, string>();  

        /// <summary>
        /// Gets all dependency properties of the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>List containing all dependency properties of the specified <see cref="FrameworkElement"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        public static List<DependencyPropertyInfo> GetDependencyProperties(this FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            EnsureItemInCache(frameworkElement);

            return _cacheByParentType[frameworkElement.GetType()];
        }

        /// <summary>
        /// Gets a the dependency property of a specific <see cref="FrameworkElement"/> by its name.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The <see cref="DependencyProperty"/> or <c>null</c> if the property cannot be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static DependencyProperty GetDependencyPropertyByName(this FrameworkElement frameworkElement, string propertyName)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            EnsureItemInCache(frameworkElement);

            var propertyKey = GetDependencyPropertyCacheKey(frameworkElement, propertyName);

            return _cacheByPropertyName.ContainsKey(propertyKey) ? _cacheByPropertyName[propertyKey] : null;
        }

        /// <summary>
        /// Gets the name of the specified dependency property.
        /// </summary>
        /// <param name="frameworkElement">The framework element containing the dependency property.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <returns>The name of the dependency property or <c>null</c> if the name could not be found.</returns>
        public static string GetDependencyPropertyName(this FrameworkElement frameworkElement, DependencyProperty dependencyProperty)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("dependencyProperty", dependencyProperty);

            EnsureItemInCache(frameworkElement);

            return _cacheByDependencyProperty.ContainsKey(dependencyProperty) ? _cacheByDependencyProperty[dependencyProperty] : null;
        }

        /// <summary>
        /// Gets the dependency property cache key prefix.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>The dependency property cache key prefix based on the framework element..</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        public static string GetDependencyPropertyCacheKeyPrefix(FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            var frameworkElementType = frameworkElement.GetType();

            return _cacheKeyCache.GetFromCacheOrFetch(frameworkElementType, () => frameworkElement.GetType().FullName.Replace(".", "_"));
        }

        /// <summary>
        /// Gets the dependency property key for the cache.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The key to use in the cache.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static string GetDependencyPropertyCacheKey(FrameworkElement frameworkElement, string propertyName)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return string.Format("{0}_{1}", GetDependencyPropertyCacheKeyPrefix(frameworkElement), propertyName);
        }

        /// <summary>
        /// Ensures that the dependency properties of the specified <see cref="FrameworkElement"/> are in the cache.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        private static void EnsureItemInCache(this FrameworkElement frameworkElement)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);

            var type = frameworkElement.GetType();
            if (_cacheByParentType.ContainsKey(type))
            {
                return;
            }

            var properties = new List<DependencyPropertyInfo>();

            var parentType = type;
            var fields = parentType.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true));
            foreach (var field in fields)
            {
                if (typeof(DependencyProperty).IsAssignableFromEx(field.FieldType))
                {
                    var names = field.ToString().Split(new[] { ' ' });
                    var name = names[names.Length - 1];

                    int propertyPostfixIndex = name.LastIndexOf("Property");
                    if (propertyPostfixIndex != -1)
                    {
                        name = name.Substring(0, propertyPostfixIndex);
                    }

                    if (string.Equals(name, "__Direct") || string.Equals(name, "DirectDependency"))
                    {
                        continue;
                    }
                        
                    var dependencyProperty = (DependencyProperty) field.GetValue(frameworkElement);
                    properties.Add(new DependencyPropertyInfo(dependencyProperty, name));

                    var propertyKey = GetDependencyPropertyCacheKey(frameworkElement, name);
                    _cacheByPropertyName[propertyKey] = dependencyProperty;
                    _cacheByDependencyProperty[dependencyProperty] = name;
                }
            }

            _cacheByParentType[type] = properties;
        }
    }
}