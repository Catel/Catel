// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyChangedHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using Logging;

#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// Dependency property changed helper. This helper class allows to subscribe to any dependency property
    /// changed of any framework element element.
    /// </summary>
    public static class DependencyPropertyChangedHelper
    {
        private const string InheritedDataContextName = "InheritedDataContext";

        /// <summary>
        /// Cache containing already registered dependency properties.
        /// </summary>
        private static readonly Dictionary<string, DependencyProperty> _dependencyProperties = new Dictionary<string, DependencyProperty>();

        /// <summary>
        /// Dictionary containing a dependency to real dependency name mapping.
        /// </summary>
        private static readonly Dictionary<DependencyProperty, string> _wrapperDependencyProperties = new Dictionary<DependencyProperty, string>();

        /// <summary>
        /// Dictionary containing values whether a property is a real dependency property.
        /// </summary>
        private static readonly Dictionary<string, bool> _realDependencyPropertiesCache = new Dictionary<string, bool>();

        //private static readonly ILog Log = LogManager.GetLogger(typeof(DependencyPropertyChangedHelper));

        /// <summary>
        /// Determines whether the specified dependency property is a real dependency or a wrapper or handler one for internal usage.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the property is a real dependency property; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public static bool IsRealDependencyProperty(this FrameworkElement frameworkElement, string propertyName)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var type = frameworkElement.GetType();

            var key = DependencyPropertyHelper.GetDependencyPropertyCacheKey(type, propertyName);

            if (!_realDependencyPropertiesCache.TryGetValue(key, out var isRealDependencyProperty))
            {
                isRealDependencyProperty = true;

                if (propertyName.EndsWith("_handler"))
                {
                    isRealDependencyProperty = false;
                }
                else if (propertyName.Contains(DependencyPropertyHelper.GetDependencyPropertyCacheKeyPrefix(type)))
                {
                    isRealDependencyProperty = false;
                }

                _realDependencyPropertiesCache.Add(key, isRealDependencyProperty);
            }

            return isRealDependencyProperty;
        }

        /// <summary>
        /// Subscribes to all dependency properties of the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="handler">The handler to subscribe.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void SubscribeToAllDependencyProperties(this FrameworkElement frameworkElement, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("handler", handler);

            var dependencyProperties = frameworkElement.GetDependencyProperties();
            foreach (var dependencyProperty in dependencyProperties)
            {
                SubscribeToDependencyProperty(frameworkElement, dependencyProperty.PropertyName, handler);
            }
        }

        /// <summary>
        /// Subscribes to the change events of the inherited DataContext.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="handler">The handler to subscribe.</param>
        /// <param name="inherited">if set to <c>true</c>, check inherited data context as well.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void SubscribeToDataContext(this FrameworkElement frameworkElement, EventHandler<DependencyPropertyValueChangedEventArgs> handler,
            bool inherited)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("handler", handler);

            var propertyName = inherited ? InheritedDataContextName : "DataContext";

            SubscribeToDependencyProperty(frameworkElement, propertyName, handler);
        }

        /// <summary>
        /// Unsubscribes from all dependency properties of the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="handler">The handler to unsubscribe.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void UnsubscribeFromAllDependencyProperties(this FrameworkElement frameworkElement, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("handler", handler);

            var dependencyProperties = frameworkElement.GetDependencyProperties();
            foreach (var dependencyProperty in dependencyProperties)
            {
                UnsubscribeFromDependencyProperty(frameworkElement, dependencyProperty.PropertyName, handler);
            }
        }

        /// <summary>
        /// Unsubscribes from the change events of the inherited DataContext.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="handler">The handler to subscribe.</param>
        /// <param name="inherited">if set to <c>true</c>, check inherited data context as well.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void UnsubscribeFromDataContext(this FrameworkElement frameworkElement, EventHandler<DependencyPropertyValueChangedEventArgs> handler, bool inherited)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNull("handler", handler);

            var propertyName = inherited ? InheritedDataContextName : "DataContext";

            UnsubscribeFromDependencyProperty(frameworkElement, propertyName, handler);
        }

        /// <summary>
        /// Subscribes to the specified dependency property of the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">The name of the dependency property to subscribe to.</param>
        /// <param name="handler">The handler to subscribe.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void SubscribeToDependencyProperty(this FrameworkElement frameworkElement, string propertyName, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("handler", handler);

            //Log.Debug("Subscribing to changed event of '{0}' for framework element '{1}'", frameworkElement.GetType().FullName, propertyName);

            var dependencyProperty = GetDependencyProperty<object>(frameworkElement, propertyName);
            if (frameworkElement.GetValue(dependencyProperty) == null)
            {
                var binding = new Binding();

                if (!string.Equals(propertyName, InheritedDataContextName))
                {
                    binding.Source = frameworkElement;
                    binding.Path = new PropertyPath(propertyName);
                }

                binding.Mode = BindingMode.OneWay;

                frameworkElement.SetBinding(dependencyProperty, binding);
            }

            var handlerDependencyPropertyName = GetHandlerDependencyPropertyName(propertyName);
            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, handlerDependencyPropertyName);

            var internalHandler = (EventHandler<DependencyPropertyValueChangedEventArgs>)frameworkElement.GetValue(handlerDependencyProperty);
            internalHandler += handler;
            frameworkElement.SetValue(handlerDependencyProperty, internalHandler);

            //Log.Debug("Subscribed to changed event of '{0}' for framework element '{1}'", frameworkElement.GetType().FullName, propertyName);
        }

        /// <summary>
        /// Subscribes from the specified dependency property of the specified <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">The name of the dependency property to unsubscribe from.</param>
        /// <param name="handler">The handler to unsubscribe.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void UnsubscribeFromDependencyProperty(this FrameworkElement frameworkElement, string propertyName, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("handler", handler);

            //Log.Debug("Unsubscribing from changed event of '{0}' for framework element '{1}'", frameworkElement.GetType().FullName, propertyName);

            var handlerDependencyPropertyName = GetHandlerDependencyPropertyName(propertyName);
            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, handlerDependencyPropertyName);
            var internalHandler = (EventHandler<DependencyPropertyValueChangedEventArgs>)frameworkElement.GetValue(handlerDependencyProperty);
            if (internalHandler != null)
            {
                internalHandler -= handler;
            }

            if (internalHandler != null)
            {
                frameworkElement.SetValue(handlerDependencyProperty, internalHandler);
            }
            else
            {
                frameworkElement.ClearValue(handlerDependencyProperty);

                var dependencyProperty = GetDependencyProperty<object>(frameworkElement, propertyName);
                frameworkElement.ClearValue(dependencyProperty);
            }

            //Log.Debug("Unsubcribed from changed event of '{0}' for framework element '{1}'", frameworkElement.GetType().FullName, propertyName);
        }

        /// <summary>
        /// Called when a dependency property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = ((FrameworkElement)sender);
            var propertyName = _wrapperDependencyProperties[e.Property];

            if (!IsRealDependencyProperty(frameworkElement, propertyName))
            {
                return;
            }

            //Log.Debug("OnDependencyPropertyChanged: '{0}' to {1}", propertyName, e.NewValue);

            var handlerDependencyPropertyName = GetHandlerDependencyPropertyName(propertyName);
            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, handlerDependencyPropertyName);

            var handler = (EventHandler<DependencyPropertyValueChangedEventArgs>)frameworkElement.GetValue(handlerDependencyProperty);
            if (handler != null)
            {
                if (string.Equals(propertyName, InheritedDataContextName))
                {
                    propertyName = "DataContext";
                }

                handler(sender, new DependencyPropertyValueChangedEventArgs(propertyName, e));
            }
        }

        /// <summary>
        /// Gets the dependency property from the cache. If it does not yet exist, it will create the dependency property and
        /// add it to the cache.
        /// </summary>
        /// <typeparam name="T">The type of the dependency property.</typeparam>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The dependency property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="frameworkElement"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        private static DependencyProperty GetDependencyProperty<T>(FrameworkElement frameworkElement, string propertyName)
        {
            var viewType = frameworkElement.GetType();
            var key = DependencyPropertyHelper.GetDependencyPropertyCacheKey(viewType, propertyName);

            if (!_dependencyProperties.TryGetValue(key, out var dependencyProperty))
            {
                // If called with object, this is the request for the dummy value containing the mapped dependency property
                // on which we subscribe for changess. Otherwise this is the dependency property containing the actual
                // handlers to call when the property changes.
                var dependencyPropertyMetaData = typeof(T) == typeof(object) ? new PropertyMetadata(default(T), OnDependencyPropertyChanged) : null;
                dependencyProperty = DependencyProperty.RegisterAttached(key, typeof(T), viewType, dependencyPropertyMetaData);

                _dependencyProperties[key] = dependencyProperty;
                _wrapperDependencyProperties[dependencyProperty] = propertyName;
            }

            return dependencyProperty;
        }

        /// <summary>
        /// Gets the name of the handler dependency property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>`
        /// <returns>The name of the dependency property containing the changed handler for the actual dependency property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        private static string GetHandlerDependencyPropertyName(string propertyName)
        {
            return string.Format("{0}_handler", propertyName);
        }
    }
}

#endif
