// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextChangedHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows
{
    using System;
    using System.Windows;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Data;
#else
    using System.Windows.Data;
#endif

    /// <summary>
    /// Helper class to subscribe to the <c>DataContextChanged</c> event of UI elements in Silverlight.
    /// </summary>
    /// <remarks>
    /// This code is originally found at http://www.pochet.net/blog/2010/06/16/silverlight-datacontext-changed-event-and-trigger/.
    /// </remarks>
    public static class DataContextChangedHelper
    {
        /// <summary>
        /// Identifies the InheritedDataContext DependencyProperty.
        /// </summary>
        private static readonly DependencyProperty InheritedDataContextProperty = DependencyProperty.RegisterAttached("InheritedDataContext",
            typeof(object), typeof(DataContextChangedHelper), new PropertyMetadata(null, OnInheritedDataContextChanged));

        /// <summary>
        /// Identifies the DataContextChangedHandler DependencyProperty.
        /// </summary>
        private static readonly DependencyProperty DataContextChangedHandlerProperty = DependencyProperty.RegisterAttached("DataContextChangedHandler",
            typeof(DependencyPropertyChangedEventHandler), typeof(DataContextChangedHelper), null);

        /// <summary>
        /// Handles changes to the InheritedDataContext DependencyProperty.
        /// </summary>
        /// <param name="d">Instance with property change.</param>
        /// <param name="e">Property change details.</param>
        private static void OnInheritedDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var handler = d.GetValue(DataContextChangedHandlerProperty) as DependencyPropertyChangedEventHandler;
            if (handler != null)
            {
                handler(d, e);
            }
        }

        /// <summary>
        /// Adds the data context changed handler.
        /// </summary>
        /// <param name="element">Element to which the handler is added.</param>
        /// <param name="handler">The handler to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void AddDataContextChangedHandler(this FrameworkElement element, DependencyPropertyChangedEventHandler handler)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNull("handler", handler);

            if (element.GetValue(InheritedDataContextProperty) == null)
            {
                element.SetBinding(InheritedDataContextProperty, new Binding());
            }

            var currentHandler = (DependencyPropertyChangedEventHandler)element.GetValue(DataContextChangedHandlerProperty);
            currentHandler += handler;
            element.SetValue(DataContextChangedHandlerProperty, currentHandler);
        }

        /// <summary>
        /// Removes the data context changed handler.
        /// </summary>
        /// <param name="element">The element from which the handler has to be removed.</param>
        /// <param name="handler">The handler to remove.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="element"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void RemoveDataContextChangedHandler(this FrameworkElement element, DependencyPropertyChangedEventHandler handler)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNull("handler", handler);

            var currentHandler = (DependencyPropertyChangedEventHandler)element.GetValue(DataContextChangedHandlerProperty);
            currentHandler -= handler;
            if (currentHandler == null)
            {
                element.ClearValue(DataContextChangedHandlerProperty);
                element.ClearValue(InheritedDataContextProperty);
            }
            else
            {
                element.SetValue(DataContextChangedHandlerProperty, currentHandler);
            }
        }
    }
}

#endif