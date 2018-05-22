// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGestureExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Input
{
    using System.Windows.Input;
    using Catel;

    /// <summary>
    /// Extension methods for the <see cref="Catel.Windows.Input.InputGesture"/>.
    /// </summary>
    public static class InputGestureExtensions
    {
        /// <summary>
        /// Determines whether the specified input gesture is empty.
        /// </summary>
        /// <param name="inputGesture">The input gesture.</param>
        /// <returns><c>true</c> if the specified input gesture is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(this Catel.Windows.Input.InputGesture inputGesture)
        {
            if (inputGesture == null)
            {
                return true;
            }

#if NET
            if (inputGesture.Key != Key.None)
            {
                return false;
            }

            if (inputGesture.Modifiers != ModifierKeys.None)
            {
                return false;
            }
#endif

            return true;
        }
    }
}

#endif