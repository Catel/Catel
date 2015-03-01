// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLineInput.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Controls
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MultiLineInput.xaml
    /// </summary>
    public partial class MultiLineInput : System.Windows.Controls.UserControl
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of this control.
        /// </summary>
        public MultiLineInput()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Caption.
        /// </summary>
        /// <remarks>
        /// Wrapper for the Caption dependency property.
        /// </remarks>
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Caption.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(MultiLineInput), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        /// <remarks>
        /// Wrapper for the Text dependency property.
        /// </remarks>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MultiLineInput), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or sets MaxTextLength.
        /// </summary>
        /// <remarks>
        /// Wrapper for the MaxTextLength dependency property.
        /// </remarks>
        public int MaxTextLength
        {
            get { return (int)GetValue(MaxTextLengthProperty); }
            set { SetValue(MaxTextLengthProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for MaxTextLength.
        /// </summary>
        public static readonly DependencyProperty MaxTextLengthProperty =
            DependencyProperty.Register("MaxTextLength", typeof(int), typeof(MultiLineInput), new UIPropertyMetadata(0));

        /// <summary>
        /// Gets or sets IsReadOnly.
        /// </summary>
        /// <remarks>
        /// Wrapper for the IsReadOnly dependency property.
        /// </remarks>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for IsReadOnly.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MultiLineInput), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets TextBoxBackground.
        /// </summary>
        /// <remarks>
        /// Wrapper for the TextBoxBackground dependency property.
        /// </remarks>
        public Brush TextBoxBackground
        {
            get { return (Brush)GetValue(TextBoxBackgroundProperty); }
            set { SetValue(TextBoxBackgroundProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for TextBoxBackground.
        /// </summary>
        public static readonly DependencyProperty TextBoxBackgroundProperty =
            DependencyProperty.Register("TextBoxBackground", typeof(Brush), typeof(MultiLineInput), new UIPropertyMetadata(Brushes.White));
        #endregion

        #region Methods
        #endregion
    }
}