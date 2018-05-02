// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation which contains a location.
    /// </summary>
    public class LocationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newLocation">The new location.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="newLocation"/> is <c>null</c>.</exception>
        public LocationChangedEventArgs(ILocation newLocation)
        {
            Location = newLocation;
        }

        /// <summary>
        /// Gets the new location.
        /// </summary>
        /// <value>The new location.</value>
        public ILocation Location { get; private set; }
    }
}