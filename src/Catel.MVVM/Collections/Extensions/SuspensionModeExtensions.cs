// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuspensionModeExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Collections
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The suspension mode extensions.
    /// </summary>
    public static class SuspensionModeExtensions
    {
        #region Fields
        /// <summary>
        /// The mixed modes.
        /// </summary>
        private static readonly List<SuspensionMode> MixedModes = Enum<SuspensionMode>.GetValues().Where(mode => Enum<SuspensionMode>.ToString(mode).ToString().ContainsIgnoreCase("Mixed")).ToList();
        #endregion Fields

        #region Methods
        /// <summary>
        /// The is mixed mode.
        /// </summary>
        /// <param name="suspensionMode">The suspension Mode.</param>
        /// <returns><c>True</c> if <see cref="SuspensionMode"/> is one of the mixed modes; otherwise, <c>false</c>.</returns>
        public static bool IsMixedMode(this SuspensionMode suspensionMode)
        {
            return MixedModes.Contains(suspensionMode);
        }
        #endregion Methods
    }
}
