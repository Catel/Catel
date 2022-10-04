namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Linq.Expressions;
    using Logging;
    using Reflection;

    public partial class ModelBase
    {
        private static readonly Dictionary<string, object> CalculatedPropertyExpressions = new Dictionary<string, object>();

        /// <summary>
        /// Gets the object value for the specified value. This method allows caching of boxed objects.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An object representing the value.</returns>
        protected static object GetObjectValue<TValue>(TValue value)
        {
            object objectValue = null;

            if (typeof(TValue).IsValueTypeEx())
            {
                objectValue = BoxingCache<TValue>.Default.GetBoxedValue(value);
            }
            else
            {
                objectValue = value;
            }

            return objectValue;
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal void SetValue<TValue>(string name, TValue value, bool notifyOnChange = true)
        {
            var property = GetPropertyData(name);

            if ((value is null) && !property.Type.IsNullableType())
            {
                throw Log.ErrorAndCreateException(msg => new PropertyNotNullableException(name, GetType()),
                    "Property '{0}' on type '{1}' is not nullable, cannot set value to null", name, GetType().FullName);
            }

            SetValue(property, value, notifyOnChange);
        }

        /// <summary>
        /// Creates the property bag implementation that will be used by this model.
        /// </summary>
        /// <returns>The <see cref="IPropertyBag"/> to be used by this object.</returns>
        protected virtual IPropertyBag CreatePropertyBag()
        {
            return new TypedPropertyBag();
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        protected internal void SetValue<TValue>(IPropertyData property, TValue value, bool notifyOnChange = true)
        {
            // Is the object currently read-only (and aren't we changing that)?
            if (IsReadOnly || _isFrozen)
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

            if ((value is not null) && !property.Type.IsInstanceOfTypeEx(value))
            {
                if (!value.GetType().IsCOMObjectEx())
                {
                    throw Log.ErrorAndCreateException(msg => new InvalidPropertyValueException(property.Name, property.Type, value.GetType()),
                        "Cannot set value '{0}' to property '{1}' of type '{2}', the value is invalid", BoxingCache.GetBoxedValue(value), property.Name, GetType().FullName);
                }
            }

            var notify = false;
            TValue oldValue;

            lock (_lock)
            {
                var changeNotificationsSuspensionContext = _changeNotificationsSuspensionContext;

                oldValue = GetValueFromPropertyBag<TValue>(property.Name);
                var areOldAndNewValuesEqual = ObjectHelper.AreEqualReferences(BoxingCache.GetBoxedValue(oldValue), BoxingCache.GetBoxedValue(value));

                if (!areOldAndNewValuesEqual)
                {
                    SetValueToPropertyBag(property.Name, value);
                }

                notify = (notifyOnChange && (AlwaysInvokeNotifyChanged || !areOldAndNewValuesEqual));

                if (changeNotificationsSuspensionContext is not null)
                {
                    changeNotificationsSuspensionContext.Add(property.Name);
                    notify = false;
                }
            }

            // Notify outside lock
            if (notify)
            {
                RaisePropertyChanged(property.Name);
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
        protected virtual void SetValueToPropertyBag<TValue>(string propertyName, TValue value)
        {
            lock (_lock)
            {
                _propertyBag.SetValue(propertyName, value);
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
        protected virtual T GetValueFromPropertyBag<T>(string propertyName)
        {
            lock (_lock)
            {
                return _propertyBag.GetValue<T>(propertyName);
            }
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
        protected TValue GetValue<TValue>(IPropertyData property)
        {
            if (property.IsCalculatedProperty)
            {
                // Note: don't use IObjectAdapter since it might cause a stackoverflow going into
                // this method again
                var expression = GetPropertyGetterExpression<TValue>(property.Name);
                if (expression is null)
                {
                    // Fall back to reflection
                    return (TValue)PropertyHelper.GetPropertyValue(this, property.Name);
                }
                else
                {
                    return expression(this);
                }
            }

            return GetValueFromPropertyBag<TValue>(property.Name);
        }

        /// <summary>
        /// Gets a property getter expression to create super fast access to calculated properties of this object.
        /// </summary>
        /// <typeparam name="TValue">The value of the property.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The compiled expression for the specified property name.</returns>
        protected Func<object, TValue> GetPropertyGetterExpression<TValue>(string propertyName)
        {
            var key = $"{propertyName}_as_{typeof(TValue).Name}";

            if (!CalculatedPropertyExpressions.TryGetValue(key, out var getter))
            {
                var expression = ExpressionBuilder.CreatePropertyGetter<TValue>(GetType(), propertyName);
                getter = expression?.Compile();

                CalculatedPropertyExpressions[key] = getter;
            }

            return (Func<object, TValue>)getter;
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
        void IModelEditor.SetValue<TValue>(string propertyName, TValue value)
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
        TValue IModelEditor.GetValueFastButUnsecure<TValue>(string propertyName)
        {
            return GetValueFromPropertyBag<TValue>(propertyName);
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
        void IModelEditor.SetValueFastButUnsecure<TValue>(string propertyName, TValue value)
        {
            SetValueToPropertyBag(propertyName, value);
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
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the 1.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModel.GetDefaultValue<TValue>(string name)
        {
            var obj = ((IModel)this).GetDefaultValue(name);

            return (obj is TValue) ? (TValue)obj : default;
        }
    }
}
