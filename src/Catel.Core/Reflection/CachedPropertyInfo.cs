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
            lock (_decoratedWithAttributeCache)
            {
                if (!_decoratedWithAttributeCache.TryGetValue(attributeType, out var isDecorated))
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

            var getMethod = propertyInfo.GetGetMethod(false);

            return getMethod is not null && getMethod.IsPublic;
        }

        private bool GetPublicSetter()
        {
            var propertyInfo = PropertyInfo;

            var setMethod = propertyInfo.GetSetMethod(false);

            return setMethod is not null && setMethod.IsPublic;
        }
    }
}
