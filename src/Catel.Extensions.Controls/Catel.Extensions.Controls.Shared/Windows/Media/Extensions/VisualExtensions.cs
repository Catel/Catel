// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows.Media
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Extensions for <see cref="System.Windows.Media.Visual"/>.
    /// </summary>
    public static class VisualExtensions
    {
        #region Methods
        /// <summary>
        /// Get the parent window for this visual object or null when not exists.
        /// </summary>
        /// <param name="visualObject">Reference to visual object.</param>
        /// <returns>Reference to partent window or null when not exists.</returns>
        public static Window GetParentWindow(this Visual visualObject)
        {
            if (visualObject == null)
            {
                return null;
            }

            return visualObject.GetAncestorObject<Window>();
        }
        #endregion
    }
}

#endif