// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.propertyregistration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Catel.Logging;
    using Catel.Reflection;

    public partial class ModelBase
    {
        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">If set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">If set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">If set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, TValue defaultValue,
            Action<TModel, AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

#if !PCL && !NETFX_CORE
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("The member type of the body of the property expression should be a property");
            }
#endif

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty(propertyName, typeof(TValue), defaultValue, (sender, args) =>
            {
                if (propertyChangedEventHandler != null)
                {
                    propertyChangedEventHandler.Invoke((TModel)sender, args);
                }
            }, includeInSerialization, includeInBackup, setParent);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">If set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">If set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, Func<TValue> createDefaultValue = null,
            Action<TModel, AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

#if !PCL && !NETFX_CORE
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("The member type of the body of the property expression should be a property");
            }
#endif

            object realDefaultValue = createDefaultValue;
            if (createDefaultValue == null)
            {
                realDefaultValue = default(TValue);
            }

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty(propertyName, typeof(TValue), realDefaultValue, (sender, args) =>
            {
                if (propertyChangedEventHandler != null)
                {
                    propertyChangedEventHandler.Invoke((TModel)sender, args);
                }
            }, includeInSerialization, includeInBackup, setParent);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TValue>(string name, Type type, TValue defaultValue,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            var value = defaultValue as Delegate;
            if (value != null)
            {
                return RegisterProperty(name, type, () => value.DynamicInvoke(), setParent, propertyChangedEventHandler,
                    includeInSerialization, includeInBackup, false);
            }

            return RegisterProperty(name, type, () => defaultValue, setParent, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty(string name, Type type, Func<object> createDefaultValue = null,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            return RegisterProperty(name, type, createDefaultValue, setParent, propertyChangedEventHandler, includeInSerialization,
                includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static PropertyData RegisterProperty<TValue>(string name, Type type, TValue defaultValue, bool setParent = true,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            var value = defaultValue as Delegate;
            if (value != null)
            {
                return RegisterProperty(name, type, () => value.DynamicInvoke(), setParent, propertyChangedEventHandler,
                    includeInSerialization, includeInBackup, isModelBaseProperty);
            }

            return RegisterProperty(name, type, () => defaultValue, setParent, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, isModelBaseProperty);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static PropertyData RegisterProperty(string name, Type type, Func<object> createDefaultValue = null, bool setParent = true,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("type", type);

            if (createDefaultValue == null)
            {
                createDefaultValue = () => type.IsValueTypeEx() ? Activator.CreateInstance(type) : null;
            }

            var isSerializable = true;

#if NET
            isSerializable = type.IsInterfaceEx() || type.IsSerializableEx();
#endif

            var property = new PropertyData(name, type, createDefaultValue, setParent, propertyChangedEventHandler, isSerializable,
                includeInSerialization, includeInBackup, isModelBaseProperty, false, false);
            return property;
        }

        /// <summary>
        /// Unregisters the property.
        /// <para />
        /// Note that the unregistration of a property applies to all models of the same type. It is not possible to 
        /// unregister a property for a single instance of a type.
        /// </summary>
        /// <param name="modelType">Type of the model, required because it cannot be retrieved in a static context.</param>
        /// <param name="name">The name.</param>
        protected internal static void UnregisterProperty(Type modelType, string name)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNullOrWhitespace("name", name);

            PropertyDataManager.UnregisterProperty(modelType, name);
        }

        /// <summary>
        /// Initializes all the properties for this object.
        /// </summary>
        private void InitializeProperties()
        {
            var type = GetType();

            var catelTypeInfo = PropertyDataManager.RegisterProperties(type);
            foreach (var propertyDataKeyValuePair in catelTypeInfo.GetCatelProperties())
            {
                var propertyData = propertyDataKeyValuePair.Value;

                InitializeProperty(propertyData);
            }

            lock (_initializedTypes)
            {
                // No need to check if already existing
                _initializedTypes.Add(type);
            }
        }

        /// <summary>
        /// Initializes a specific property for this object after the object is already constructed and initialized.
        /// <para />
        /// Normally, properties are automatically registered in the constructor. If properties should be registered
        /// via runtime behavior, this method must be used.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        protected internal void InitializePropertyAfterConstruction(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            bool isCalculatedProperty = false;

            var type = GetType();

            var reflectedProperty = type.GetPropertyEx(property.Name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (reflectedProperty == null)
            {
                Log.Warning("Property '{0}' is registered after construction of type '{1}', but could not be found using reflection", property.Name, type.FullName);
            }
            else
            {
                isCalculatedProperty = !reflectedProperty.CanWrite;
            }

            InitializeProperty(property, true, isCalculatedProperty);
        }

        /// <summary>
        /// Initializes a specific property for this object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="lateRegistration">If set to <c>true</c>, the property is assumed to be registered after the official initialization.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        private void InitializeProperty(PropertyData property, bool lateRegistration = false, bool isCalculatedProperty = false)
        {
            InitializeProperty(property.Name, property.Type, property.GetDefaultValue(), property.SetParent, property.PropertyChangedEventHandler,
                property.IsSerializable, property.IncludeInSerialization, property.IncludeInBackup, property.IsModelBaseProperty, lateRegistration,
                isCalculatedProperty, property.IsDynamicProperty);
        }

        /// <summary>
        /// Initializes a specific property for this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <param name="lateRegistration">if set to <c>true</c>, the property is assumed to be registered after the official initialization.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <param name="isDynamicProperty">if set to <c>true</c>, the property is a dynamic property.</param>
        /// <exception cref="InvalidPropertyException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        private void InitializeProperty(string name, Type type, object defaultValue, bool setParent, EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool lateRegistration, bool isCalculatedProperty, bool isDynamicProperty)
        {
            var objectType = GetType();
            if ((defaultValue == null) && !type.IsNullableType())
            {
                throw Log.ErrorAndCreateException(msg => new PropertyNotNullableException(name, objectType),
                    "Property '{0}' is not nullable, please provide a valid (not null) default value", name);
            }

            lock (_initializedTypes)
            {
                if (!_initializedTypes.Contains(objectType) || lateRegistration)
                {
                    if (!IsPropertyRegistered(name))
                    {
                        var propertyData = new PropertyData(name, type, defaultValue, setParent, propertyChangedEventHandler,
                            isSerializable, includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty,
                            isDynamicProperty);
                        PropertyDataManager.RegisterProperty(objectType, name, propertyData);

#if !NETFX_CORE && !PCL
                        // Skip validation for modelbase properties
                        if (propertyData.IsModelBaseProperty)
                        {
                            _propertyValuesIgnoredOrFailedForValidation[type].Add(propertyData.Name);
                        }
#endif
                    }
                }
            }

            lock (_lock)
            {
                if (!_propertyBag.IsPropertyAvailable(name))
                {
                    SetValueFast(name, defaultValue);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property is a property declared by the <see cref="ModelBase"/> itself.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// <c>true</c> if the specified property is a property declared by the <see cref="ModelBase"/> itself; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsModelBaseProperty(string name)
        {
            if (!IsPropertyRegistered(name))
            {
                return false;
            }

            var propertyData = GetPropertyData(name);
            return propertyData.IsModelBaseProperty;
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property is registered, otherwise false.</returns>
        /// TODO: Try to revert to internal but is required by XAMARIN_FORMS
        public bool IsPropertyRegistered(string name)
        {
            return IsPropertyRegistered(GetType(), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <typeparam name="T">Type of the object for which to check.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegistered<T>(string name)
        {
            return IsPropertyRegistered(typeof(T), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="type">The type of the object for which to check.</param>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegistered(Type type, string name)
        {
            return PropertyDataManager.IsPropertyRegistered(type, name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyData"/> for the specified property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The <see cref="PropertyData"/>.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected PropertyData GetPropertyData(string name)
        {
            return PropertyDataManager.GetPropertyData(GetType(), name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><see cref="PropertyInfo"/> or <c>null</c> if no property info is found.</returns>
        protected PropertyInfo GetPropertyInfo(PropertyData property)
        {
            return GetPropertyInfo(property.Name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the specified property.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        /// <returns><see cref="PropertyInfo"/> or <c>null</c> if no property info is found.</returns>
        protected PropertyInfo GetPropertyInfo(string property)
        {
            return GetType().GetPropertyEx(property, BindingFlagsHelper.GetFinalBindingFlags(true, false));
        }

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        Type IModel.GetPropertyType(string name)
        {
            return GetPropertyData(name).Type;
        }

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        Type IModel.GetPropertyType(PropertyData property)
        {
            return ((IModel)this).GetPropertyType(property.Name);
        }
    }
}