// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkLabelBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    /// <summary>
    /// Available <see cref="LinkLabel"/> behaviors.
    /// </summary>
    [ObsoleteEx(Replacement = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public enum LinkLabelBehavior
    {
        /// <summary>
        /// Default.
        /// </summary>
        SystemDefault,

        /// <summary>
        /// Always underline.
        /// </summary>
        AlwaysUnderline,

        /// <summary>
        /// Hover underline.
        /// </summary>
        HoverUnderline,

        /// <summary>
        /// Never underline.
        /// </summary>
        NeverUnderline
    }
}

#endif