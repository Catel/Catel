// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkLabelClickBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Controls
{
    /// <summary>
    /// Available <see cref="LinkLabel"/> clickevent behaviors.
    /// </summary>
    [ObsoleteEx(Replacement = "Orc.Controls, see https://github.com/wildgums/orc.controls", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public enum LinkLabelClickBehavior
    {
        /// <summary>
        /// No explicit behavior defined, will use the set-click-event.
        /// </summary>
        Undefined,

        /// <summary>
        /// Opens the set url in the systems webbrowser.
        /// </summary>
        OpenUrlInBrowser
    }
}

#endif