// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FocusFirstControl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Behavior to focus the first control in a window.
    /// </summary>
    public class FocusFirstControl : BehaviorBase<FrameworkElement>
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the parent should be focused first. 
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the parent should be focused first; otherwise, <c>false</c>.</value>
        public bool FocusParentsFirst
        {
            get { return (bool) GetValue(FocusParentsFirstProperty); }
            set { SetValue(FocusParentsFirstProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="FocusParentsFirst"/> property.
        /// </summary>
        public static readonly DependencyProperty FocusParentsFirstProperty = DependencyProperty.Register("FocusParentsFirst",
            typeof(bool), typeof(FocusFirstControl), new PropertyMetadata(true));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            base.OnAssociatedObjectLoaded(sender, e);

            AssociatedObject.FocusFirstControl(FocusParentsFirst);
        }
        #endregion
    }
}