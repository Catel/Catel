// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.propertyregistration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using Logging;
    using Reflection;

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
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static IPropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, TValue defaultValue,
            Action<TModel, PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("The member type of the body of the property expression should be a property");
            }

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty<TValue>(propertyName, defaultValue, (sender, args) =>
            {
                if (propertyChangedEventHandler is not null)
                {
                    propertyChangedEventHandler.Invoke((TModel)sender, args);
                }
            }, includeInSerialization, includeInBackup);
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
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static IPropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, Func<TValue> createDefaultValue = null,
            Action<TModel, PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw Log.ErrorAndCreateException<ArgumentException>("The member type of the body of the property expression should be a property");
            }

            if (createDefaultValue is null)
            {
                createDefaultValue = () => default;
            }

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty<TValue>(propertyName, createDefaultValue, (sender, args) =>
            {
                propertyChangedEventHandler?.Invoke((TModel)sender, args);
            }, includeInSerialization, includeInBackup);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static IPropertyData RegisterProperty<TValue>(string name, TValue defaultValue,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true)
        {
            return RegisterProperty<TValue>(name, defaultValue, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public static IPropertyData RegisterProperty<TValue>(string name, Func<TValue> createDefaultValue = null,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true)
        {
            return RegisterProperty<TValue>(name, createDefaultValue, propertyChangedEventHandler, 
                includeInSerialization, includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        private static IPropertyData RegisterProperty<TValue>(string name, TValue defaultValue,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            return RegisterProperty<TValue>(name, () => defaultValue, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, isModelBaseProperty);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        private static IPropertyData RegisterProperty<TValue>(string name, Func<object> createDefaultValue = null,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            // Note: this overload is required for non-generic-to-generic conversion

            Func<TValue> typedDefaultValueCallback = null;

            if (createDefaultValue is null)
            {
                typedDefaultValueCallback = () => default;
            }
            else
            {
                typedDefaultValueCallback = () =>
                {
                    var defaultValue = createDefaultValue();
                    if (defaultValue is TValue value)
                    {
                        return value;
                    }

                    return default(TValue);
                };
            }

            var isSerializable = true;

            isSerializable = typeof(TValue).IsInterfaceEx() || typeof(TValue).IsSerializableEx();

            var property = new PropertyData<TValue>(name, typedDefaultValueCallback, propertyChangedEventHandler, isSerializable,
                includeInSerialization, includeInBackup, isModelBaseProperty, false);
            return property;
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        private static IPropertyData RegisterProperty<TValue>(string name, Func<TValue> createDefaultValue = null,
            EventHandler<PropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            if (createDefaultValue is null)
            {
                createDefaultValue = () => default;
            }

            var isSerializable = true;

            isSerializable = typeof(TValue).IsInterfaceEx() || typeof(TValue).IsSerializableEx();

            var property = new PropertyData<TValue>(name, createDefaultValue, propertyChangedEventHandler, isSerializable,
                includeInSerialization, includeInBackup, isModelBaseProperty, false);
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
        }

        /// <summary>
        /// Initializes a specific property for this object after the object is already constructed and initialized.
        /// <para />
        /// Normally, properties are automatically registered in the constructor. If properties should be registered
        /// via runtime behavior, this method must be used.
        /// </summary>
        /// <param name="property"><see cref="IPropertyData"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        protected internal void InitializePropertyAfterConstruction(IPropertyData property)
        {
            Argument.IsNotNull("property", property);

            bool isCalculatedProperty = false;

            var type = GetType();

            var reflectedProperty = type.GetPropertyEx(property.Name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (reflectedProperty is null)
            {
                Log.Warning("Property '{0}' is registered after construction of type '{1}', but could not be found using reflection", property.Name, type.FullName);
            }
            else
            {
                isCalculatedProperty = !reflectedProperty.CanWrite;
            }

            InitializeProperty(property, isCalculatedProperty);
        }

        /// <summary>
        /// Initializes a specific property for this object.
        /// </summary>
        /// <param name="propertyData">The property.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        private void InitializeProperty(IPropertyData propertyData, bool isCalculatedProperty = false)
        {
            var objectType = GetType();
            var propertyName = propertyData.Name;

            if (propertyData.Type == typeof(Type))
            {
                // If default value is a type, something smells (could be the result of a bad migration)
                var propertyInfo = PropertyHelper.GetPropertyInfo(this, propertyName);
                if (propertyInfo is not null)
                {
                    if (propertyInfo.PropertyType != typeof(Type))
                    {
                        throw Log.ErrorAndCreateException<CatelException>($"Default property value for property '{objectType.Name}.{propertyName}' is of type 'Type', but actual type is '{propertyInfo.PropertyType.Name}'. This appears to be an upgrade issue to Catel 6.x");
                    }
                }
            }

            if (!IsPropertyRegistered(propertyName))
            {
                propertyData.IsCalculatedProperty = isCalculatedProperty;

                PropertyDataManager.RegisterProperty(objectType, propertyName, propertyData);
            }

            lock (_lock)
            {
                if (!_propertyBag.IsAvailable(propertyName))
                {
                    SetDefaultValueToPropertyBag(propertyData);
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
        protected IPropertyData GetPropertyData(string name)
        {
            return PropertyDataManager.GetPropertyData(GetType(), name);
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
    }
}
