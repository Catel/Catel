// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataResource.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Catel.Windows.Data
{
    /// <summary>
    /// Class that represents a bound property value for a non-bindable property.
    /// </summary>
    /// <remarks>
    /// This code originally comes from: http://www.wpfmentor.com/2009/01/how-to-add-binding-to-commandparameter.html.
    /// </remarks>
    public class DataResource : Freezable
    {
        #region Properties
        /// <summary>
        /// Gets or sets the binding target.
        /// </summary>
        /// <value>The binding target.</value>
        public object BindingTarget
        {
            get { return (object)GetValue(BindingTargetProperty); }
            set { SetValue(BindingTargetProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BindingTarget"/> dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the <see cref="BindingTarget"/> dependency property.
        /// </value>
        public static readonly DependencyProperty BindingTargetProperty = DependencyProperty.Register("BindingTarget", typeof(object), typeof(DataResource), new UIPropertyMetadata(null));
        #endregion

        #region Methods
        /// <summary>
        /// Creates an instance of the specified type using that type's default constructor. 
        /// </summary>
        /// <returns>
        /// A reference to the newly created object.
        /// </returns>
        protected override Freezable CreateInstanceCore()
        {
            return (Freezable)Activator.CreateInstance(GetType());
        }

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="Freezable"/>
        /// using base (non-animated) property values. 
        /// </summary>
        /// <param name="sourceFreezable">
        /// The object to clone.
        /// </param>
        protected sealed override void CloneCore(Freezable sourceFreezable)
        {
            base.CloneCore(sourceFreezable);
        }
        #endregion
    }

    /// <summary>
    /// Data resource binding extension class.
    /// </summary>
    public class DataResourceBindingExtension : MarkupExtension
    {
        #region Fields
        private object _targetObject;
        private object _targetProperty;
        private DataResource _dataResource;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the data resource.
        /// </summary>
        /// <value>The data resource.</value>
        public DataResource DataResource
        {
            get
            {
                return _dataResource;
            }

            set
            {
                if (_dataResource != value)
                {
                    if (_dataResource != null)
                    {
                        _dataResource.Changed -= OnDataResourceChanged;
                    }

                    _dataResource = value;

                    if (_dataResource != null)
                    {
                        _dataResource.Changed += OnDataResourceChanged;
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            _targetObject = target.TargetObject;
            _targetProperty = target.TargetProperty;

            // _targetProperty can be null when this is called in the Designer.
            Debug.Assert(_targetProperty != null || Environment.IsInDesignMode);

            if (DataResource.BindingTarget == null && _targetProperty != null)
            {
                var propInfo = _targetProperty as PropertyInfo;
                if (propInfo != null)
                {
                    try
                    {
                        return Activator.CreateInstance(propInfo.PropertyType);
                    }
                    catch (MissingMethodException)
                    {
                        // there isn't a default constructor
                    }
                }

                var depProp = _targetProperty as DependencyProperty;
                if (depProp != null)
                {
                    var depObj = (DependencyObject)_targetObject;
                    return depObj.GetValue(depProp);
                }
            }

            return DataResource.BindingTarget;
        }

        /// <summary>
        /// Called when the data resource has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDataResourceChanged(object sender, EventArgs e)
        {
            // Ensure that the bound object is updated when DataResource changes.
            var dataResource = (DataResource)sender;
            var depProp = _targetProperty as DependencyProperty;

            if (depProp != null)
            {
                var depObj = (DependencyObject)_targetObject;
                object value = Convert(dataResource.BindingTarget, depProp.PropertyType);
                depObj.SetValue(depProp, value);
            }
            else
            {
                var propInfo = _targetProperty as PropertyInfo;
                if (propInfo != null)
                {
                    object value = Convert(dataResource.BindingTarget, propInfo.PropertyType);
                    propInfo.SetValue(_targetObject, value, new object[0]);
                }
            }
        }

        /// <summary>
        /// Converts the specified object.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="toType">The type to convert the object to.</param>
        /// <returns>The converted object.</returns>
        private static object Convert(object obj, Type toType)
        {
            try
            {
                return System.Convert.ChangeType(obj, toType);
            }
            catch (InvalidCastException)
            {
                return obj;
            }
        }
        #endregion
    }
}
