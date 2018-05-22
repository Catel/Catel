// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseInfo.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UWP

namespace Catel.Windows.Interactivity
{
    using System;

    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Input;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;

    /// <summary>
    /// Trigger that enables a property to bind the several mouse events for the associated object.
    /// </summary>
    public class PointerInfo : BehaviorBase<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is pointer over.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pointer over; otherwise, <c>false</c>.
        /// </value>
        public bool IsPointerOver
        {
            get { return (bool)GetValue(IsPointerOverProperty); }
            set { SetValue(IsPointerOverProperty, value); }
        }

        /// <summary>
        /// The is pointer over property.
        /// </summary>
        public static readonly DependencyProperty IsPointerOverProperty = 
            DependencyProperty.Register("IsPointerOver", typeof(bool), typeof(PointerInfo), new PropertyMetadata(null));

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.PointerEntered += OnPointerEntered;
            AssociatedObject.PointerExited += PointerExited;
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.PointerEntered -= OnPointerEntered;
            AssociatedObject.PointerExited -= PointerExited;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsPointerOver = true;
        }

        private void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsPointerOver = false;
        }
    }
}

#endif