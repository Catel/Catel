// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoutedEventTrigger.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Event trigger that supports routed events.
    /// </summary>
    public class RoutedEventTrigger : EventTriggerBase<FrameworkElement>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the routed event.
        /// </summary>
        /// <value>The routed event.</value>
        public RoutedEvent RoutedEvent { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the required properties.
        /// </summary>
        protected override void ValidateRequiredProperties()
        {
            if (RoutedEvent == null)
            {
                throw new InvalidOperationException("RoutedEvent is a required property");
            }
        }

        /// <summary>
        /// Called when the <see cref="EventTriggerBase{T}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            if (RoutedEvent != null)
            {
                AssociatedObject.AddHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }
        }

        /// <summary>
        /// Called when the <see cref="EventTriggerBase{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
            if (RoutedEvent != null)
            {
                AssociatedObject.RemoveHandler(RoutedEvent, new RoutedEventHandler(OnRoutedEvent));
            }

            base.OnAssociatedObjectUnloaded(sender, e);
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
        #endregion
    }
}