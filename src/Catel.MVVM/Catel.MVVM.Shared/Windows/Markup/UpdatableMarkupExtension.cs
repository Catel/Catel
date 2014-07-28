// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatableMarkupExtension.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SILVERLIGHT

namespace Catel.Windows.Markup
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using Catel.Windows.Threading;

#if !NETFX_CORE
    using System.Windows.Data;
    using System.Windows.Markup;
#endif

    /// <summary>
    /// Markup extension that allows an update of the binding values.
    /// </summary>
    /// <remarks>
    /// This class is found at http://www.thomaslevesque.com/2009/07/28/wpf-a-markup-extension-that-can-update-its-target/.
    /// </remarks>
    public abstract class UpdatableMarkupExtension : MarkupExtension
    {
        #region Fields
        private object _targetObject;
        private object _targetProperty;
        private bool _isFrameworkElementLoaded;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target object.
        /// </summary>
        /// <value>The target object.</value>
        protected object TargetObject
        {
            get { return _targetObject; }
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <value>The target property.</value>
        protected object TargetProperty
        {
            get { return _targetProperty; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public override sealed object ProvideValue(IServiceProvider serviceProvider)
        {
#if WINDOWS_PHONE
            _targetObject = null;
            _targetProperty = null;
#else
            var target = serviceProvider.GetService(typeof (IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                _targetObject = target.TargetObject;
                _targetProperty = target.TargetProperty;

                FrameworkElement frameworkElement;
#if !SILVERLIGHT
                FrameworkContentElement frameworkContentElement;
#endif

                if ((frameworkElement = _targetObject as FrameworkElement) != null) 
                {
#if NET
                    _isFrameworkElementLoaded = frameworkElement.IsLoaded;
#endif

                    frameworkElement.Loaded += OnTargetObjectLoadedInternal;
                    frameworkElement.Unloaded += OnTargetObjectUnloadedInternal;
                }
#if !SILVERLIGHT
                else if ((frameworkContentElement = _targetObject as FrameworkContentElement) != null)
                {
                    _isFrameworkElementLoaded = frameworkContentElement.IsLoaded;

                    frameworkContentElement.Loaded += OnTargetObjectLoadedInternal;
                    frameworkContentElement.Unloaded += OnTargetObjectUnloadedInternal;
                }
#endif
            }
#endif

            return ProvideDynamicValue();
        }

        private void OnTargetObjectLoadedInternal(object sender, RoutedEventArgs e)
        {
            if (_isFrameworkElementLoaded)
            {
                return;
            }

            OnTargetObjectLoaded();

            _isFrameworkElementLoaded = true;
        }

        /// <summary>
        /// Called when the target object is loaded.
        /// <para />
        /// Note that this method will only be called if the target object is a <see cref="FrameworkElement"/>.
        /// </summary>
        protected virtual void OnTargetObjectLoaded()
        {   
        }

        private void OnTargetObjectUnloadedInternal(object sender, RoutedEventArgs e)
        {
            if (!_isFrameworkElementLoaded)
            {
                return;
            }

            OnTargetObjectUnloaded();

            _isFrameworkElementLoaded = false;
        }

        /// <summary>
        /// Called when the target object is unloaded.
        /// <para />
        /// Note that this method will only be called if the target object is a <see cref="FrameworkElement"/>.
        /// </summary>
        protected virtual void OnTargetObjectUnloaded()
        {
        }

        /// <summary>
        /// Updates the value.
        /// </summary>
        protected void UpdateValue()
        {
            var value = ProvideDynamicValue();

            if (_targetObject != null)
            {
                var targetPropertyAsDependencyProperty = _targetProperty as DependencyProperty;
                if (targetPropertyAsDependencyProperty != null)
                {
                    var obj = _targetObject as DependencyObject;
                    if (obj == null)
                    {
                        return;
                    }

                    Action updateAction = () => obj.SetValue(targetPropertyAsDependencyProperty, value);

                    if (obj.CheckAccess())
                    {
                        updateAction();
                    }
                    else
                    {
                        obj.Dispatcher.Invoke(updateAction);
                    }
                }
                else
                {
                    var prop = _targetProperty as PropertyInfo;
                    if (prop != null)
                    {
                        prop.SetValue(_targetObject, value, null);
                    }
                }
            }
        }

        /// <summary>
        /// Provides the dynamic value.
        /// </summary>
        /// <returns>System.Object.</returns>
        protected abstract object ProvideDynamicValue();
        #endregion
    }
}

#endif