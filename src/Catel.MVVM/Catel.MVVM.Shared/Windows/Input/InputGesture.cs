// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGesture.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Input
{
    using Catel.Data;

#if NETFX_CORE
    using ModifierKeys = global::Windows.System.VirtualKeyModifiers;
    using Key = global::Windows.System.VirtualKey;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#else
    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
#endif

    /// <summary>
    /// Input gesture class.
    /// </summary>
    public class InputGesture : ModelBase
    {
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
        public static readonly PropertyData KeyProperty = RegisterProperty("Key", typeof(Key));

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
        public static readonly PropertyData ModifiersProperty = RegisterProperty("Modifiers", typeof(ModifierKeys));

        /// <summary>
        /// Checks whether this input gesture matches the specified event args.
        /// </summary>
        /// <param name="eventArgs">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <returns><c>true</c> if this gesture matches the event args, <c>false</c> otherwise.</returns>
        public bool Matches(KeyEventArgs eventArgs)
        {
            if (eventArgs == null)
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
            var format = string.Empty;

            if (Modifiers != ModifierKeys.None)
            {
                foreach (var modifier in Enum<ModifierKeys>.GetValues())
                {
                    if (Enum<ModifierKeys>.Flags.IsFlagSet(Modifiers, modifier) && modifier != ModifierKeys.None)
                    {
                        format += string.Format("{0} + ", modifier);
                    }
                }
            }

            return format + Key;
        }
    }
}

#endif