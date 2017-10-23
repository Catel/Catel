// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using Catel.Caching;
    using Catel.Logging;

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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
        /// dependency property in some platforms.
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

            return GetDependencyProperties(frameworkElement.GetType());
        }

        /// <summary>
        /// Gets all dependency properties of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        /// <returns>List containing all dependency properties of the specified <see cref="FrameworkElement"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static List<DependencyPropertyInfo> GetDependencyProperties(Type viewType)
        {
            Argument.IsNotNull("viewType", viewType);

            EnsureItemInCache(viewType);

            return _cacheByParentType[viewType];
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

            var viewType = frameworkElement.GetType();

            EnsureItemInCache(viewType);

            var propertyKey = GetDependencyPropertyCacheKey(viewType, propertyName);

            if (_cacheByPropertyName.TryGetValue(propertyKey, out DependencyProperty dependencyProperty))
            {
                return dependencyProperty;
            }

            return null;
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

            EnsureItemInCache(frameworkElement.GetType());

            if (_cacheByDependencyProperty.TryGetValue(dependencyProperty, out string name))
            {
                return name;
            }

            return null;
        }

        /// <summary>
        /// Gets the dependency property cache key prefix.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        /// <returns>The dependency property cache key prefix based on the framework element..</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        public static string GetDependencyPropertyCacheKeyPrefix(Type viewType)
        {
            Argument.IsNotNull("viewType", viewType);

            return _cacheKeyCache.GetFromCacheOrFetch(viewType, () => viewType.FullName.Replace(".", "_"));
        }

        /// <summary>
        /// Gets the dependency property key for the cache.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The key to use in the cache.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static string GetDependencyPropertyCacheKey(Type viewType, string propertyName)
        {
            Argument.IsNotNull("viewType", viewType);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return string.Format("{0}_{1}", GetDependencyPropertyCacheKeyPrefix(viewType), propertyName);
        }

        /// <summary>
        /// Ensures that the dependency properties of the specified <see cref="FrameworkElement"/> are in the cache.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewType"/> is <c>null</c>.</exception>
        private static void EnsureItemInCache(this Type viewType)
        {
            if (_cacheByParentType.ContainsKey(viewType))
            {
                return;
            }

            var properties = new List<DependencyPropertyInfo>();

            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, true);

            var typeMembers = new List<MemberInfo>();
            typeMembers.AddRange(viewType.GetFieldsEx(bindingFlags));
            typeMembers.AddRange(viewType.GetPropertiesEx(bindingFlags));

            foreach (var member in typeMembers)
            {
                try
                {
                    var fieldInfo = member as FieldInfo;
                    var propertyInfo = member as PropertyInfo;

                    if (fieldInfo != null)
                    {
                        if (!typeof(DependencyProperty).IsAssignableFromEx(fieldInfo.FieldType))
                        {
                            continue;
                        }
                    }
                    else if (propertyInfo != null)
                    {
                        if (!typeof(DependencyProperty).IsAssignableFromEx(propertyInfo.PropertyType))
                        {
                            continue;
                        }
                    }

                    string name = member.Name;

                    int propertyPostfixIndex = name.LastIndexOf("Property", StringComparison.Ordinal);
                    if (propertyPostfixIndex != -1)
                    {
                        name = name.Substring(0, propertyPostfixIndex);
                    }

                    if (string.Equals(name, "__Direct") || string.Equals(name, "DirectDependency"))
                    {
                        continue;
                    }

                    DependencyProperty dependencyProperty;
                    if (fieldInfo != null)
                    {
                        var fieldValue = fieldInfo.GetValue(null);
                        dependencyProperty = fieldValue as DependencyProperty;
                    }
                    else if (propertyInfo != null)
                    {
#if NETFX_CORE
                        var propertyValue = propertyInfo.GetValue(null);
                        dependencyProperty = propertyValue as DependencyProperty;
#else
                        var propertyValue = propertyInfo.GetValue(null, null);
                        dependencyProperty = propertyValue as DependencyProperty;
#endif
                    }
                    else
                    {
                        continue;
                    }

                    if (dependencyProperty == null)
                    {
                        continue;
                    }

                    properties.Add(new DependencyPropertyInfo(dependencyProperty, name));

                    string propertyKey = GetDependencyPropertyCacheKey(viewType, name);
                    _cacheByPropertyName[propertyKey] = dependencyProperty;
                    _cacheByDependencyProperty[dependencyProperty] = name;
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"Failed to enumerate member '{member.Name}' as dependency property");
                }
            }

            _cacheByParentType[viewType] = properties;
        }
    }
}

#endif