// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoCompleteBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Catel.IoC;
    using Catel.Services;
    using Catel.Windows.Input;

    /// <summary>
    /// Auto complete behavior to support auto complete on a <c>TextBox</c> control.
    /// </summary>
    public class AutoCompletionBehavior : BehaviorBase<TextBox>
    {
        #region Fields
        private readonly IAutoCompletionService _autoCompletionService;
        private readonly ListBox _suggestionListBox;
        private readonly Popup _popup;

        private bool _subscribed;
        private string _valueAtSuggestionBoxOpen;
        private string[] _availableSuggestions;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletionBehavior"/> class.
        /// </summary>
        public AutoCompletionBehavior()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _autoCompletionService = dependencyResolver.Resolve<IAutoCompletionService>();

            _suggestionListBox = new ListBox();

            _popup = new Popup();
            _popup.Child = _suggestionListBox;

#if NET
            _popup.StaysOpen = false;
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the auto completion functionality is enabled.
        /// </summary>
        /// <value>The is enabled.</value>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// The is enabled property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool),
            typeof(AutoCompletionBehavior), new PropertyMetadata(true, (sender, e) => ((AutoCompletionBehavior)sender).OnIsEnabledChanged()));

        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        /// <summary>
        /// The property name property.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string),
            typeof(AutoCompletionBehavior), new PropertyMetadata(string.Empty, (sender, e) => ((AutoCompletionBehavior)sender).OnPropertyNameChanged()));

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// The items source property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",  typeof(IEnumerable), 
            typeof(AutoCompletionBehavior), new PropertyMetadata((sender, e) => ((AutoCompletionBehavior)sender).OnItemsSourceChanged()));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            base.OnAssociatedObjectLoaded(sender, e);

            SubscribeEvents();
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
            UnsubscribeEvents();

            base.OnAssociatedObjectUnloaded(sender, e);
        }

        private void SubscribeEvents()
        {
            if (!IsEnabled || _subscribed)
            {
                return;
            }

            var associatedObject = AssociatedObject;
            if (associatedObject != null)
            {
                _subscribed = true;

                associatedObject.TextChanged += OnTextChanged;

#if NET
                associatedObject.PreviewKeyDown += OnPreviewKeyDown;
#else
                associatedObject.KeyDown += OnPreviewKeyDown;
#endif

                _suggestionListBox.SelectionChanged += OnSuggestionListBoxSelectionChanged;
                _suggestionListBox.MouseLeftButtonUp += OnSuggestionListBoxMouseLeftButtonUp;

                UpdateSuggestionBox(false);
            }
        }

        private void UnsubscribeEvents()
        {
            if (!_subscribed)
            {
                return;
            }

            var associatedObject = AssociatedObject;
            if (associatedObject != null)
            {
                associatedObject.TextChanged -= OnTextChanged;

#if NET
                associatedObject.PreviewKeyDown -= OnPreviewKeyDown;
#else
                associatedObject.KeyDown -= OnPreviewKeyDown;
#endif

                _suggestionListBox.SelectionChanged -= OnSuggestionListBoxSelectionChanged;
                _suggestionListBox.MouseLeftButtonUp -= OnSuggestionListBoxMouseLeftButtonUp;

                _subscribed = false;
            }
        }

        private void UpdateSuggestionBox(bool isVisible)
        {
            var textBox = AssociatedObject;

            if (isVisible && !_popup.IsOpen)
            {
                _valueAtSuggestionBoxOpen = textBox.Text;
            }

            _popup.Width = textBox.ActualWidth;

#if NET
            _popup.PlacementTarget = textBox;
            _popup.Placement = PlacementMode.Bottom;
#else
            // TODO: Support silverlight
#endif

            _popup.IsOpen = isVisible;
        }

        private void UpdateSuggestions()
        {
            if (!IsEnabled)
            {
                return;
            }

            var associatedObject = AssociatedObject;
            if (associatedObject == null)
            {
                return;
            }

            var text = AssociatedObject.Text;
            string[] availableSuggestions = null;

            if (!string.IsNullOrWhiteSpace(PropertyName) && (ItemsSource != null))
            {
                availableSuggestions = _autoCompletionService.GetAutoCompleteValues(PropertyName, text, ItemsSource);
            }

            _availableSuggestions = availableSuggestions;
            _suggestionListBox.ItemsSource = _availableSuggestions;
        }

        private void OnPropertyNameChanged()
        {
            UpdateSuggestions();
        }

        private void OnItemsSourceChanged()
        {
            UpdateSuggestions();
        }

        private void OnIsEnabledChanged()
        {
            if (IsEnabled)
            {
                SubscribeEvents();
            }
            else
            {
                UnsubscribeEvents();
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSuggestions();

            if (_availableSuggestions == null || _availableSuggestions.Length == 0)
            {
                UpdateSuggestionBox(false);
            }
            else
            {
                UpdateSuggestionBox(true);
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (KeyboardHelper.AreKeyboardModifiersPressed(ModifierKeys.Control))
                {
                    UpdateSuggestionBox(true);
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Down)
            {
                if (_suggestionListBox.SelectedIndex < _suggestionListBox.Items.Count)
                {
                    _suggestionListBox.SelectedIndex = _suggestionListBox.SelectedIndex + 1;
                }
            }

            if (e.Key == Key.Up)
            {
                if (_suggestionListBox.SelectedIndex > -1)
                {
                    _suggestionListBox.SelectedIndex = _suggestionListBox.SelectedIndex - 1;
                }
            }

            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                // Commit the selection
                UpdateSuggestionBox(false);
                e.Handled = (e.Key == Key.Enter);

                var binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
            }

            if (e.Key == Key.Escape)
            {
                if (_popup.IsOpen)
                {
                    // Cancel the selection
                    UpdateSuggestionBox(false);

                    AssociatedObject.Text = _valueAtSuggestionBoxOpen;
                    e.Handled = true;
                }
            }
        }

        private void OnSuggestionListBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateSuggestionBox(false);
        }

        private void OnSuggestionListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var textBox = AssociatedObject;

            if (_suggestionListBox.ItemsSource != null)
            {
                textBox.TextChanged -= OnTextChanged;

                if (_suggestionListBox.SelectedIndex != -1)
                {
                    textBox.Text = _suggestionListBox.SelectedItem.ToString();
                }

                textBox.TextChanged += OnTextChanged;
            }
        }
        #endregion
    }
}