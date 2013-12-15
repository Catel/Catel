// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGesture.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Windows.Input
{
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
    public class InputGesture
    {
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
        /// <value>The key.</value>
        public Key Key { get; private set; }

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public ModifierKeys Modifiers { get; private set; }

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