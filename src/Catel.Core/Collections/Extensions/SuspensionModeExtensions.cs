namespace Catel.Collections
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The suspension mode extensions.
    /// </summary>
    public static class SuspensionModeExtensions
    {
        /// <summary>
        /// The mixed modes.
        /// </summary>
        private static readonly List<SuspensionMode> MixedModes = Enum<SuspensionMode>.GetValues().Where(mode => Enum<SuspensionMode>.ToString(mode).ToString().ContainsIgnoreCase("Mixed")).ToList();

        /// <summary>
        /// The is mixed mode.
        /// </summary>
        /// <param name="suspensionMode">The suspension Mode.</param>
        /// <returns><c>True</c> if <see cref="SuspensionMode"/> is one of the mixed modes; otherwise, <c>false</c>.</returns>
        public static bool IsMixedMode(this SuspensionMode suspensionMode)
        {
            return MixedModes.Contains(suspensionMode);
        }
    }
}
