// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumbericTextBox.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System.Windows.Controls;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// Behavior to only allow numeric input on a <see cref="TextBox"/>.
    /// </summary>
    public class NumericTextBox : BehaviorBase<TextBox>
    {
        private const string _minusCharacter = "-";
        private const string _periodCharacter = ".";
        private const string _commaCharacter = ",";

        private static readonly List<Key> AllowedKeys = new List<Key>
        {
            Key.Back,
            Key.CapsLock,                                                                
            #if SILVERLIGHT                                                                
                //Key.Ctrl
            #else
                Key.LeftCtrl,
                Key.RightCtrl,
            #endif                                                                
            Key.Down,
            Key.End,
            Key.Enter,
            Key.Escape,
            Key.Home,
            Key.Insert,
            Key.Left,
            Key.PageDown,
            Key.PageUp,
            Key.Right,
            #if SILVERLIGHT
                //Key.Shift                                                                
            #else
                Key.LeftShift,
                Key.RightShift,
            #endif
            Key.Tab,
            Key.Up                                                                                                                                                                                          
        };

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            AssociatedObject.KeyDown += OnAssociatedObjectKeyDown;
            AssociatedObject.TextChanged += OnAssociatedObjectTextChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether negative values are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow negative]; otherwise, <c>false</c>.
        /// </value>
        public bool IsNegativeAllowed
        {
            get { return (bool)GetValue(IsNegativeAllowedProperty); }
            set
            {
                if (value)
                {                    
#if NET
                    AllowedKeys.Add(Key.OemMinus);
                }
                else
                {                  
                    if (AllowedKeys.Contains(Key.OemMinus))
                    {
                        AllowedKeys.Remove(Key.OemMinus);                        
                    }
#endif
                }

                SetValue(IsNegativeAllowedProperty, value);
            }
        }
        
        /// <summary>
        /// Are negative numbers allowed
        /// </summary>
        public static readonly DependencyProperty IsNegativeAllowedProperty =
            DependencyProperty.Register("IsNegativeAllowed", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether decimal values are allowed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [IS decimal allowed]; otherwise, <c>false</c>.
        /// </value>
        public bool IsDecimalAllowed
        {
            get { return (bool)GetValue( IsDecimalAllowedProperty); }
            set { SetValue( IsDecimalAllowedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDecimalAllowed.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty  IsDecimalAllowedProperty =
            DependencyProperty.Register("IsDecimalAllowed", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(true));

        /// <summary>
        /// Called when the <see cref="UIElement.KeyDown"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>        
        private void OnAssociatedObjectKeyDown(object sender, KeyEventArgs e)
        {
            bool handled = true;
            string keyValue = GetKeyValue(e);

            System.Globalization.CultureInfo currentCi = System.Threading.Thread.CurrentThread.CurrentCulture;
            string numberDecimalSeparator = currentCi.NumberFormat.NumberDecimalSeparator;

            if (keyValue == numberDecimalSeparator && IsDecimalAllowed)
            {
                handled = AssociatedObject.Text.Contains(numberDecimalSeparator);
            }
#if SILVERLIGHT
            else if (keyValue == _minusCharacter && IsNegativeAllowed)
            {
                handled = AssociatedObject.Text.Length > 0;  
            }
            else if (AllowedKeys.Contains(e.Key) || IsDigit(e.Key))
            {
                handled = false;
            }
#else
            else if (AllowedKeys.Contains(e.Key) || IsDigit(e.Key))
            {
                handled = (e.Key == Key.OemMinus && ((TextBox)sender).CaretIndex > 0 && IsNegativeAllowed );                
            }
#endif
            e.Handled = handled;
        }

        /// <summary>
        /// Called when the <c>TextBox.TextChanged</c> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectTextChanged(object sender, TextChangedEventArgs e)
        {            
            var binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (binding == null)
            {
                return;
            }

            binding.UpdateSource();
        }       

        /// <summary>
        /// Determines whether the specified key is a digit.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key is digit; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDigit(Key key)
        {
            bool isDigit;

            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;

            if (key >= Key.D0 && key <= Key.D9 && !isShiftKey)
            {
                isDigit = true;
            }
            else
            {
                isDigit = key >= Key.NumPad0 && key <= Key.NumPad9;
            }

            return isDigit; 
        }

        /// <summary>
        /// Gets the Key to a string value.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private string GetKeyValue(KeyEventArgs e)
        {
            string keyValue = string.Empty;

#if NET
            if (e.Key == Key.OemMinus)
            {
                keyValue = _minusCharacter;
            }
            else if (e.Key == Key.OemComma)
            {
                keyValue = _commaCharacter;
            }
            else if (e.Key == Key.OemPeriod)
            {
                keyValue = _periodCharacter;
            }
#else
            if (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 110)
            {
                keyValue = _periodCharacter;
            }
            else if(e.PlatformKeyCode == 188)
            {                 
                keyValue = _commaCharacter;
            }
            else if(e.PlatformKeyCode == 189)
            {                 
                keyValue = _minusCharacter;
            }
            else
            {
                keyValue = e.Key.ToString().Replace("D", "").Replace("NumPad", "");
            }  
#endif

            return keyValue;
        }
    }
}

