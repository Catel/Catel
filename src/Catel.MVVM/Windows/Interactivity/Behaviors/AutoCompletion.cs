// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoCompleteBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE || UWP

namespace Catel.Windows.Interactivity
{
    using System.Collections.Generic;
    using System.Collections;
    using Catel.Services;
    using IoC;
    using Input;

#if UWP
    using global::Windows.Foundation;
    using global::Windows.UI;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Input;
    using global::Windows.UI.Xaml.Media;

    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
    using Key = global::Windows.System.VirtualKey;
    using ModifierKeys = global::Windows.System.VirtualKeyModifiers;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
#endif

    /// <summary>
    /// Auto complete behavior to support auto complete on a <c>TextBox</c> control.
    /// </summary>
    public class AutoCompletion : BehaviorBase<TextBox>
    {
        #region Fields
        private readonly IAutoCompletionService _autoCompletionService;
        private readonly ListBox _suggestionListBox;
        private readonly Popup _popup;

        private bool _isUpdatingAssociatedObject;

        private bool _subscribed;
        private string _valueAtSuggestionBoxOpen;
        private string[] _availableSuggestions;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompletion"/> class.
        /// </summary>
        public AutoCompletion()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _autoCompletionService = dependencyResolver.Resolve<IAutoCompletionService>();

            _suggestionListBox = new ListBox();
            _suggestionListBox.Margin = new Thickness(0d);

#if UWP
            _suggestionListBox.Background = new SolidColorBrush(Colors.Gainsboro);
#endif

            _popup = new Popup();
            _popup.Child = _suggestionListBox;

#if NET || NETCORE
            _popup.StaysOpen = false;
#elif UWP
            _popup.IsLightDismissEnabled = true;
#else
            // To determine
#endif
        }
        #endregion

        #region Properties
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
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string),
            typeof(AutoCompletion), new PropertyMetadata(string.Empty, (sender, e) => ((AutoCompletion)sender).OnPropertyNameChanged()));

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
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
            typeof(AutoCompletion), new PropertyMetadata(null, (sender, e) => ((AutoCompletion)sender).OnItemsSourceChanged()));



        /// <summary>
        /// Gets or sets a value indicating whether this behavior should use the auto completion service to filter the items source.
        /// <para />
        /// If this value is set to <c>false</c>, it will show the <see cref="ItemsSource"/> as auto completion source without filtering.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if this behavior should use the auto completion service; otherwise, <c>false</c>.</value>
        public bool UseAutoCompletionService
        {
            get { return (bool)GetValue(UseAutoCompletionServiceProperty); }
            set { SetValue(UseAutoCompletionServiceProperty, value); }
        }

        /// <summary>
        /// The use automatic completion service property.
        /// </summary>
        public static readonly DependencyProperty UseAutoCompletionServiceProperty =
            DependencyProperty.Register(nameof(UseAutoCompletionService), typeof(bool), typeof(AutoCompletion), new PropertyMetadata(true));
        #endregion

        #region Methods
        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            SubscribeEvents();
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            UnsubscribeEvents();

            base.OnAssociatedObjectUnloaded();
        }

        private void SubscribeEvents()
        {
            if (!IsEnabled || _subscribed)
            {
                return;
            }

            var associatedObject = AssociatedObject;
            if (associatedObject is not null)
            {
                _subscribed = true;

                associatedObject.TextChanged += OnTextChanged;

#if NET || NETCORE
                associatedObject.PreviewKeyDown += OnPreviewKeyDown;
#else
                associatedObject.KeyDown += OnPreviewKeyDown;
#endif

                _suggestionListBox.SelectionChanged += OnSuggestionListBoxSelectionChanged;

#if UWP
                _suggestionListBox.Tapped += OnSuggestionListBoxTapped;
#else
                _suggestionListBox.MouseLeftButtonUp += OnSuggestionListBoxMouseLeftButtonUp;
#endif

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
            if (associatedObject is not null)
            {
                associatedObject.TextChanged -= OnTextChanged;

#if NET || NETCORE
                associatedObject.PreviewKeyDown -= OnPreviewKeyDown;
#else
                associatedObject.KeyDown -= OnPreviewKeyDown;
#endif

                _suggestionListBox.SelectionChanged -= OnSuggestionListBoxSelectionChanged;

#if UWP
                _suggestionListBox.Tapped -= OnSuggestionListBoxTapped;
#else
                _suggestionListBox.MouseLeftButtonUp -= OnSuggestionListBoxMouseLeftButtonUp;
#endif

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

#if NET || NETCORE
            _popup.PlacementTarget = textBox;
            _popup.Placement = PlacementMode.Bottom;
#elif UWP
            var offset = CalculateOffset(AssociatedObject, new Point(0, 0));

            _popup.HorizontalOffset = offset.X;
            _popup.VerticalOffset = offset.Y;
            _popup.Width = AssociatedObject.ActualWidth;

#else
            // TODO: Determine
#endif

            _popup.IsOpen = isVisible;

#if UWP
            AssociatedObject.Focus(FocusState.Programmatic);
#endif
        }

#if UWP
        private static Point CalculateOffset(FrameworkElement element, Point offset)
        {
            var transform = element.TransformToVisual(Window.Current.Content);
            var point = transform.TransformPoint(offset);

            point.Y += element.ActualHeight;

            return point;
        }
#endif

        private void UpdateSuggestions()
        {
            if (!IsEnabled)
            {
                return;
            }

            var associatedObject = AssociatedObject;
            if (associatedObject is null)
            {
                return;
            }

            var text = AssociatedObject.Text;
            string[] availableSuggestions = null;

            if (ItemsSource is not null)
            {
                if (UseAutoCompletionService)
                {
                    availableSuggestions = _autoCompletionService.GetAutoCompleteValues(PropertyName, text, ItemsSource);
                }
                else
                {
                    var items = new List<string>();
                    foreach (var item in ItemsSource)
                    {
                        var itemAsString = item as string;
                        if (!string.IsNullOrWhiteSpace(itemAsString))
                        {
                            items.Add(itemAsString);
                        }
                    }

                    availableSuggestions = items.ToArray();
                }
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

        /// <summary>
        /// Called when the is enabled property has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
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
            if (_isUpdatingAssociatedObject)
            {
                _isUpdatingAssociatedObject = false;
                return;
            }

            UpdateSuggestions();

            if (_availableSuggestions is null || _availableSuggestions.Length == 0)
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
                if (binding is not null)
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

                    _isUpdatingAssociatedObject = true;

                    AssociatedObject.Text = _valueAtSuggestionBoxOpen;
                    e.Handled = true;
                }
            }
        }

#if UWP
        private void OnSuggestionListBoxTapped(object sender, TappedRoutedEventArgs e)
        {
            UpdateSuggestionBox(false);
        }
#else
        private void OnSuggestionListBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateSuggestionBox(false);
        }
#endif

        private void OnSuggestionListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var textBox = AssociatedObject;

            if (_suggestionListBox.ItemsSource is not null)
            {
                textBox.TextChanged -= OnTextChanged;

                if (_suggestionListBox.SelectedIndex != -1)
                {
                    _isUpdatingAssociatedObject = true;

                    textBox.Text = _suggestionListBox.SelectedItem.ToString();
                }

                textBox.TextChanged += OnTextChanged;
            }
        }
        #endregion
    }
}

#endif
