namespace Catel.Windows
{
    using System;
    using System.Windows;
    using Data;
    using MVVM;

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
        /// <param name="dataContextSubscriptionService">The data context subscription service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler" /> is <c>null</c>.</exception>
        public static void AddDataContextChangedHandler(this FrameworkElement element, EventHandler<DependencyPropertyValueChangedEventArgs> handler, IDataContextSubscriptionService dataContextSubscriptionService)
        {
            var subscriptionMode = GetDataContextSubscriptionMode(element, dataContextSubscriptionService);

            element.SubscribeToDataContext(handler, subscriptionMode == DataContextSubscriptionMode.InheritedDataContext);
        }

        /// <summary>
        /// Removes the data context changed handler.
        /// </summary>
        /// <param name="element">The element from which the handler has to be removed.</param>
        /// <param name="handler">The handler to remove.</param>
        /// <param name="dataContextSubscriptionService">The data context subscription service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void RemoveDataContextChangedHandler(this FrameworkElement element, EventHandler<DependencyPropertyValueChangedEventArgs> handler, IDataContextSubscriptionService dataContextSubscriptionService)
        {
            var subscriptionMode = GetDataContextSubscriptionMode(element, dataContextSubscriptionService);

            element.UnsubscribeFromDataContext(handler, subscriptionMode == DataContextSubscriptionMode.InheritedDataContext);
        }

        private static DataContextSubscriptionMode GetDataContextSubscriptionMode(FrameworkElement element, IDataContextSubscriptionService dataContextSubscriptionService)
        {
            return dataContextSubscriptionService.GetDataContextSubscriptionMode(element.GetType());
        }
    }
}
