// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayBindingUpdate.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !WIN80 && !XAMARIN

namespace Catel.Windows.Interactivity
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
    using TimerTickEventArgs = System.Object;
#else
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using System.Windows.Threading;
    using UIEventArgs = System.EventArgs;
    using TimerTickEventArgs = System.EventArgs;
#endif

    using System;
    using Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// This behaviors sets the binding to <see cref="UpdateSourceTrigger.Explicit"/> and manually updates the
    /// binding from view to view model after the delay.
    /// </summary>
    public class DelayBindingUpdate : BehaviorBase<FrameworkElement>
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly List<PropertyInfo> BindingProperties;

        private readonly DispatcherTimer _timer;

        private Binding _originalBinding;

        private DependencyProperty _dependencyPropertyCache;
        #endregion

        #region Constructors
        static DelayBindingUpdate()
        {
            BindingProperties = new List<PropertyInfo>(from property in typeof(Binding).GetPropertiesEx()
                                                       where property.CanRead && property.CanWrite
                                                       select property);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelayBindingUpdate" /> class.
        /// </summary>
        public DelayBindingUpdate()
        {
            UpdateDelay = 100;

            _timer = new DispatcherTimer();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the update delay. 
        /// <para />
        /// This is the value that is used between updates in milliseconds. The binding will be updated
        /// after the delay. When a new value becomes available, the timer is reset and starts all over.
        /// <para />
        /// The default value is <c>100</c>. If the value is smaller than <c>50</c>, the value
        /// will be ignored and there will be no delay between the value change and binding update. If the
        /// value is higher than <c>5000</c>, it will be set to <c>5000</c>.
        /// </summary>
        /// <value>The update delay.</value>
        public int UpdateDelay { get; set; }

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <remarks>
        /// This property does not reflect to any changes, so this property must be set when the 
        /// <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </remarks>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the dependency property. This property is used before the <see cref="PropertyName"/>. By
        /// default, this behavior tries to retrieve the dependency property via "[PropertyName]Property" (which is the default
        /// naming convention of dependency properties). In the rare case that this naming convention is not followed, it is
        /// possible to use this property.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <remarks>
        /// This property does not reflect to any changes, so this property must be set when the 
        /// <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// <para />
        /// This property should only be used as backup if the <see cref="PropertyName"/> property does not work.
        /// </remarks>
        public string DependencyPropertyName { get; set; }

        /// <summary>
        /// Gets the name of the used dependency property.
        /// </summary>
        /// <value>The name of the used property or <c>null</c> if no property is used.</value>
        private string UsedDependencyPropertyName
        {
            get
            {
                DependencyProperty property;

                // Fallback first
                if (!string.IsNullOrEmpty(DependencyPropertyName))
                {
                    property = GetDependencyProperty(DependencyPropertyName);
                    if (property != null)
                    {
                        return DependencyPropertyName;
                    }
                }

                string propertyName = string.Format("{0}Property", PropertyName);
                property = GetDependencyProperty(propertyName);
                if (property != null)
                {
                    return propertyName;
                }

                return null;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the required properties.
        /// </summary>
        protected override void ValidateRequiredProperties()
        {
            Argument.IsNotNullOrWhitespace("PropertyName", PropertyName);

            if (GetDependencyProperty() == null)
            {
                throw new InvalidOperationException("Dependency property is not found on the associated object, make sure to set the PropertyName or DependencyPropertyName");
            }
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> has been loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            var dependencyProperty = GetDependencyProperty();
            var bindingExpression = AssociatedObject.GetBindingExpression(dependencyProperty);
            if (bindingExpression == null)
            {
                Log.Error("No binding expression found on '{0}'", UsedDependencyPropertyName);
                return;
            }

            var binding = bindingExpression.ParentBinding;
            _originalBinding = binding;

            var newBinding = CreateBindingCopy(binding);
            newBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            AssociatedObject.ClearValue(dependencyProperty);
            AssociatedObject.SetBinding(dependencyProperty, newBinding);

            Log.Debug("Changed UpdateSourceTrigger from to 'Explicit' for dependency property '{0}'", UsedDependencyPropertyName);

            AssociatedObject.SubscribeToDependencyProperty(PropertyName, OnDependencyPropertyChanged);

            Log.Debug("Subscribed to property changes of the original object");

            _timer.Tick += OnTimerTick;
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> has been unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            var dependencyProperty = GetDependencyProperty();

            AssociatedObject.ClearValue(dependencyProperty);
            AssociatedObject.SetBinding(dependencyProperty, _originalBinding);

            Log.Debug("Restored binding for dependency property '{0}'", UsedDependencyPropertyName);

            AssociatedObject.UnsubscribeFromDependencyProperty(PropertyName, OnDependencyPropertyChanged);

            Log.Debug("Unsubscribed from property changes of the original object");

            _timer.Stop();
            _timer.Tick -= OnTimerTick;
        }

        /// <summary>
        /// Called when the associated dependency property has changed.
        /// </summary>
        private void OnDependencyPropertyChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            if (UpdateDelay < 50)
            {
                UpdateBinding();
                return;
            }

            if (UpdateDelay > 5000)
            {
                UpdateDelay = 5000;
            }

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _timer.Interval = new TimeSpan(0, 0, 0, 0, UpdateDelay);
            _timer.Start();
        }

        /// <summary>
        /// Called when timer ticks.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void OnTimerTick(object sender, TimerTickEventArgs e)
        {
            _timer.Stop();

            UpdateBinding();
        }

        /// <summary>
        /// Updates the binding value.
        /// </summary>
        private void UpdateBinding()
        {
            if (!IsEnabled)
            {
                return;
            }

            var dependencyProperty = GetDependencyProperty();
            var bindingExpression = AssociatedObject.GetBindingExpression(dependencyProperty);

            bindingExpression.UpdateSource();
        }

        /// <summary>
        /// Gets the dependency property based on the properties of this behavior.
        /// </summary>
        /// <returns>The <see cref="DependencyProperty"/> of <c>null</c> if the dependency property is not found.</returns>
        private DependencyProperty GetDependencyProperty()
        {
            if (_dependencyPropertyCache != null)
            {
                return _dependencyPropertyCache;
            }

            DependencyProperty property;

            // Fallback first
            if (!string.IsNullOrEmpty(DependencyPropertyName))
            {
                property = GetDependencyProperty(DependencyPropertyName);
                if (property != null)
                {
                    property = _dependencyPropertyCache;
                    return property;
                }
            }

            property = GetDependencyProperty(string.Format("{0}Property", PropertyName));
            if (property != null)
            {
                _dependencyPropertyCache = property;
                return property;
            }

            return null;
        }

        /// <summary>
        /// Gets the dependency property with the specified property name.
        /// </summary>
        /// <param name="dependencyPropertyName">Name of the property.</param>
        /// <returns>The <see cref="DependencyProperty"/> or <c>null</c> if the dependency property is not found.</returns>
        private DependencyProperty GetDependencyProperty(string dependencyPropertyName)
        {
            DependencyProperty property = null;

            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(true, true);
            var fieldInfo = AssociatedObject.GetType().GetFieldEx(dependencyPropertyName, bindingFlags);
            if (fieldInfo != null)
            {
                property = fieldInfo.GetValue(null) as DependencyProperty;
            }

            if (property == null)
            {
                Log.Error("Failed to retrieve dependency property '{0}' from object '{1}'", dependencyPropertyName, AssociatedObject.GetType());
            }
            else
            {
                Log.Debug("Retrieved dependency property '{0}' from object '{1}'", dependencyPropertyName, AssociatedObject.GetType());
            }

            return property;
        }

        /// <summary>
        /// Creates the binding copy.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <returns>The duplicated binding.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="binding"/> is <c>null</c>.</exception>
        private static Binding CreateBindingCopy(Binding binding)
        {
            Argument.IsNotNull("binding", binding);

            // Copy all properties with a setter via reflection
            // only copy when value != null, otherwise exceptions will be thrown

            var newBinding = new Binding();

            foreach (var property in BindingProperties)
            {
                try
                {
                    if (!property.CanWrite)
                    {
                        continue;
                    }

                    var propertyValue = property.GetValue(binding, null);
                    if (propertyValue != null)
                    {
                        property.SetValue(newBinding, propertyValue, null);
                    }
                }
                catch (Exception)
                {
                }
            }

            return newBinding;
        }
        #endregion
    }
}

#endif