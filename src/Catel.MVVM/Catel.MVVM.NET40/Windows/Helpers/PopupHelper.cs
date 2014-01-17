// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interop;

    /// <summary>
    /// Popup helper class to retrieve all the popups inside a windows application, which are not available
    /// via the regular <see cref="Application.Windows"/> property.
    /// </summary>
    public static class PopupHelper
    {
        /// <summary>
        /// Gets all the popups of the current application.
        /// </summary>
        /// <value>The popups.</value>
        public static IEnumerable<Popup> Popups { get { return GetAllPopups(); } }

        /// <summary>
        /// Gets all the popups of the current application.
        /// </summary>
        /// <returns>The popups.</returns>
        public static IEnumerable<Popup> GetAllPopups()
        {
            return FindAllPopups();
        }

        /// <summary>
        /// Finds all the popups of the current application.
        /// </summary>
        private static IEnumerable<Popup> FindAllPopups()
        {
            var popups = new List<Popup>();
            var sources = PresentationSource.CurrentSources.OfType<HwndSource>();

            foreach (var hwndSource in sources)
            {
                if (hwndSource != null)
                {
                    var popupRoot = hwndSource.RootVisual as FrameworkElement;
                    if (popupRoot != null)
                    {
                        var popup = popupRoot.Parent as Popup;
                        if (popup != null)
                        {
                            popups.Add(popup);
                        }
                    }
                }
            }

            return popups;
        }
    }
}
