﻿namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xml.Serialization;
    using Catel.Reflection;

    /// <summary>
    /// Object that contains all the property data that is used by the <see cref="ModelBase"/> class.
    /// </summary>
    public class PropertyData : PropertyData<object>
    {
        internal PropertyData(string name, object defaultValue,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler, bool isSerializable,
            bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
            : base(name, defaultValue, propertyChangedEventHandler, isSerializable, includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty)
        {
        }

        internal PropertyData(string name, Func<object> createDefaultValue,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler, bool isSerializable,
            bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
            : base(name, createDefaultValue, propertyChangedEventHandler, isSerializable, includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty)
        {
        }
    }

    /// <summary>
    /// Object that contains all the property data that is used by the <see cref="ModelBase"/> class.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    public class PropertyData<T> : IPropertyData
    {
        /// <summary>
        /// Callback to use to create the default value.
        /// </summary>
        private readonly Func<T> _createDefaultValue;

        private CachedPropertyInfo? _cachedPropertyInfo;

        private bool _updatedCachedPropertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyData" /> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase" />.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        internal PropertyData(string name, T defaultValue, EventHandler<PropertyChangedEventArgs>? propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
            : this(name, () => defaultValue, propertyChangedEventHandler, isSerializable,
                   includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyData"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for reference types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="createDefaultValue"/> is <c>null</c>.</exception>
        internal PropertyData(string name, Func<T> createDefaultValue, EventHandler<PropertyChangedEventArgs>? propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool isCalculatedProperty)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            Name = name;
            Type = typeof(T);
            PropertyChangedEventHandler = propertyChangedEventHandler;
            IsSerializable = isSerializable;
            IncludeInSerialization = includeInSerialization;
            IsModelBaseProperty = isModelBaseProperty;
            IncludeInBackup = includeInBackup;
            IsCalculatedProperty = isCalculatedProperty;

            _createDefaultValue = createDefaultValue;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        [XmlIgnore]
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the default value of the property.
        /// </summary>
        [XmlIgnore]
        private T DefaultValue
        {
            get { return _createDefaultValue(); }
        }

        /// <summary>
        /// Gets a value indicating the property changed event handler.
        /// </summary>
        /// <value>The property changed event handler.</value>
        [XmlIgnore]
        public EventHandler<PropertyChangedEventArgs>? PropertyChangedEventHandler { get; private set; }

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
        public bool IsCalculatedProperty { get; set; }

        /// <summary>
        /// Gets a value indicating whether this property has validation attributes.
        /// <para/>
        /// If this value is <c>null</c>, the state is unknown and needs to be determined.
        /// </summary>
        /// <value><c>true</c> if this property has validation attributes; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        public bool? IsDecoratedWithValidationAttributes { get; set; }

        /// <summary>
        /// Returns the default value of the property.
        /// </summary>
        /// <returns>Default value of the property.</returns>
        public object? GetDefaultValue()
        {
            return DefaultValue;
        }

        /// <summary>
        /// Returns the default value of the property.
        /// </summary>
        /// <returns>Default value of the property.</returns>
        public TValue GetDefaultValue<TValue>()
        {
            if (DefaultValue is TValue typedValue)
            {
                return typedValue;
            }

            return default!;
        }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <param name="containingType">Type of the containing.</param>
        /// <returns>CachedPropertyInfo.</returns>
        public CachedPropertyInfo? GetPropertyInfo(Type containingType)
        {
            if (!_updatedCachedPropertyInfo)
            {
                _updatedCachedPropertyInfo = true;

                var propertyInfo = containingType.GetPropertyEx(Name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                _cachedPropertyInfo = (propertyInfo is null) ? null : new CachedPropertyInfo(propertyInfo);
            }

            return _cachedPropertyInfo;
        }

        public override string ToString()
        {
            return $"[{Type.Name}] {Name}";
        }
    }
}
