// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatableMarkupExtension.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Markup
{
    using Catel.Logging;
    using System;
    using System.ComponentModel;
    using System.Reflection;

#if !NETFX_CORE
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
#else
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
#endif

    /// <summary>
    /// Markup extension that allows an update of the binding values.
    /// </summary>
    /// <remarks>
    /// This class is found at http://www.thomaslevesque.com/2009/07/28/wpf-a-markup-extension-that-can-update-its-target/.
    /// </remarks>
    public abstract class UpdatableMarkupExtension : MarkupExtension, INotifyPropertyChanged
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private WeakReference<object> _targetObject;
        private object _targetProperty;
        private bool _isFrameworkElementLoaded;
        private IServiceProvider _serviceProvider;

        private bool _hasBeenLoadedOnce = false;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// If set the <c>true</c>, this markup extension can replace the value by a dynamic binding
        /// in case this markup extension is used inside a setter inside a style.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        protected bool AllowUpdatableStyleSetters { get; set; }

        /// <summary>
        /// Gets the target object.
        /// </summary>
        /// <value>The target object.</value>
        protected object TargetObject
        {
            get
            {
                if (_targetObject is null)
                {
                    return null;
                }

                if (!_targetObject.TryGetTarget(out var targetObject))
                {
                    // Make sure to call this so we don't leak
                    OnTargetObjectUnloadedInternal(null, null);

                    _targetObject = null;
                    return null;
                }

                return targetObject;
            }
        }

        /// <summary>
        /// Gets the target property.
        /// </summary>
        /// <value>The target property.</value>
        protected object TargetProperty
        {
            get { return _targetProperty; }
        }

        /// <summary>
        /// Gets the value of this markup extension.
        /// </summary>
        public object Value
        {
            get
            {
                return GetValue();
            }
        }
        #endregion

        #region Events
#if !NETFX_CORE
        public event PropertyChangedEventHandler PropertyChanged;
#endif
        #endregion

        #region Methods
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public sealed override object ProvideValue(IServiceProvider serviceProvider)
        {
#if NETFX_CORE
            _targetObject = null;
            _targetProperty = null;
            _serviceProvider = null;
#else
            _serviceProvider = serviceProvider;

            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target != null)
            {
                var targetObject = target.TargetObject;
                if (targetObject != null)
                {
                    // CTL-636
                    // In a template the TargetObject is a SharedDp (internal WPF class)
                    // In that case, the markup extension itself is returned to be re-evaluated later
                    var targetObjectType = target.TargetObject.GetType();
                    if (string.Equals(targetObjectType.FullName, "System.Windows.SharedDp"))
                    // Checking for setter crashes other extensions (such as FontImage in Orchestra)
                    //string.Equals(targetObjectType.FullName, "System.Windows.Setter"))
                    {
                        return this;
                    }

                    _targetObject = new WeakReference<object>(targetObject);
                    _targetProperty = target.TargetProperty;

                    if (targetObject is FrameworkElement frameworkElement)
                    {
#if NET
                        _isFrameworkElementLoaded = frameworkElement.IsLoaded;
#endif

                        frameworkElement.Loaded += OnTargetObjectLoadedInternal;
                        frameworkElement.Unloaded += OnTargetObjectUnloadedInternal;
                    }
#if !NETFX_CORE
                    else if (targetObject is FrameworkContentElement frameworkContentElement)
                    {
                        _isFrameworkElementLoaded = frameworkContentElement.IsLoaded;

                        frameworkContentElement.Loaded += OnTargetObjectLoadedInternal;
                        frameworkContentElement.Unloaded += OnTargetObjectUnloadedInternal;
                    }
                    else if (targetObject is Setter setter)
                    {
                        if (!AllowUpdatableStyleSetters)
                        {
                            //throw Log.ErrorAndCreateException<NotSupportedException>($"Note that the target object is a setter in a style, and will never be updatable without enabling 'AllowUpdatableStyleSetters'. Either enable this property or use a different base class.");
                            Log.Warning($"Note that the target object is a setter in a style, and will never be updatable without enabling 'AllowUpdatableStyleSetters'. Either enable this property or use a different base class.");
                            return;
                        }

                        // Very special case, see https://github.com/Catel/Catel/issues/1231. Since this
                        // object will be used inside a style, we will raise "OnTargetObjectLoaded" to allow
                        // the markup extensions to register to events. This mode will never call
                        // "OnTargetObjectUnloaded"

                        OnTargetObjectLoaded();

                        return new Binding
                        {
                            Source = this,
                            Path = new PropertyPath(nameof(Value))
                        };
                    }
#endif
                }
            }
#endif

            var value = GetValue();
            return value;
        }

        private void OnTargetObjectLoadedInternal(object sender, RoutedEventArgs e)
        {
            if (_isFrameworkElementLoaded)
            {
                return;
            }

            OnTargetObjectLoaded();

            // CTL-925: if an item has been loaded at least once, update the value next time it is being loaded
            if (_hasBeenLoadedOnce)
            {
                UpdateValue();
            }

            _hasBeenLoadedOnce = true;
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
            var targetObject = TargetObject;
            if (targetObject is null)
            {
                return;
            }

            if (AllowUpdatableStyleSetters && targetObject is Setter)
            {
                // Special case, this is binding to the Value property, raise property change & exit
                RaisePropertyChanged(nameof(Value));
                return;
            }

            var value = GetValue();

            var targetProperty = _targetProperty;
            if (targetProperty is DependencyProperty targetPropertyAsDependencyProperty)
            {
                var obj = targetObject as DependencyObject;
                if (obj == null)
                {
                    return;
                }

                Action updateAction =
#if NETFX_CORE
                () => obj.SetValue(targetPropertyAsDependencyProperty, value);
#else
                () => obj.SetCurrentValue(targetPropertyAsDependencyProperty, value);
#endif

#if NETFX_CORE
                if (obj.Dispatcher.HasThreadAccess)
                {
                    UpdateValue();
                }
                else
                {
#pragma warning disable 4014
                    obj.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => updateAction());
#pragma warning restore 4014
                }
#else
                if (obj.CheckAccess())
                {
                    updateAction();
                }
                else
                {
                    obj.Dispatcher.Invoke(updateAction);
                }
#endif
            }
#if !NETFX_CORE
            else if (targetObject is Setter setter)
            {
                setter.Value = value;
                //Setter.ReceiveMarkupExtension(targetObject, new XamlSetMarkupExtensionEventArgs(null, this, _serviceProvider));
            }
#endif
            else if (targetProperty is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(targetObject, value, null);
            }

            RaisePropertyChanged(nameof(Value));
        }

#if !NETFX_CORE
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endif

        /// <summary>
        /// Gets the value by combining the rights methods (so we don't have to repeat ourselves).
        /// </summary>
        /// <returns>System.Object.</returns>
        private object GetValue()
        {
            var value = ProvideDynamicValue(_serviceProvider);
            return value;
        }

        /// <summary>
        /// Provides the dynamic value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>System.Object.</returns>
        protected virtual object ProvideDynamicValue(IServiceProvider serviceProvider)
        {
            return null;
        }
        #endregion
    }
}
#endif
