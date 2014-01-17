// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyChangedHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Logging;

#if NETFX_CORE
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

            string key = DependencyPropertyHelper.GetDependencyPropertyCacheKey(frameworkElement, propertyName);

            if (!_realDependencyPropertiesCache.ContainsKey(key))
            {
                bool isRealDependencyProperty = true;

                if (propertyName.EndsWith("_handler"))
                {
                    isRealDependencyProperty = false;
                }
                else if (propertyName.Contains(DependencyPropertyHelper.GetDependencyPropertyCacheKeyPrefix(frameworkElement)))
                {
                    isRealDependencyProperty = false;
                }

                _realDependencyPropertiesCache.Add(key, isRealDependencyProperty);
            }

            return _realDependencyPropertiesCache[key];
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
                var propertyPath = new PropertyPath(propertyName);
                var binding = new Binding { Path = propertyPath, Source = frameworkElement };
                frameworkElement.SetBinding(dependencyProperty, binding);
            }

            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, GetHandlerDependencyPropertyName(propertyName));
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

            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, GetHandlerDependencyPropertyName(propertyName));
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
            var frameworkElement = ((FrameworkElement) sender);
            var propertyName = _wrapperDependencyProperties[e.Property];

            //Log.Debug("OnDependencyPropertyChanged: '{0}' to {1}", propertyName, e.NewValue);

            var handlerDependencyProperty = GetDependencyProperty<EventHandler<DependencyPropertyValueChangedEventArgs>>(frameworkElement, GetHandlerDependencyPropertyName(propertyName));
            var handler = frameworkElement.GetValue(handlerDependencyProperty) as EventHandler<DependencyPropertyValueChangedEventArgs>;
            if (handler != null)
            {
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
            Argument.IsNotNull("frameworkElement", frameworkElement);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            var key = DependencyPropertyHelper.GetDependencyPropertyCacheKey(frameworkElement, propertyName);

            if (!_dependencyProperties.ContainsKey(key))
            {
                var dependencyProperty = DependencyProperty.RegisterAttached(key, typeof(T), frameworkElement.GetType(),
                    typeof(T) == typeof(object) ? new PropertyMetadata(default(T), OnDependencyPropertyChanged) : null);

                _dependencyProperties[key] = dependencyProperty;
                _wrapperDependencyProperties[dependencyProperty] = propertyName;
            }

            return _dependencyProperties[key];
        }

        /// <summary>
        /// Gets the name of the handler dependency property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>`
        /// <returns>The name of the dependency property containing the changed handler for the actual dependency property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        private static string GetHandlerDependencyPropertyName(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return string.Format("{0}_handler", propertyName);
        }
    }
}