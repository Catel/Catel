namespace Catel.Windows.Input
{
    using System.Collections.Generic;
    using System;
    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;

    /// <summary>
    /// Helper class for the keyboard.
    /// </summary>
    public static class KeyboardHelper
    {
        /// <summary>
        /// Determines whether the specified keyboard modifiers are currently pressed.
        /// </summary>
        /// <param name="modifier">One or more keyboard modifiers.</param>
        /// <param name="checkForExactModifiers">if set to <c>true</c>, this check requires the exact modifiers to be pressed.</param>
        /// <returns><c>true</c> if all the specified keyboard modifiers are being pressed; otherwise, <c>false</c>.</returns>
        public static bool AreKeyboardModifiersPressed(ModifierKeys modifier, bool checkForExactModifiers = true)
        {
            var allModifiers = Enum<ModifierKeys>.GetValues();
            allModifiers.Remove(ModifierKeys.None);

            var currentlyPressedModifiers = GetCurrentlyPressedModifiers();

            if (checkForExactModifiers)
            {
                if (modifier == ModifierKeys.None)
                {
                    if (currentlyPressedModifiers.Count > 0)
                    {
                        if (currentlyPressedModifiers[0] != ModifierKeys.None)
                        {
                            return false;
                        }
                    }
                }
            }

            if (checkForExactModifiers)
            {
                foreach (var currentlyPressedModifier in currentlyPressedModifiers)
                {
                    if (!Enum<ModifierKeys>.Flags.IsFlagSet(modifier, currentlyPressedModifier))
                    {
                        // Pressed but not expected
                        return false;
                    }
                }
            }

            foreach (var modifierToCheck in allModifiers)
            {
                if (Enum<ModifierKeys>.Flags.IsFlagSet(modifier, modifierToCheck))
                {
                    // Key is expected, checking if it is pressed
                    if (!currentlyPressedModifiers.Contains(modifierToCheck))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the currently pressed modifiers.
        /// </summary>
        /// <returns>List of currently pressed modifiers.</returns>
        public static List<ModifierKeys> GetCurrentlyPressedModifiers()
        {
            var modifiers = new List<ModifierKeys>();

            var allModifiers = Enum<ModifierKeys>.GetValues();
            var keyboardModifiers = Keyboard.Modifiers;

            foreach (var modifier in allModifiers)
            {
                if (Enum<ModifierKeys>.Flags.IsFlagSet(keyboardModifiers, modifier))
                {
                    modifiers.Add(modifier);
                }
            }

            // #1606: always remove None, then we re-add it if there is nothing else
            if (modifiers.Count > 0)
            {
                modifiers.Remove(ModifierKeys.None);
            }

            if (modifiers.Count == 0)
            {
                modifiers.Add(ModifierKeys.None);
            }

            return modifiers;
        }
    }
}
