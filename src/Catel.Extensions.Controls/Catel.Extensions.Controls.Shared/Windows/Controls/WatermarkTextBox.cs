// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WatermarkTextBox.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

#if NET
    using System;
    using System.Windows.Input;
#endif

    /// <summary>
    /// WatermarkTextBox which is a simple <see cref="TextBox"/> that is able to show simple and complex watermarks.
    /// </summary>
    [ObsoleteEx(ReplacementTypeOrMember = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class WatermarkTextBox : TextBox
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TextBox"/> class.
        /// </summary>
        static WatermarkTextBox()
        {
#if NET
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.TextBox"/> class.
        /// </summary>
        public WatermarkTextBox()
        {
#if SILVERLIGHT
            DefaultStyleKey = GetType();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether all text should be selected when the control receives the focus.
        /// </summary>
        /// <value><c>true</c> if all text should be selected when the control receives the focus; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        private bool SelectAllOnGotFocus
        {
            get { return (bool)GetValue(SelectAllOnGotFocusProperty); }
            set { SetValue(SelectAllOnGotFocusProperty, value); }
        }

        /// <summary>
        /// Dependency property registration for the <see cref="SelectAllOnGotFocus"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectAllOnGotFocusProperty = DependencyProperty.Register("SelectAllOnGotFocus", typeof(bool), typeof(WatermarkTextBox), new PropertyMetadata(false));


        /// <summary>
        /// Gets or sets the watermark to show.
        /// </summary>
        /// <value>The watermark.</value>
        /// <remarks></remarks>
        public object Watermark
        {
            get { return GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Dependency property registration for the <see cref="Watermark"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(object), typeof(WatermarkTextBox), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the watermark template to show.
        /// </summary>
        /// <value>The watermark template.</value>
        /// <remarks></remarks>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// Dependency property registration for the <see cref="WatermarkTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register("WatermarkTemplate", typeof(DataTemplate), typeof(WatermarkTextBox), new PropertyMetadata(null));
        #endregion

        #region Methods
#if SILVERLIGHT
        /// <summary>
        /// Called before <see cref="E:System.Windows.UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        /// <remarks></remarks>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (SelectAllOnGotFocus)
            {
                SelectAll();
            }
        }
#endif

#if NET
        /// <summary>
        /// Invoked whenever an unhandled <c>System.Windows.Input.Keyboard.GotKeyboardFocus</c> attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        /// <remarks></remarks>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (SelectAllOnGotFocus)
            {
                SelectAll();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        /// <remarks></remarks>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocused && SelectAllOnGotFocus)
            {
                e.Handled = true;
                Focus();
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }
#endif
        #endregion
    }
}

#endif