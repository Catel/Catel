namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Event trigger that supports routed events.
    /// </summary>
    public class RoutedEventTrigger : EventTriggerBase<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the routed event.
        /// </summary>
        /// <value>The routed event.</value>
#pragma warning disable WPF0107
        public RoutedEvent RoutedEvent { get; set; }
#pragma warning restore WPF0107

        /// <summary>
        /// Validates the required properties.
        /// </summary>
        protected override void ValidateRequiredProperties()
        {
            if (RoutedEvent is null)
            {
                throw new InvalidOperationException("RoutedEvent is a required property");
            }
        }

        /// <summary>
        /// Called when the <see cref="EventTriggerBase{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            if (RoutedEvent is not null)
            {
                AssociatedObject.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }
        }

        /// <summary>
        /// Called when the <see cref="EventTriggerBase{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            if (RoutedEvent is not null)
            {
                AssociatedObject.RemoveHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }

            base.OnAssociatedObjectUnloaded();
        }

        /// <summary>
        /// Called when the routed event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnRoutedEvent(object sender, RoutedEventArgs args)
        {
            base.OnEvent(args);
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        protected override string GetEventName()
        {
            return RoutedEvent.Name;
        }
    }
}
