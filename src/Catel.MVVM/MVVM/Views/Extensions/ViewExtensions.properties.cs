namespace Catel.MVVM.Views
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Catel.Windows.Data;
    using System.Windows;

    public static partial class ViewExtensions
    {
        /// <summary>
        /// Gets the properties of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>List of properties.</returns>
        public static string[] GetProperties(this IView view)
        {
            ArgumentNullException.ThrowIfNull(view);

            var viewProperties = ((FrameworkElement)view).GetDependencyProperties();
            return viewProperties.Select(x => x.PropertyName).ToArray();
        }

        /// <summary>
        /// Gets the properties of the view.
        /// </summary>
        /// <param name="viewType">The view type.</param>
        /// <returns>List of properties.</returns>
        public static string[] GetProperties(Type viewType)
        {
            ArgumentNullException.ThrowIfNull(viewType);

            var viewProperties = Catel.Windows.Data.DependencyPropertyHelper.GetDependencyProperties(viewType);
            return viewProperties.Select(x => x.PropertyName).ToArray();
        }

        /// <summary>
        /// Subscribes to the property changed event.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="handler">The handler.</param>
        public static void SubscribeToPropertyChanged(this IView view, string propertyName, EventHandler<PropertyChangedEventArgs> handler)
        {
            ArgumentNullException.ThrowIfNull(view);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);
            ArgumentNullException.ThrowIfNull(handler);

            ((FrameworkElement)view).SubscribeToDependencyProperty(propertyName, (sender, e) =>
            {
                if (sender is FrameworkElement senderFx)
                {
                    if (!senderFx.IsRealDependencyProperty(e.PropertyName))
                    {
                        // Ignore, this is a wrapper
                        return;
                    }
                }

                handler(sender, new PropertyChangedEventArgs(e.PropertyName));
            });
        }
    }
}
