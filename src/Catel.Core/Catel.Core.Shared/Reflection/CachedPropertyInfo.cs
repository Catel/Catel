// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedPropertyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Reflection
{
    using System;
    using System.Reflection;
    using Catel.Caching;

    /// <summary>
    /// Cached implementation of the <see cref="PropertyInfo"/>.
    /// </summary>
    public class CachedPropertyInfo
    {
        private readonly Lazy<bool> _publicGetter;
        private readonly Lazy<bool> _publicSetter;

        private readonly ICacheStorage<Type, bool> _decoratedWithAttributeCache = new CacheStorage<Type, bool>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertyInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        public CachedPropertyInfo(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull("propertyInfo", propertyInfo);

            PropertyInfo = propertyInfo;

            _publicGetter = new Lazy<bool>(() =>
            {
#if NETFX_CORE
                var getMethod = propertyInfo.GetMethod;
#else
                var getMethod = propertyInfo.GetGetMethod(false);
#endif

                return getMethod != null && getMethod.IsPublic;
            });

            _publicSetter = new Lazy<bool>(() =>
            {
#if NETFX_CORE
                var setMethod = propertyInfo.SetMethod;
#else
                var setMethod = propertyInfo.GetSetMethod(false);
#endif

                return setMethod != null && setMethod.IsPublic;
            });
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Determines whether the property is decorated with the specified attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns><c>true</c> if the property is decorated with the specified attribute.; otherwise, <c>false</c>.</returns>
        public bool IsDecoratedWithAttribute(Type attributeType)
        {
            Argument.IsNotNull("attributeType", attributeType);

            return _decoratedWithAttributeCache.GetFromCacheOrFetch(attributeType, () =>
            {
                return AttributeHelper.IsDecoratedWithAttribute(PropertyInfo, attributeType);
            });
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a public getter.
        /// </summary>
        /// <value><c>true</c> if this instance has a public getter; otherwise, <c>false</c>.</value>
        public bool HasPublicGetter { get { return _publicGetter.Value; } }

        /// <summary>
        /// Gets a value indicating whether this instance has a public setter.
        /// </summary>
        /// <value><c>true</c> if this instance has a public setter; otherwise, <c>false</c>.</value>
        public bool HasPublicSetter { get { return _publicSetter.Value; } }
    }
}