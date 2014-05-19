// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadioButtonEx.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A radio button that solves the 2-way binding issue of the <c>RadioButton.IsChecked</c> property in WPF 3.5.
    /// <para />
    /// To solve the issue, use this <see cref="RadioButton"/> implementation and bind to the <see cref="IsCheckedReal"/> instead of
    /// <c>RadioButton.IsChecked</c>.
    /// <para />
    /// For more information about the issue, see this forum thread:
    /// http://social.msdn.microsoft.com/forums/en-US/wpf/thread/8eb8280a-19c4-4502-8260-f74633a9e2f2/
    /// </summary>
    /// <remarks>
    /// This code is originally found at http://pstaev.blogspot.com/2008/10/binding-ischecked-property-of.html.
    /// </remarks>
    public class RadioButtonEx : RadioButton
    {
        #region Fields
        private bool _isChanging;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonEx"/> class.
        /// </summary>
        public RadioButtonEx()
        {
            Checked += RadioButtonExtended_Checked;
            Unchecked += RadioButtonExtended_Unchecked;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets IsCheckedReal.
        /// </summary>
        /// <remarks>
        /// Wrapper for the IsCheckedReal dependency property.
        /// </remarks>
        public bool IsCheckedReal
        {
            get { return (bool)GetValue(IsCheckedRealProperty); }
            set { SetValue(IsCheckedRealProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for IsCheckedReal.
        /// </summary>
        public static readonly DependencyProperty IsCheckedRealProperty = DependencyProperty.Register("IsCheckedReal", typeof(bool), typeof(RadioButtonEx), 
            new FrameworkPropertyMetadata(false,  FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (sender, e) => ((RadioButtonEx)sender).OnIsCheckedRealChanged()));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the <see cref="IsCheckedReal"/> property has changed.
        /// </summary>
        private void OnIsCheckedRealChanged()
        {
            _isChanging = true;
            IsChecked = IsCheckedReal;
            _isChanging = false;
        }

        /// <summary>
        /// Handles the Unchecked event of the RadioButtonExtended control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RadioButtonExtended_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_isChanging)
            {
                IsCheckedReal = false;
            }
        }

        /// <summary>
        /// Handles the Checked event of the RadioButtonExtended control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RadioButtonExtended_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isChanging)
            {
                IsCheckedReal = true;
            }
        }
        #endregion
    }
}

#endif