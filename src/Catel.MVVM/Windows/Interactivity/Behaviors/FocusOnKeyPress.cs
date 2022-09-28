namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Xaml.Behaviors;
    using KeyDownEventArgs = System.Windows.Input.KeyEventArgs;
    using Input;
    using Logging;
    using Reflection;

    /// <summary>
    /// Behavior to set the focus on a key press.
    /// </summary>
    public class FocusOnKeyPress : FocusBehaviorBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private FrameworkElement _layoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusOnKeyPress"/> class.
        /// </summary>
        public FocusOnKeyPress()
        {
            
        }

        /// <summary>
        /// Gets or sets the modifiers to check for.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers
        {
            get { return (ModifierKeys)GetValue(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Modifiers.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register(nameof(Modifiers), typeof(ModifierKeys), 
            typeof(FocusOnKeyPress), new PropertyMetadata(ModifierKeys.None));

        /// <summary>
        /// Gets or sets the key to which the behavior should respond.
        /// </summary>
        /// <value>The key.</value>
        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(nameof(Key), typeof(Key), 
            typeof(FocusOnKeyPress), new PropertyMetadata(Key.None));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            Subscribe();
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            Unsubscribe();

            base.OnAssociatedObjectUnloaded();
        }

        private void Subscribe()
        {
            Unsubscribe();

            _layoutRoot = AssociatedObject.FindLogicalRoot() as FrameworkElement;
            if (_layoutRoot is not null)
            {
                Log.Debug("Found layout root '{0}', subscribing to KeyDown event", _layoutRoot.GetType().GetSafeFullName(false));

                _layoutRoot.KeyDown += OnKeyDown;
            }
        }

        private void Unsubscribe()
        {
            if (_layoutRoot is not null)
            {
                _layoutRoot.KeyDown -= OnKeyDown;
                _layoutRoot = null;
            }
        }

        /// <summary>
        /// Called when the specified key is pressed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The key event args instance containing the event data.</param>
        private void OnKeyDown(object sender, KeyDownEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (e.Handled)
            {
                return;
            }

            if (KeyboardHelper.AreKeyboardModifiersPressed(Modifiers))
            {
                if (e.Key == Key)
                {
                    StartFocus();

                    e.Handled = true;
                }
            }
        }
    }
}

