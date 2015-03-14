// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyChangedExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Extension methods for the <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public static class INotifyPropertyChangedExtensions
    {
        /// <summary>
        /// Subscribes to the specified property.
        /// </summary>
        /// <param name="notifyPropertyChanged">The notify property changed.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="notifyPropertyChanged"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public static void SubscribeToPropertyChanged(this INotifyPropertyChanged notifyPropertyChanged, string propertyName, EventHandler<PropertyChangedEventArgs> handler)
        {
            Argument.IsNotNull("notifyPropertyChanged", notifyPropertyChanged);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            Argument.IsNotNull("handler", handler);

            // TODO: Check for a way to prevent memory leaks

            notifyPropertyChanged.PropertyChanged += (sender, e) =>
            {
                if (string.Equals(e.PropertyName, propertyName))
                {
                    handler(sender, e);
                }
            };
        }
    }
}