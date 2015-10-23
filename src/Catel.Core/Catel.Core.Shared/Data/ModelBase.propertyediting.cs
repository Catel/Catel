// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.editing.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using Catel.Logging;
    using Catel.Reflection;

    public partial class ModelBase
    {
        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal void SetValue(string name, object value)
        {
            SetValue(name, value, true, true);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <param name="validateAttributes">If set to <c>true</c>, the validation attributes on the property will be validated.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        internal void SetValue(string name, object value, bool notifyOnChange, bool validateAttributes)
        {
            var property = GetPropertyData(name);
            if ((value == null) && !property.Type.IsNullableType())
            {
                throw Log.ErrorAndCreateException(msg => new PropertyNotNullableException(name, GetType()),
                    "Property '{0}' on type '{1}' is not nullable, cannot set value to null", name, GetType().FullName);
            }

            SetValue(property, value, notifyOnChange, validateAttributes);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal void SetValue(PropertyData property, object value)
        {
            Argument.IsNotNull("property", property);

            SetValue(property, value, true, true);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <param name="validateAttributes">If set to <c>true</c>, the validation attributes on the property will be validated.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        internal void SetValue(PropertyData property, object value, bool notifyOnChange, bool validateAttributes)
        {
            Argument.IsNotNull("property", property);

            // Is the object currently read-only (and aren't we changing that)?
            if (IsReadOnly)
            {
                if (property != IsReadOnlyProperty)
                {
                    Log.Warning("Cannot set property '{0}', object is currently read-only", property.Name);
                    return;
                }
            }

            if (property.IsCalculatedProperty)
            {
                Log.Warning("Cannot set property '{0}', the property is a calculated property", property.Name);
                return;
            }

            if (!LeanAndMeanModel)
            {
                if ((value != null) && !property.Type.IsInstanceOfTypeEx(value))
                {
                    if (!value.GetType().IsCOMObjectEx())
                    {
                        throw Log.ErrorAndCreateException(msg => new InvalidPropertyValueException(property.Name, property.Type, value.GetType()), 
                            "Cannot set value '{0}' to property '{1}' of type '{2}', the value is invalid", value, property.Name, GetType().FullName);
                    }
                }
            }

            var notify = false;
            object oldValue = null;

            lock (_propertyValuesLock)
            {
                oldValue = GetValueFast<object>(property.Name);
                var areOldAndNewValuesEqual = ObjectHelper.AreEqualReferences(oldValue, value);

                if (notifyOnChange && (AlwaysInvokeNotifyChanged || !areOldAndNewValuesEqual) && !LeanAndMeanModel)
                {
                    var propertyChangingEventArgs = new AdvancedPropertyChangingEventArgs(property.Name);
                    RaisePropertyChanging(this, propertyChangingEventArgs);

                    if (propertyChangingEventArgs.Cancel)
                    {
                        Log.Debug("Change of property '{0}.{1}' is canceled in PropertyChanging event", GetType().FullName, property.Name);
                        return;
                    }
                }

                // Validate before assigning, dynamic properties will cause exception
                if (validateAttributes && !LeanAndMeanModel)
                {
                    ValidatePropertyUsingAnnotations(property.Name, value, property);
                }

                if (!areOldAndNewValuesEqual)
                {
                    SetValueFast(property.Name, value);
                }

                notify = (notifyOnChange && (AlwaysInvokeNotifyChanged || !areOldAndNewValuesEqual) && !LeanAndMeanModel);
            }

            // Notify outside lock
            if (notify)
            {
                RaisePropertyChanged(property.Name, oldValue, value);
            }
        }

        /// <summary>
        /// Sets the value fast without checking for any constraints or additional logic such as change notifications. This 
        /// means that if this method is used incorrectly, it can throw random exceptions.
        /// <para />
        /// This is a wrapper around the _propertyValues field. Don't use the field directly, always use
        /// this method because it takes care of locking and event subscriptions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        private void SetValueFast(string propertyName, object value)
        {
            lock (_propertyValuesLock)
            {
                _propertyBag.SetPropertyValue(propertyName, value);

                HandleObjectEventsSubscription(propertyName, value);

                IsValidated = false;
            }
        }

        /// <summary>
        /// Gets the value fast without checking for any constraints. This means that if this method is used incorrectly,
        /// it can throw random exceptions.
        /// <para />
        /// This is a wrapper around the _propertyValues field. Don't use the field directly, always use
        /// this method because it takes care of locking and event subscriptions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        private T GetValueFast<T>(string propertyName)
        {
            lock (_propertyValuesLock)
            {
                return _propertyBag.GetPropertyValue<T>(propertyName);
            }
        }

        /// <summary>
        /// Gets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal object GetValue(string name)
        {
            Argument.IsNotNullOrEmpty("name", name);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), name);

            return GetValue(propertyData);
        }

        /// <summary>
        /// Gets the typed value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected TValue GetValue<TValue>(string name)
        {
            Argument.IsNotNullOrEmpty("name", name);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), name);

            return GetValue<TValue>(propertyData);
        }

        /// <summary>
        /// Gets the value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected object GetValue(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            if (property.IsCalculatedProperty)
            {
                return PropertyHelper.GetPropertyValue(this, property.Name);
            }

            return GetValueFast<object>(property.Name);
        }

        /// <summary>
        /// Gets the typed value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected TValue GetValue<TValue>(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            object obj = GetValue(property);

            return ((obj != null) && (obj is TValue)) ? (TValue)obj : default(TValue);
        }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        object IModelEditor.GetValue(string propertyName)
        {
            return GetValue(propertyName);
        }

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModelEditor.GetValue<TValue>(string propertyName)
        {
            return GetValue<TValue>(propertyName);
        }

        /// <summary>
        /// Sets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        void IModelEditor.SetValue(string propertyName, object value)
        {
            SetValue(propertyName, value);
        }

        /// <summary>
        /// Gets the value in the fastest way possible without doing sanity checks.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value.</returns>
        /// <remarks>
        /// Note that this method does not do any sanity checks. Use at your own risk!
        /// </remarks>
        object IModelEditor.GetValueFastButUnsecure(string propertyName)
        {
            return GetValueFast<object>(propertyName);
        }

        /// <summary>
        /// Sets the value in the fastest way possible without doing sanity checks.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The value.</returns>
        /// <remarks>
        /// Note that this method does not do any sanity checks. Use at your own risk!
        /// </remarks>
        void IModelEditor.SetValueFastButUnsecure(string propertyName, object value)
        {
            SetValueFast(propertyName, value);
        }

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        object IModel.GetDefaultValue(string name)
        {
            return GetPropertyData(name).GetDefaultValue();
        }

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        object IModel.GetDefaultValue(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            return ((IModel)this).GetDefaultValue(property.Name);
        }

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the 1.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModel.GetDefaultValue<TValue>(string name)
        {
            object obj = ((IModel)this).GetDefaultValue(name);

            return (obj is TValue) ? (TValue)obj : default(TValue);
        }

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the 1.</typeparam>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModel.GetDefaultValue<TValue>(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            return ((IModel)this).GetDefaultValue<TValue>(property.Name);
        }

    }
}