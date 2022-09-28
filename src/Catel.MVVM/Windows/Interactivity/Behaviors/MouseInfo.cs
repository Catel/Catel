namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    using UIEventArgs = System.EventArgs;

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
            DependencyProperty.Register(nameof(IsMouseOver), typeof(bool), typeof(MouseInfo), new PropertyMetadata(false));

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
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
