// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiLineInputWindow.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System.Windows;

    /// <summary>
    /// Window that contains a <see cref="Controls.MultiLineInput"/> control so the user is able
    /// to edit a multiline text value.
    /// </summary>
    [ObsoleteEx(Message = "Will be removed, copy to your own solution if you use it", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public partial class MultiLineInputWindow : DataWindow
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of this window.
        /// </summary>
        public MultiLineInputWindow()
            : base(DataWindowMode.OkCancel)
        {
            Title = "Change text";
            Explanation = "Change the text below";

            InitializeComponent();

            this.SetOwnerWindowAndFocus();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Explanation (textbox caption).
        /// </summary>
        /// <remarks>
        /// Wrapper for the Explanation dependency property.
        /// </remarks>
        public string Explanation
        {
            get { return (string)GetValue(ExplanationProperty); }
            set { SetValue(ExplanationProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Explanation.
        /// </summary>
        public static readonly DependencyProperty ExplanationProperty = DependencyProperty.Register("Explanation",
            typeof(string), typeof(MultiLineInputWindow), new UIPropertyMetadata(CatelEnvironment.DefaultMultiLingualDependencyPropertyValue));

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
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string), typeof(MultiLineInputWindow), new UIPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets IsTextMandatory.
        /// </summary>
        /// <remarks>
        /// Wrapper for the IsTextMandatory dependency property.
        /// </remarks>
        public bool IsTextMandatory
        {
            get { return (bool)GetValue(IsTextMandatoryProperty); }
            set { SetValue(IsTextMandatoryProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for IsTextMandatory.
        /// </summary>
        public static readonly DependencyProperty IsTextMandatoryProperty = DependencyProperty.Register("IsTextMandatory",
            typeof(bool), typeof(MultiLineInputWindow), new UIPropertyMetadata(false));
        #endregion

        #region Methods
        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <returns>True if successful, otherwise false.</returns>
        protected override bool ValidateData()
        {
            bool isValid = base.ValidateData();

            if (isValid && IsTextMandatory)
            {
                isValid = Text.Trim().Length > 0;
            }

            return isValid;
        }
        #endregion
    }
}
