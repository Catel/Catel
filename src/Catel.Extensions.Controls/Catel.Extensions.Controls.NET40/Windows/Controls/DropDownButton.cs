// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropDownButton.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   DropDownButton, which is unfortunately missing in the controls library of WPF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;

	/// <summary>
	/// DropDownButton, which is unfortunately missing in the controls library of WPF.
	/// </summary>
	/// <remarks>
	/// The code for this control is based on the code found at http://andyonwpf.blogspot.com/2006/10/dropdownbuttons-in-wpf.html.
	/// </remarks>
	public class DropDownButton : ToggleButton
	{
		#region Fields
		private Popup _popup;
		private DispatcherTimer _dispatcherTimer;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DropDownButton"/> class.
		/// </summary>
		public DropDownButton()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets DropDownContent.
		/// </summary>
		/// <remarks>
		/// Wrapper for the DropDownContent dependency property.
		/// </remarks>
		public UIElement DropDownContent
		{
			get { return (UIElement)GetValue(DropDownContentProperty); }
			set { SetValue(DropDownContentProperty, value); }
		}

		/// <summary>
		/// DependencyProperty definition as the backing store for DropDownContent.
		/// </summary>
		public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(UIElement),
			typeof(DropDownButton), new UIPropertyMetadata(null, (sender, e) => ((DropDownButton)sender).OnDropDownContentChanged(sender, e)));
		#endregion

		#region Methods
		/// <summary>
		/// Invoked when the DropDownContent dependency property has changed.
		/// </summary>
		/// <param name="sender">The object that contains the dependency property.</param>
		/// <param name="e">The event data.</param>
		private void OnDropDownContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (_popup != null)
			{
				_popup.LostFocus -= OnPopupLostFocus;
			}

			if ((e.NewValue == null) || !(e.NewValue is UIElement))
			{
				_popup = null;
				return;
			}

			_popup = new Popup();
			_popup.Child = (UIElement)e.NewValue;
			_popup.StaysOpen = false;
			_popup.PopupAnimation = PopupAnimation.Fade;
			_popup.LostFocus += OnPopupLostFocus;
		}

		/// <summary>
		/// Called when the popup has lost its focus.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void OnPopupLostFocus(object sender, RoutedEventArgs e)
		{
			if (_dispatcherTimer == null)
			{
				_dispatcherTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 40), DispatcherPriority.Normal, OnDispatcherTimerTick, Dispatcher);
			}

			_dispatcherTimer.Start();
		}

		/// <summary>
		/// Called when the dispatcher timer has invoked the tick event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void OnDispatcherTimerTick(object sender, EventArgs e)
		{
			ResetTimer();

			if (_popup != null)
			{
			    _popup.IsOpen = false;
			}
		}

		/// <summary>
		/// Called when a control is clicked by the mouse or the keyboard.
		/// </summary>
		protected override void OnClick()
		{
			// If there is a drop-down assigned to this button, then position and display it 
			if (_popup != null)
			{
				_popup.PlacementTarget = this;
				_popup.Placement = PlacementMode.Bottom;
				_popup.IsOpen = true;
				_popup.Child.Focus();
			}
		}

		/// <summary>
		/// Resets the timer.
		/// </summary>
		private void ResetTimer()
		{
			if (_dispatcherTimer != null)
			{
				_dispatcherTimer.Stop();
				_dispatcherTimer = null;
			}
		}
		#endregion
	}
}
