// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;

    /// <summary>
    /// Service that supports retrieving the current location.
    /// </summary>
    public class LocationService : ViewModelServiceBase, ILocationService
    {
        #region Fields
        private readonly GeoCoordinateWatcher _geoCoordinateWatcher = new GeoCoordinateWatcher();

        private bool _hasLocation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        public LocationService()
        {
            _geoCoordinateWatcher.PositionChanged += OnGeoCoordinateWatcherPositionChanged;
            _geoCoordinateWatcher.StatusChanged += OnGeoCoordinateWatcherStatusChanged;

            if (_geoCoordinateWatcher.Status == GeoPositionStatus.Ready)
            {
                _hasLocation = true;
            }
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
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                case GeoPositionStatus.Initializing:
                case GeoPositionStatus.NoData:
                    _hasLocation = false;
                    OnLocationChanged();
                    break;

                case GeoPositionStatus.Ready:
                    if (!_hasLocation)
                    {
                        _hasLocation = true;
                        OnLocationChanged();
                    }

                    break;
            }   
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

            var value = new LocationChangedEventArgs(GetCurrentLocation());

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
            ILocation currentLocation = null;

            if (_hasLocation)
            {
                var currentCoordinate = _geoCoordinateWatcher.Position;
                currentLocation = new Location(currentCoordinate.Location);
            }

            return currentLocation;
        }

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        public void Start()
        {
            _geoCoordinateWatcher.TryStart(false, new TimeSpan(0, 0, 5));
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        public void Stop()
        {
            _geoCoordinateWatcher.Stop();
        }
        #endregion
    }
}
