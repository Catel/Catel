// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyData.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Reflection;
    using System.Xml.Serialization;
    using Catel.Caching;
    using Catel.Reflection;

    /// <summary>
    /// Object that contains all the property data that is used by the <see cref="ModelBase"/> class.
    /// </summary>
    public class PropertyData
    {
        #region Fields
        /// <summary>
        /// Type of the property.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private Type _type;

        /// <summary>
        /// Callback to use to create the default value.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Func<object> _createDefaultValue;

#if NET
        [field: NonSerialized]
#endif
        private readonly ICacheStorage<Type, CachedPropertyInfo> _cachedPropertyInfo = new CacheStorage<Type, CachedPropertyInfo>(storeNullValues: true);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyData" /> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase" />.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        internal PropertyData(string name, Type type, object defaultValue, bool setParent, EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
            : this(name, type, () => defaultValue, setParent, propertyChangedEventHandler, isSerializable,
                   includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyData"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for reference types).</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="createDefaultValue"/> is <c>null</c>.</exception>
        internal PropertyData(string name, Type type, Func<object> createDefaultValue, bool setParent, EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("createDefaultValue", createDefaultValue);

            Name = name;
            Type = type;
            SetParent = setParent;
            PropertyChangedEventHandler = propertyChangedEventHandler;
            IsSerializable = isSerializable;
            IncludeInSerialization = includeInSerialization;
            IsModelBaseProperty = isModelBaseProperty;
            IncludeInBackup = includeInBackup;
            IsCalculatedProperty = isCalculatedProperty;

            _createDefaultValue = createDefaultValue;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        [XmlIgnore]
        public Type Type
        {
            get { return _type ?? typeof(object); }
            private set { _type = value; }
        }

        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        [XmlIgnore]
        private object DefaultValue
        {
            get { return _createDefaultValue(); }
        }

        /// <summary>
        /// Gets a value indicating whether to set the parent after creating or deserializing the property.
        /// </summary>
        /// <value><c>true</c> if the parent of the should be set after creating or deserializing the property; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool SetParent { get; private set; }

        /// <summary>
        /// Gets a value indicating the property changed event handler.
        /// </summary>
        /// <value>The property changed event handler.</value>
        [XmlIgnore]
        internal EventHandler<AdvancedPropertyChangedEventArgs> PropertyChangedEventHandler { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this property is serializable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this property is serializable; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsSerializable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether whether the property should be included in the serialization.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property should be included in the serialization; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IncludeInSerialization { get; private set; }

        /// <summary>
        /// Gets a value indicating whether whether the property should be included in the backup for IEditableObject.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property should be included in the backup for IEditableObject; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IncludeInBackup { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the property is declared by the <see cref="ModelBase"/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property is declared by the <see cref="ModelBase"/>; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IsModelBaseProperty { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is a calculated property.
        /// </summary>
        /// <value><c>true</c> if this is a calculated property; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool IsCalculatedProperty { get; internal set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the default value of the property.
        /// </summary>
        /// <returns>Default value of the property.</returns>
        public object GetDefaultValue()
        {
            return DefaultValue;
        }

        /// <summary>
        /// Returns the typed default value of the property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>Default value of the property.</returns>
        public TValue GetDefaultValue<TValue>()
        {
            return (DefaultValue is TValue) ? (TValue)DefaultValue : default(TValue);
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <param name="containingType">Type of the containing.</param>
        /// <returns>CachedPropertyInfo.</returns>
        public CachedPropertyInfo GetPropertyInfo(Type containingType)
        {
            Argument.IsNotNull("containingType", containingType);

            return _cachedPropertyInfo.GetFromCacheOrFetch(containingType, () =>
            {
                var propertyInfo = containingType.GetPropertyEx(Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (propertyInfo == null)
                {
                    return null;
                }

                return new CachedPropertyInfo(propertyInfo);
            });
        }
        #endregion
    }
}