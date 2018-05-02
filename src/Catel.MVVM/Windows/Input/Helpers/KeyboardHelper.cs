// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

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

#if NETFX_CORE
            var coreWindow = CoreWindow.GetForCurrentThread();

            // Control
            var controlState = coreWindow.GetKeyState(VirtualKey.Control);
            var leftControlState = coreWindow.GetKeyState(VirtualKey.LeftControl);
            var rightControlState = coreWindow.GetKeyState(VirtualKey.RightControl);
            if (Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(controlState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(leftControlState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(rightControlState, CoreVirtualKeyStates.Down))
            {
                modifiers.Add(ModifierKeys.Control);
            }

            // Menu == Alt key
            var menuState = coreWindow.GetKeyState(VirtualKey.Menu);
            var leftMenuState = coreWindow.GetKeyState(VirtualKey.LeftMenu);
            var rightMenuState = coreWindow.GetKeyState(VirtualKey.RightMenu);
            if (Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(menuState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(leftMenuState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(rightMenuState, CoreVirtualKeyStates.Down))
            {
                modifiers.Add(ModifierKeys.Menu);
            }

            // Shift
            var shiftState =coreWindow.GetKeyState(VirtualKey.Shift);
            var leftShiftState =coreWindow.GetKeyState(VirtualKey.LeftShift);
            var rightShiftState =coreWindow.GetKeyState(VirtualKey.RightShift);
            if (Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(shiftState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(leftShiftState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(rightShiftState, CoreVirtualKeyStates.Down))
            {
                modifiers.Add(ModifierKeys.Shift);
            }

            // Windows
            var leftWindowsState = coreWindow.GetKeyState(VirtualKey.LeftWindows);
            var rightWindowsState = coreWindow.GetKeyState(VirtualKey.LeftWindows);
            if (Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(leftWindowsState, CoreVirtualKeyStates.Down) ||
                Enum<CoreVirtualKeyStates>.Flags.IsFlagSet(rightWindowsState, CoreVirtualKeyStates.Down))
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