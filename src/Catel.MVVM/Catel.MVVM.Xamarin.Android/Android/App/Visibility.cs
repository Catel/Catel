// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Visibility.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace System.Windows
{
    using Android.Views;

    /// <summary>
    /// Specifies the display state of an element.
    /// </summary>
    public enum Visibility : byte
    {
        /// <summary>
        /// Visible on screen; the default value.
        /// </summary>
        Visible = ViewStates.Visible,

        /// <summary>
        /// Not displayed, but taken into account during layout (space is left for it).
        /// </summary>
        Hidden = ViewStates.Invisible,

        /// <summary>
        /// Completely hidden, as if the view had not been added.
        /// </summary>
        Collapsed = ViewStates.Gone
    }
}