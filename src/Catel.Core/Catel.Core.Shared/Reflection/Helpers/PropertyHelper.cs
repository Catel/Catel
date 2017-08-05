// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Globalization;
    using System.Reflection;

    using Catel.Caching;
    using Collections;
    using Logging;

    /// <summary>
    /// Property helper class.
    /// </summary>
    public static partial class PropertyHelper
    {
        #region Constants
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ICacheStorage<string, PropertyInfo> _availableProperties = new CacheStorage<string, PropertyInfo>();
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified property is a public property on the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <returns><c>true</c> if the property is a public property on the specified object; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool IsPublicProperty(object obj, string property, bool ignoreCase = false)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            var propertyInfo = GetPropertyInfo(obj, property, ignoreCase);
            if (propertyInfo == null)
            {
                return false;
            }

#if NETFX_CORE || PCL
            var getMethod = propertyInfo.GetMethod;
#else
            var getMethod = propertyInfo.GetGetMethod();
#endif
            if (getMethod == null)
            {
                return false;
            }

            return getMethod.IsPublic;
        }

        /// <summary>
        /// Determines whether the specified property is available on the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <returns><c>true</c> if the property exists on the object type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool IsPropertyAvailable(object obj, string property, bool ignoreCase = false)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            return GetPropertyInfo(obj, property, ignoreCase) != null;
        }

        /// <summary>
        /// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
        /// is set to a default value and the method will return <c>false</c>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value as output parameter.</param>
        /// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool TryGetPropertyValue(object obj, string property, out object value)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            return TryGetPropertyValue<object>(obj, property, out value);
        }

        /// <summary>
        /// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
        /// is set to a default value and the method will return <c>false</c>.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <param name="value">The value as output parameter.</param>
        /// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool TryGetPropertyValue(object obj, string property, bool ignoreCase, out object value)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            return TryGetPropertyValue<object>(obj, property, ignoreCase, out value);
        }

        /// <summary>
        /// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
        /// is set to a default value and the method will return <c>false</c>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value as output parameter.</param>
        /// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool TryGetPropertyValue<TValue>(object obj, string property, out TValue value)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            return TryGetPropertyValue(obj, property, false, out value);
        }

        /// <summary>
        /// Tries to get the property value. If it fails, not exceptions will be thrown but the <paramref name="value" />
        /// is set to a default value and the method will return <c>false</c>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <param name="value">The value as output parameter.</param>
        /// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool TryGetPropertyValue<TValue>(object obj, string property, bool ignoreCase, out TValue value)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            return TryGetPropertyValue(obj, property, ignoreCase, false, out value);
        }

        /// <summary>
        /// Gets the property value of a specific object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <returns>The property value or <c>null</c> if no property can be found.</returns>
        /// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
        /// <exception cref="CannotGetPropertyValueException">The property value cannot be read.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static object GetPropertyValue(object obj, string property, bool ignoreCase = false)
        {
            return GetPropertyValue<object>(obj, property, ignoreCase);
        }

        /// <summary>
        /// Gets the property value of a specific object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <returns>The property value or <c>null</c> if no property can be found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
        /// <exception cref="CannotGetPropertyValueException">The property value cannot be read.</exception>
        public static TValue GetPropertyValue<TValue>(object obj, string property, bool ignoreCase = false)
        {
            TValue returnValue;

            TryGetPropertyValue(obj, property, ignoreCase, true, out returnValue);

            return returnValue;
        }

        private static bool TryGetPropertyValue<TValue>(object obj, string property, bool ignoreCase, bool throwOnException, out TValue value)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            value = default(TValue);

            var propertyInfo = GetPropertyInfo(obj, property, ignoreCase);
            if (propertyInfo == null)
            {
                if (throwOnException)
                {
                    throw Log.ErrorAndCreateException(s => new PropertyNotFoundException(property),
                        "Property '{0}' is not found on the object '{1}', probably the wrong field is being mapped", property, obj.GetType().Name);
                }

                return false;
            }

            // Return property value if available
            if (!propertyInfo.CanRead)
            {
                if (throwOnException)
                {
                    throw Log.ErrorAndCreateException(s => new CannotGetPropertyValueException(property), 
                        "Cannot read property {0}.'{1}'", obj.GetType().Name, property);
                }

                return false;
            }

#if NETFX_CORE || PCL
            value = (TValue)propertyInfo.GetValue(obj, null);
            return true;
#else
            try
            {
                value = (TValue)propertyInfo.GetValue(obj, null);
                return true;
            }
            catch (MethodAccessException)
            {
                if (throwOnException)
                {
                    throw Log.ErrorAndCreateException(s => new CannotGetPropertyValueException(property),
                        "Cannot read property {0}.'{1}'", obj.GetType().Name, property);
                }

                return false;
            }
#endif
        }

        /// <summary>
        /// Tries to set the property value. If it fails, no exceptions will be thrown, but <c>false</c> will
        /// be returned.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <returns><c>true</c> if the method succeeds; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static bool TrySetPropertyValue(object obj, string property, object value, bool ignoreCase = false)
        {
            return TrySetPropertyValue(obj, property, value, ignoreCase, false);
        }

        /// <summary>
        /// Sets the property value of a specific object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case when searching for the property name.</param>
        /// <exception cref="PropertyNotFoundException">The <paramref name="obj" /> is not found or not publicly available.</exception>
        /// <exception cref="CannotSetPropertyValueException">The the property value cannot be written.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        public static void SetPropertyValue(object obj, string property, object value, bool ignoreCase = false)
        {
            TrySetPropertyValue(obj, property, value, ignoreCase, true);
        }

        private static bool TrySetPropertyValue(object obj, string property, object value, bool ignoreCase, bool throwOnError)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);

            var propertyInfo = GetPropertyInfo(obj, property, ignoreCase);
            if (propertyInfo == null)
            {
                if (throwOnError)
                {
                    throw Log.ErrorAndCreateException(s => new PropertyNotFoundException(property),
                        "Property '{0}' is not found on the object '{1}', probably the wrong field is being mapped", property, obj.GetType().Name);
                }

                return false;
            }

            if (!propertyInfo.CanWrite)
            {
                if (throwOnError)
                {
                    throw Log.ErrorAndCreateException(s => new CannotSetPropertyValueException(property),
                        "Cannot write property {0}.'{1}'", obj.GetType().Name, property);
                }

                return false;
            }

#if NETFX_CORE || PCL
            propertyInfo.SetValue(obj, value, null);
#else

#if NET
            var setMethod = propertyInfo.GetSetMethod(true);
#else
            var setMethod = propertyInfo.GetSetMethod();
#endif
            if (setMethod == null)
            {
                if (throwOnError)
                {
                    throw Log.ErrorAndCreateException(s => new CannotSetPropertyValueException(property),
                        "Cannot write property {0}.'{1}', SetMethod is null", obj.GetType().Name, property);
                }

                return false;
            }

            setMethod.Invoke(obj, new[] { value });
#endif

            return true;
        }

#if !NETFX_CORE && !PCL
        /// <summary>
        /// Gets hidden property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the T value.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="property">The property.</param>
        /// <param name="baseType">The base Type.</param>
        /// <returns>``0.</returns>
        /// <exception cref="PropertyNotFoundException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="property" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="obj" /> is <c>null</c>.</exception>
        public static TValue GetHiddenPropertyValue<TValue>(object obj, string property, Type baseType)
        {
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNullOrWhitespace("property", property);
            Argument.IsNotNull("baseType", baseType);
            Argument.IsOfType("obj", obj, baseType);

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var propertyInfo = baseType.GetPropertyEx(property, bindingFlags);
            if (propertyInfo == null)
            {
                throw Log.ErrorAndCreateException(s => new PropertyNotFoundException(property),
                    "Hidden property '{0}' is not found on the base type '{1}'", property, baseType.GetType().Name);
            }

            return (TValue)propertyInfo.GetValue(obj, bindingFlags, null, ArrayShim.Empty<object>(), CultureInfo.InvariantCulture);
        }
#endif

        /// <summary>
        /// Gets the property info from the cache.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo GetPropertyInfo(object obj, string property, bool ignoreCase = false)
        {
            string cacheKey = $"{obj.GetType().FullName}_{property}_{ignoreCase}";
            return _availableProperties.GetFromCacheOrFetch(cacheKey, () =>
            {
                var objectType = obj.GetType();

                if (!ignoreCase)
                {
                    // Use old mechanism to ensure no breaking changes / performance hite
                    return objectType.GetPropertyEx(property);
                }

                var allProperties = objectType.GetPropertiesEx();

                foreach (var propertyInfo in allProperties)
                {
                    if (string.Equals(propertyInfo.Name, property, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        return propertyInfo;
                    }
                }

                return null;
            });
        }
        #endregion
    }
}