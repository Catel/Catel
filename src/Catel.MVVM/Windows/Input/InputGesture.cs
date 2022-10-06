namespace Catel.Windows.Input
{
    using Catel.Data;
    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;

    /// <summary>
    /// Input gesture class.
    /// </summary>
    public class InputGesture : ModelBase
    {
        /// <summary>
        /// <see cref="ToString"/> method result cache.
        /// </summary>
        private string? _string;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGesture"/> class.
        /// </summary>
        public InputGesture()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGesture" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public InputGesture(Key key)
            : this(key, ModifierKeys.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputGesture" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="modifiers">The modifiers.</param>
        public InputGesture(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public Key Key
        {
            get { return GetValue<Key>(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        /// Register the Key property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData KeyProperty = RegisterProperty<InputGesture, Key>(o => o.Key, propertyChangedEventHandler: (o, e) => o.OnInputGesturePropertyChanged());

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        public ModifierKeys Modifiers
        {
            get { return GetValue<ModifierKeys>(ModifiersProperty); }
            set { SetValue(ModifiersProperty, value); }
        }

        /// <summary>
        /// Register the Modifiers property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ModifiersProperty = RegisterProperty<InputGesture, ModifierKeys>(o => o.Modifiers, propertyChangedEventHandler: (o, e) => o.OnInputGesturePropertyChanged());

        /// <summary>
        /// Called whether <see cref="Modifiers"/> or <see cref="Key"/> properties changed.
        /// </summary>
        private void OnInputGesturePropertyChanged()
        {
            _string = null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            // Keep this check to prevent == comparison troubles on the same object
            if (!(obj is InputGesture))
            {
                return false;
            }

            return Equals((InputGesture)obj);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected bool Equals(InputGesture other)
        {
            if (other is null)
            {
                return false;
            }

            if (Key != other.Key)
            {
                return false;
            }

            if (Modifiers != other.Modifiers)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Checks whether this input gesture matches the specified event args.
        /// </summary>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <returns><c>true</c> if this gesture matches the event args, <c>false</c> otherwise.</returns>
        public bool Matches(KeyEventArgs eventArgs)
        {
            if (eventArgs is null)
            {
                return false;
            }

            if (eventArgs.Key != Key)
            {
                return false;
            }

            if (!KeyboardHelper.AreKeyboardModifiersPressed(Modifiers))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (_string is null)
            {
                var format = string.Empty;

                if (Modifiers != ModifierKeys.None)
                {
                    foreach (var modifier in Enum<ModifierKeys>.GetValues())
                    {
                        if (Enum<ModifierKeys>.Flags.IsFlagSet(Modifiers, modifier) && modifier != ModifierKeys.None)
                        {
                            format += $"{Enum<ModifierKeys>.ToString(modifier)} + ";
                        }
                    }
                }

                _string = format + Enum<Key>.ToString(Key);
            }

            return _string;
        }
    }
}
