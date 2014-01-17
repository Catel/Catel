// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindableRun.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System.Windows.Documents;
    using System.Windows;

    /// <summary>
    /// Bindable run class.
    /// </summary>
    public class BindableRun : Run
	{
		#region Properties
		/// <summary>
        /// Wrapper for the BoundText dependency property.
        /// </summary>
        public string BoundText
        {
            get { return (string)GetValue(BoundTextProperty); }
            set { SetValue(BoundTextProperty, value); }
        }

		/// <summary>
		/// DependencyProperty definition as the backing store for BoundText
		/// </summary>
        public static readonly DependencyProperty BoundTextProperty = DependencyProperty.Register("BoundText", typeof(string),
            typeof(BindableRun), new UIPropertyMetadata(string.Empty, BoundText_Changed));
		#endregion

		#region Methods
		/// <summary>
        /// Invoked when the BoundText dependency property has changed.
        /// </summary>
        /// <param name="sender">The object that contains the dependency property.</param>
        /// <param name="e">The event data.</param>
        private static void BoundText_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BindableRun typedSender = sender as BindableRun;
            if (typedSender != null)
            {
                typedSender.Text = e.NewValue as string;
            }
		}
		#endregion
	}
}
