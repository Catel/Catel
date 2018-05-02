// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextChangedHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows
{
    using System;
    using System.Windows;
    using Data;
    using IoC;
    using MVVM;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#endif

    /// <summary>
    /// Helper class to subscribe to the <c>DataContextChanged</c> event of UI elements.
    /// </summary>
    public static class DataContextChangedHelper
    {
        /// <summary>
        /// Adds the data context changed handler.
        /// </summary>
        /// <param name="element">Element to which the handler is added.</param>
        /// <param name="handler">The handler to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler" /> is <c>null</c>.</exception>
        public static void AddDataContextChangedHandler(this FrameworkElement element, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            var subscriptionMode = GetDataContextSubscriptionMode(element);

            element.SubscribeToDataContext(handler, subscriptionMode == DataContextSubscriptionMode.InheritedDataContext);
        }

        /// <summary>
        /// Removes the data context changed handler.
        /// </summary>
        /// <param name="element">The element from which the handler has to be removed.</param>
        /// <param name="handler">The handler to remove.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void RemoveDataContextChangedHandler(this FrameworkElement element, EventHandler<DependencyPropertyValueChangedEventArgs> handler)
        {
            var subscriptionMode = GetDataContextSubscriptionMode(element);

            element.UnsubscribeFromDataContext(handler, subscriptionMode == DataContextSubscriptionMode.InheritedDataContext);
        }

        private static DataContextSubscriptionMode GetDataContextSubscriptionMode(FrameworkElement element)
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var dataContextSubscriptionService = dependencyResolver.Resolve<IDataContextSubscriptionService>();

            return dataContextSubscriptionService.GetDataContextSubscriptionMode(element.GetType());
        }
    }
}

#endif