// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleChoiceWindow.xaml.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for MultipleChoiceWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System.Collections.Generic;
    using System.Windows;

	/// <summary>
	/// Interaction logic for MultipleChoiceWindow.xaml
	/// </summary>
	public partial class MultipleChoiceWindow : DataWindow
	{
		#region Fields
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the mutliple choice window where specify is allowed.
		/// </summary>
		/// <param name="choiceCollection">Collection of <see cref="Choice"/> objects to display.</param>
		public MultipleChoiceWindow(IEnumerable<Choice> choiceCollection)
			: this(choiceCollection, true) { }

		/// <summary>
		/// Creates a new instance of the mutliple choice window.
		/// </summary>
		/// <param name="choiceCollection">Collection of <see cref="Choice"/> objects to display.</param>
		/// <param name="allowSpecify">If true, a custom specification is allowed.</param>
		public MultipleChoiceWindow(IEnumerable<Choice> choiceCollection, bool allowSpecify)
		{
			InitializeComponent();

		    ChoiceSpecify = Extensions.Controls.Properties.Resources.ChoiceSpecify;
            Title = Extensions.Controls.Properties.Resources.MultipleChoiceWindowTitle;
			AllowSpecify = allowSpecify;
			ChoiceCollection = new List<Choice>();
			ChoiceCollection.AddRange(choiceCollection);

			if (allowSpecify)
			{
				ChoiceCollection.Add(new Choice(ChoiceSpecify, string.Empty, true));
			}

			UpdateChoices();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets whether a custom specification is allowed.
		/// </summary>
		private bool AllowSpecify { get; set; }

		/// <summary>
		/// Gets or sets the collection of <see cref="Choice"/> objects.
		/// </summary>
		private List<Choice> ChoiceCollection { get; set; }

		/// <summary>
		/// Gets or sets ChoiceSpecify.
		/// </summary>
		/// <remarks>
		/// Wrapper for the ChoiceSpecify dependency property.
		/// </remarks>
		public string ChoiceSpecify
		{
			get { return (string)GetValue(ChoiceSpecifyProperty); }
			set { SetValue(ChoiceSpecifyProperty, value); }
		}

		/// <summary>
		/// DependencyProperty definition as the backing store for ChoiceSpecify.
		/// </summary>
		public static readonly DependencyProperty ChoiceSpecifyProperty = DependencyProperty.Register("ChoiceSpecify", typeof(string),
            typeof(MultipleChoiceWindow), new UIPropertyMetadata(Environment.DefaultMultiLingualDependencyPropertyValue, OnChoiceSpecifyChanged));

		/// <summary>
		/// Gets or sets SelectedChoice.
		/// </summary>
		/// <remarks>
		/// Wrapper for the SelectedChoice dependency property.
		/// </remarks>
		public Choice SelectedChoice
		{
			get { return (Choice)GetValue(SelectedChoiceProperty); }
			set { SetValue(SelectedChoiceProperty, value); }
		}

		/// <summary>
		/// DependencyProperty definition as the backing store for SelectedChoice.
		/// </summary>
		public static readonly DependencyProperty SelectedChoiceProperty = DependencyProperty.Register("SelectedChoice", typeof(Choice),
			typeof(MultipleChoiceWindow), new UIPropertyMetadata(null, OnSelectedChoiceChanged));

		/// <summary>
		/// Gets or sets Choice.
		/// </summary>
		/// <remarks>
		/// Wrapper for the Choice dependency property.
		/// </remarks>
		public string Choice
		{
			get { return (string)GetValue(ChoiceProperty); }
			set { SetValue(ChoiceProperty, value); }
		}

		/// <summary>
		/// DependencyProperty definition as the backing store for Choice.
		/// </summary>
		public static readonly DependencyProperty ChoiceProperty =
			DependencyProperty.Register("Choice", typeof(string), typeof(MultipleChoiceWindow), new UIPropertyMetadata(string.Empty));
		#endregion

		#region Methods
		/// <summary>
		/// Invoked when the ChoiceSpecify dependency property has changed.
		/// </summary>
		/// <param name="sender">The object that contains the dependency property.</param>
		/// <param name="e">The event data.</param>
		private static void OnChoiceSpecifyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MultipleChoiceWindow typedSender = sender as MultipleChoiceWindow;
			if (typedSender != null)
			{
				typedSender.UpdateChoices();
			}
		}

		/// <summary>
		/// Invoked when the SelectedChoice dependency property has changed.
		/// </summary>
		/// <param name="sender">The object that contains the dependency property.</param>
		/// <param name="e">The event data.</param>
		private static void OnSelectedChoiceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			MultipleChoiceWindow typedSender = sender as MultipleChoiceWindow;
			if (typedSender != null)
			{
				Choice newValue = e.NewValue as Choice;
				typedSender.Choice = (newValue != null) ? newValue.Explanation : string.Empty;
			}
		}

		/// <summary>
		/// Updates the available choices.
		/// </summary>
		private void UpdateChoices()
		{
			if (AllowSpecify && (ChoiceCollection.Count > 0))
			{
				ChoiceCollection.RemoveAt(ChoiceCollection.Count - 1);
			}

			if (AllowSpecify)
			{
				Choice specifyChoice = new Choice(ChoiceSpecify, string.Empty, true);
				ChoiceCollection.Add(specifyChoice);

				if (SelectedChoice == null)
				{
					SelectedChoice = specifyChoice;
				}
			}
			
			Resources["ChoiceCollection"] = ChoiceCollection;
		}

		/// <summary>
		/// Validates the data.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		protected override bool ValidateData()
		{
			if (string.IsNullOrEmpty(Choice))
			{
			    return false;
			}
			
			return true;
		}

		/// <summary>
		/// Applies all changes made by this window.
		/// </summary>
		/// <returns>True if successful, otherwise false.</returns>
		protected override bool ApplyChanges()
		{
			return true;
		}

		/// <summary>
		/// Discards all changes made by this window.
		/// </summary>
        /// <returns>True if successful, otherwise false.</returns>
		protected override bool DiscardChanges()
		{
			Choice = string.Empty;

		    return true;
		}
		#endregion
	}

	/// <summary>
	/// Class representing a choice that can be used in the <see cref="MultipleChoiceWindow"/>.
	/// </summary>
	public class Choice
	{
		#region Fields
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new choice where the text displayed is the same
		/// as the value of the choice comment when this choice is chosen.
		/// </summary>
		/// <param name="header">Text to display.</param>
		public Choice(string header)
			: this(header, header) { }

		/// <summary>
		/// Creates a new reason where the text displayed is different
		/// then the value of the reason when this reason is chosen.
		/// </summary>
		/// <param name="header">Text to display.</param>
		/// <param name="explanation">Text to set to reason when this reason is chosen.</param>
		public Choice(string header, string explanation)
			: this(header, explanation, false) { }

		/// <summary>
		/// Creates a new reason where the text displayed is different
		/// then the value of the reason when this reason is chosen.
		/// 
		/// This constructor also allows to specify whether customization is allowed.
		/// </summary>
		/// <param name="header">Text to display.</param>
		/// <param name="explanation">Text to set to reason when this reason is chosen.</param>
		/// <param name="allowCustomization">If true, customization is allowed for this reason.</param>
		internal Choice(string header, string explanation, bool allowCustomization)
		{
			AllowCustomization = allowCustomization;
			Header = header;
			Explanation = explanation;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets whether this reason allows customization.
		/// </summary>
		public bool AllowCustomization { get; private set; }

		/// <summary>
		/// Gets the reason header. This text will be used to display the reason in
		/// the list of available reasons.
		/// </summary>
		public string Header { get; private set; }

		/// <summary>
		/// Gets the explanation of the reason. This text will be used to
		/// fill the reason text when this reason is chosen.
		/// </summary>
		public string Explanation { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }
		#endregion

		#region Methods
		#endregion
	}
}
