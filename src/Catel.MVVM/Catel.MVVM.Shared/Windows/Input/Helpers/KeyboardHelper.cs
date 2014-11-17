// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Input
{
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
        /// <returns><c>true</c> if all the specified keyboard modifiers are being pressed; otherwise, <c>false</c>.</returns>
        public static bool AreKeyboardModifiersPressed(ModifierKeys modifier)
        {
            var keys = Enum<ModifierKeys>.GetValues();
            foreach (var key in keys)
            {
                if (Enum<ModifierKeys>.Flags.IsFlagSet(modifier, key))
                {
#if NETFX_CORE
                    var coreWindow = CoreWindow.GetForCurrentThread();

                    switch (key)
                    {
                        case ModifierKeys.None:
                            break;

                        case ModifierKeys.Control:
                            if ((coreWindow.GetKeyState(VirtualKey.Control) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.LeftControl) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.RightControl) == CoreVirtualKeyStates.None))
                            {
                                return false;
                            }
                            break;

                        case ModifierKeys.Menu:
                            if ((coreWindow.GetKeyState(VirtualKey.Menu) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.LeftMenu) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.RightMenu) == CoreVirtualKeyStates.None))
                            {
                                return false;
                            }
                            break;

                        case ModifierKeys.Shift:
                            if ((coreWindow.GetKeyState(VirtualKey.Shift) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.LeftShift) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.RightShift) == CoreVirtualKeyStates.None))
                            {
                                return false;
                            }
                            break;

                        case ModifierKeys.Windows:
                            if ((coreWindow.GetKeyState(VirtualKey.LeftWindows) == CoreVirtualKeyStates.None) &&
                                (coreWindow.GetKeyState(VirtualKey.RightWindows) == CoreVirtualKeyStates.None))
                            {
                                return false;
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
#else
                    if (!Enum<ModifierKeys>.Flags.IsFlagSet(Keyboard.Modifiers, key))
                    {
                        return false;
                    }
#endif
                }
            }

            return true;
        }
    }
}

#endif