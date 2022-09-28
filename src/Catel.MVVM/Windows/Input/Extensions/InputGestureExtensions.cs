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
            if (inputGesture is null)
            {
                return true;
            }

            if (inputGesture.Key != Key.None)
            {
                return false;
            }

            if (inputGesture.Modifiers != ModifierKeys.None)
            {
                return false;
            }

            return true;
        }
    }
}
