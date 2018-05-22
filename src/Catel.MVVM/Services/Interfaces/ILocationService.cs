// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface that supports retrieving the current location.
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Occurs when the current location has changed.
        /// </summary>
        event EventHandler<LocationChangedEventArgs> LocationChanged;

        /// <summary>
        /// Gets the current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </summary>
        /// <value>The current location.</value>
        /// <remarks>
        /// This is convenience property that internally calls <see cref="GetCurrentLocation"/>.
        /// <para />
        /// Note that the services inside Catel do not support <see cref="INotifyPropertyChanged"/>, thus you cannot 
        /// subscribe to changes of this property. Instead, subscribe to the <see cref="LocationChanged"/> event.
        /// </remarks>
        ILocation CurrentLocation { get; }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        ILocation GetCurrentLocation();

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        bool Start();

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        void Stop();
    }
}
