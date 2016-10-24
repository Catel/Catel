// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedPropertyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Cached implementation of the <see cref="PropertyInfo"/>.
    /// </summary>
    public class CachedPropertyInfo
    {
        private bool? _publicGetter;
        private bool? _publicSetter;

        // Don't instantiate yet, lazy-load when required
        private readonly Dictionary<Type, bool> _decoratedWithAttributeCache = new Dictionary<Type, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedPropertyInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        public CachedPropertyInfo(PropertyInfo propertyInfo)
        {
            Argument.IsNotNull("propertyInfo", propertyInfo);

            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>The property information.</value>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Determines whether the property is decorated with the specified attribute.
        /// </summary>
        /// <returns><c>true</c> if the property is decorated with the specified attribute.; otherwise, <c>false</c>.</returns>
        public bool IsDecoratedWithAttribute<TAttribute>()
        {
            return IsDecoratedWithAttribute(typeof(TAttribute));
        }

        /// <summary>
        /// Determines whether the property is decorated with the specified attribute.
        /// </summary>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns><c>true</c> if the property is decorated with the specified attribute.; otherwise, <c>false</c>.</returns>
        public bool IsDecoratedWithAttribute(Type attributeType)
        {
            Argument.IsNotNull("attributeType", attributeType);

            lock (_decoratedWithAttributeCache)
            {
                bool isDecorated;
                if (!_decoratedWithAttributeCache.TryGetValue(attributeType, out isDecorated))
                {
                    isDecorated = PropertyInfo.IsDecoratedWithAttribute(attributeType);
                    _decoratedWithAttributeCache[attributeType] = isDecorated;
                }

                return isDecorated;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a public getter.
        /// </summary>
        /// <value><c>true</c> if this instance has a public getter; otherwise, <c>false</c>.</value>
        public bool HasPublicGetter
        {
            get
            {
                if (!_publicGetter.HasValue)
                {
                    _publicGetter = GetPublicGetter();
                }

                return _publicGetter.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a public setter.
        /// </summary>
        /// <value><c>true</c> if this instance has a public setter; otherwise, <c>false</c>.</value>
        public bool HasPublicSetter
        {
            get
            {
                if (!_publicSetter.HasValue)
                {
                    _publicSetter = GetPublicSetter();
                }

                return _publicSetter.Value;
            }
        }

        private bool GetPublicGetter()
        {
            var propertyInfo = PropertyInfo;

#if NETFX_CORE || PCL
            var getMethod = propertyInfo.GetMethod;
#else
            var getMethod = propertyInfo.GetGetMethod(false);
#endif

            return getMethod != null && getMethod.IsPublic;
        }

        private bool GetPublicSetter()
        {
            var propertyInfo = PropertyInfo;

#if NETFX_CORE || PCL
            var setMethod = propertyInfo.SetMethod;
#else
            var setMethod = propertyInfo.GetSetMethod(false);
#endif

            return setMethod != null && setMethod.IsPublic;
        }
    }
}