// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewExtensions.properties.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    

#if XAMARIN
    
#elif NETFX_CORE
    using Catel.Windows.Data;
    using global::Windows.UI.Xaml;
#else
    using Catel.Windows.Data;
    using System.Windows;
#endif



    public static partial class ViewExtensions
    {
        /// <summary>
        /// Gets the properties of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>List of properties.</returns>
        public static string[] GetProperties(this IView view)
        {
            Argument.IsNotNull("view", view);

#if !XAMARIN
            var viewProperties = ((FrameworkElement)view).GetDependencyProperties();
            return viewProperties.Select(x => x.PropertyName).ToArray();
#else
            throw new MustBeImplementedException();
#endif
        }

        /// <summary>
        /// Subscribes to the property changed event.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="handler">The handler.</param>
        public static void SubscribeToPropertyChanged(this IView view, string propertyName, EventHandler<PropertyChangedEventArgs> handler)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("handler", handler);

#if !XAMARIN
            ((FrameworkElement)view).SubscribeToDependencyProperty(propertyName, (sender, e) =>
            {
                if (!((FrameworkElement)sender).IsRealDependencyProperty(e.PropertyName))
                {
                    // Ignore, this is a wrapper
                    return;
                }

                handler(sender, new PropertyChangedEventArgs(e.PropertyName));
            });
#else

#endif
        }
    }
}