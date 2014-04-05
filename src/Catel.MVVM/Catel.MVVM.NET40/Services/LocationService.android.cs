// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if ANDROID

namespace Catel.Services
{
    using System;
    using global::Android.App;
    using global::Android.Content;
    using global::Android.Locations;
    using global::Android.OS;

    /// <summary>
    /// A location listener.
    /// </summary>
    public class LocationListener : Java.Lang.Object, ILocationListener
    {
        /// <summary>
        /// Occurs when the status has changed.
        /// </summary>
        public event EventHandler<EventArgs> StatusChanged;

        /// <summary>
        /// Occurs when the location has changed.
        /// </summary>
        public event EventHandler<EventArgs> LocationChanged;

        /// <summary>
        /// Called when the provider is enabled by the user.
        /// </summary>
        /// <param name="provider">The name of the location provider associated with this update.</param>
        public void OnProviderEnabled(string provider)
        {
            // Not required
        }

        /// <summary>
        /// Called when the provider is disabled by the user.
        /// </summary>
        /// <param name="provider">The name of the location provider associated with this update.</param>		
        public void OnProviderDisabled(string provider)
        {
            // Not required
        }

        /// <summary>
        /// Called when the provider status changes.
        /// </summary>
        /// <param name="provider">The name of the location provider associated with this update.</param>
        /// <param name="status">The status.</param>
        /// <param name="extras">The extras.</param>
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            StatusChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the location has changed.
        /// </summary>
        /// <param name="location">The new location, as a Location object.</param>
        public void OnLocationChanged(global::Android.Locations.Location location)
        {
            LocationChanged.SafeInvoke(this);
        }
    }

    public partial class LocationService
    {
        private const string Provider = LocationManager.GpsProvider;
        private LocationManager _locationManager;
        private LocationListener _locationListener;

        private void OnStatusChanged(object sender, EventArgs e)
        {
            RaiseLocationChanged();
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            RaiseLocationChanged();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        protected override void Initialize()
        {
            var context = Application.Context;
            _locationManager = context.GetSystemService(Context.LocationService) as LocationManager;

            _locationListener = new LocationListener();
            _locationListener.StatusChanged += OnStatusChanged;
            _locationListener.LocationChanged += OnLocationChanged;
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        public override ILocation GetCurrentLocation()
        {
            var location = _locationManager.GetLastKnownLocation(Provider);

            var currentLocation = new Location(location.Latitude, location.Longitude, location.Altitude);

            return currentLocation;
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        protected override bool StartSensor()
        {
            if (!_locationManager.IsProviderEnabled(Provider))
            {
                return false;
            }

            _locationManager.RequestLocationUpdates(Provider, 2000, 1, _locationListener);
            return true;
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        protected override void StopSensor()
        {
            _locationManager.RemoveUpdates(_locationListener);
        }
    }
}

#endif