// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Input
{
    using System.Collections.Generic;
    using System;

#if NETFX_CORE
    using global::Windows.UI.Core;
    using global::Windows.System;
    using ModifierKeys = global::Windows.System.VirtualKeyModifiers;
#else
    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
#endif

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

#if NETFX_CORE
            var coreWindow = CoreWindow.GetForCurrentThread();

            if ((coreWindow.GetKeyState(VirtualKey.Control) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.LeftControl) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.RightControl) != CoreVirtualKeyStates.None))
            {
                modifiers.Add(ModifierKeys.Control);
            }

            //if ((coreWindow.GetKeyState(VirtualKey.Alt) != CoreVirtualKeyStates.None) ||
            //    (coreWindow.GetKeyState(VirtualKey.LeftAlt) != CoreVirtualKeyStates.None) ||
            //    (coreWindow.GetKeyState(VirtualKey.RightAlt) != CoreVirtualKeyStates.None))
            //{
            //    modifiers.Add(ModifierKeys.Alt);
            //}

            if ((coreWindow.GetKeyState(VirtualKey.Menu) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.LeftMenu) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.RightMenu) != CoreVirtualKeyStates.None))
            {
                modifiers.Add(ModifierKeys.Menu);
            }

            if ((coreWindow.GetKeyState(VirtualKey.Shift) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.LeftShift) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.RightShift) != CoreVirtualKeyStates.None))
            {
                modifiers.Add(ModifierKeys.Shift);
            }

            if ((coreWindow.GetKeyState(VirtualKey.LeftWindows) != CoreVirtualKeyStates.None) ||
                (coreWindow.GetKeyState(VirtualKey.RightWindows) != CoreVirtualKeyStates.None))
            {
                modifiers.Add(ModifierKeys.Windows);
            }
#else
            var allModifiers = Enum<ModifierKeys>.GetValues();
            var keyboardModifiers = Keyboard.Modifiers;

            foreach (var modifier in allModifiers)
            {
                if (Enum<ModifierKeys>.Flags.IsFlagSet(keyboardModifiers, modifier))
                {
                    modifiers.Add(modifier);
                }
            }
#endif

            if (modifiers.Count == 0)
            {
                modifiers.Add(ModifierKeys.None);
            }

            return modifiers;
        }
    }
}

#endif