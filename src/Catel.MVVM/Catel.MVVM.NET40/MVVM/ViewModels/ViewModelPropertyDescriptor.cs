// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelPropertyDescriptor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    using Catel.Caching;

    using Logging;
    using Reflection;

    /// <summary>
    /// Class deriving from the <see cref="PropertyDescriptor"/> to show how the properties dynamically
    /// created by the <see cref="ExposeAttribute"/> should be treated.
    /// </summary>
    public class ViewModelPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ICacheStorage<string, PropertyInfo> _propertyInfoCache = new CacheStorage<string, PropertyInfo>(storeNullValues: true);  

        private readonly ViewModelBase _viewModel;
        private readonly Type _viewModelType;
        private readonly string _propertyName;
        private readonly Type _propertyType;
        private PropertyInfo _propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelPropertyDescriptor" /> class.
        /// </summary>
        /// <param name="viewModel">The view model. Can be <c>null</c> for generic property definitions.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="attributes">The attributes.</param>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType" /> is <c>null</c>.</exception>
        /// <remarks>Must be kept internal because it contains special generic options such as a null view model.</remarks>
        internal ViewModelPropertyDescriptor(ViewModelBase viewModel, string propertyName, Type propertyType, Attribute[] attributes)
            : base(propertyName, attributes)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("propertyType", propertyType);

            _viewModel = viewModel;
            _viewModelType = (viewModel != null) ? viewModel.GetType() : null;
            _propertyName = propertyName;
            _propertyType = propertyType;

            if (_viewModelType != null)
            {
                string cacheKey = string.Format("{0}_{1}", _viewModelType.FullName, propertyName);
                _propertyInfo = _propertyInfoCache.GetFromCacheOrFetch(cacheKey, () => _viewModelType.GetPropertyEx(propertyName));
            }
        }

        #region Properties
        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.
        /// </returns>
        public override Type ComponentType
        {
            get { return _viewModelType; }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the property is read-only; otherwise, false.
        /// </returns>
        public override bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Type"/> that represents the type of the property.
        /// </returns>
        public override Type PropertyType
        {
            get { return _propertyType; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns><c>true</c> if resetting the component changes its value; otherwise, <c>false</c>.</returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            throw new NotSupportedException("Resetting values is not supported");
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>
        /// The value of a property for a given component.
        /// </returns>
        public override object GetValue(object component)
        {
            if (_propertyInfo == null)
            {
                if (_viewModel.IsPropertyRegistered(_propertyName))
                {
                    return _viewModel.GetValue(_propertyName);
                }

                string error = string.Format("Property '{0}' is not a real property, nor an exposed property", _propertyName);
                Log.Error(error);
                throw new NotSupportedException(error);
            }

            return _propertyInfo.GetValue(_viewModel, null);
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            if (IsReadOnly)
            {
                return;
            }

            if (_propertyInfo == null)
            {
                if (_viewModel.IsPropertyRegistered(_propertyName))
                {
                    _viewModel.SetValue(_propertyName, value);
                }
                else
                {
                    string error = string.Format("Property '{0}' is not a real property, nor an exposed property", _propertyName);
                    Log.Error(error);
                    throw new NotSupportedException(error);
                }
            }
            else
            {
                _propertyInfo.SetValue(_viewModel, value, null);
            }
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns>
        /// true if the property should be persisted; otherwise, false.
        /// </returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        #endregion
    }
}