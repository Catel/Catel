// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using Catel.Logging;

    /// <summary>
    /// Service that supports retrieving the current location.
    /// </summary>
    public class LocationService : ViewModelServiceBase, ILocationService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly GeoCoordinateWatcher _geoCoordinateWatcher;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        public LocationService()
        {
            _geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            _geoCoordinateWatcher.MovementThreshold = 0;

            _geoCoordinateWatcher.PositionChanged += OnGeoCoordinateWatcherPositionChanged;
            _geoCoordinateWatcher.StatusChanged += OnGeoCoordinateWatcherStatusChanged;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </summary>
        /// <value>The current location.</value>
        /// <remarks>
        /// This is convenience property that internally calls <see cref="GetCurrentLocation"/>.
        /// <para/>
        /// Note that the services inside Catel do not support <see cref="INotifyPropertyChanged"/>, thus you cannot
        /// subscribe to changes of this property. Instead, subscribe to the <see cref="LocationChanged"/> event.
        /// </remarks>
        public ILocation CurrentLocation { get { return GetCurrentLocation(); } }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the current location has changed.
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Called when the position has changed, according to the <see cref="GeoCoordinateWatcher"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Device.Location.GeoPositionChangedEventArgs{T}"/> instance containing the event data.</param>
        private void OnGeoCoordinateWatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            OnLocationChanged();
        }

        /// <summary>
        /// Called when the status has changed, according to the <see cref="GeoCoordinateWatcher"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Device.Location.GeoPositionStatusChangedEventArgs"/> instance containing the event data.</param>
        private void OnGeoCoordinateWatcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Log.Info("Changed status to '{0}'", e.Status);

            OnLocationChanged();
        }

        /// <summary>
        /// Called when the current location has changed.
        /// </summary>
        private void OnLocationChanged()
        {
            if (LocationChanged == null)
            {
                return;
            }

            var currentLocation = GetCurrentLocation();
            var value = new LocationChangedEventArgs(currentLocation);

            // Must be thread-safe, dispatch
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(() => LocationChanged(this, value));
            }
            else
            {
                // If no dispatcher is available, trust the user of this service
                LocationChanged(this, value);
            }
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        public ILocation GetCurrentLocation()
        {
            var currentCoordinate = _geoCoordinateWatcher.Position;
            var currentLocation = new Location(currentCoordinate.Location);

            return currentLocation;
        }

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        public bool Start()
        {
            Log.Info("Starting LocationService");

            var result = _geoCoordinateWatcher.TryStart(false, new TimeSpan(0, 0, 5));

            if (result)
            {
                Log.Info("Started LocationService");
            }
            else
            {
                Log.Info("Failed to start LocationService");
            }

            return result;
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        public void Stop()
        {
            Log.Info("Stopping LocationService");

            _geoCoordinateWatcher.Stop();

            Log.Info("Stopped LocationService");
        }
        #endregion
    }
}
