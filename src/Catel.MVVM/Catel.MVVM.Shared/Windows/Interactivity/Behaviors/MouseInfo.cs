// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseInfo.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SILVERLIGHT

namespace Catel.Windows.Interactivity
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Trigger that enables a property to bind the several mouse events for the associated object.
    /// </summary>
    public class MouseInfo : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the mouse is currently over the associated object.
        /// </summary>
        /// <value>
        /// <c>true</c> if the mouse is currently over the associated object; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(MouseInfo), new PropertyMetadata(false));

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, UIEventArgs e)
        {
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, UIEventArgs e)
        {
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        /// <summary>
        /// Called when the mouse enters the associated object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMouseEnter(object sender, EventArgs e)
        {
            IsMouseOver = true;
        }

        /// <summary>
        /// Called when the mouse leaves the associated object.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMouseLeave(object sender, EventArgs e)
        {
            IsMouseOver = false;
        }
    }
}

#endif