// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT

namespace Catel.Services
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using Catel.Logging;

    public partial class LocationService
    {
        #region Fields
        private GeoCoordinateWatcher _geoCoordinateWatcher;
        #endregion

        #region Methods
        private void OnGeoCoordinateWatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            RaiseLocationChanged();
        }

        private void OnGeoCoordinateWatcherStatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Log.Info("Changed status to '{0}'", e.Status);

            RaiseLocationChanged();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        protected override void Initialize()
        {
            _geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            _geoCoordinateWatcher.MovementThreshold = 0;

            _geoCoordinateWatcher.PositionChanged += OnGeoCoordinateWatcherPositionChanged;
            _geoCoordinateWatcher.StatusChanged += OnGeoCoordinateWatcherStatusChanged;
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        public override ILocation GetCurrentLocation()
        {
            var currentCoordinate = _geoCoordinateWatcher.Position;
            var currentLocation = new Location(currentCoordinate.Location.Latitude, currentCoordinate.Location.Longitude, 
                currentCoordinate.Location.Altitude);

            return currentLocation;
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        protected override bool StartSensor()
        {
            return _geoCoordinateWatcher.TryStart(false, new TimeSpan(0, 0, 5));
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        protected override void StopSensor()
        {
            _geoCoordinateWatcher.Stop();
        }
        #endregion
    }
}

#endif