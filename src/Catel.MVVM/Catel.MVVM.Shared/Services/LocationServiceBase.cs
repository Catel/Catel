// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.ComponentModel;
    using Catel.Logging;

    /// <summary>
    /// Class to allow partial abstract methods.
    /// </summary>
    public abstract class LocationServiceBase : ILocationService
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly IDispatcherService _dispatcherService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationServiceBase"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        protected LocationServiceBase(IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => dispatcherService);

            _dispatcherService = dispatcherService;

            Initialize();
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
        /// Called when the current location has changed.
        /// </summary>
        protected void RaiseLocationChanged()
        {
            var locationChanged = LocationChanged;
            if (locationChanged != null)
            {
                var currentLocation = GetCurrentLocation();
                var value = new LocationChangedEventArgs(currentLocation);

                _dispatcherService.BeginInvoke(() => locationChanged(this, value));
            }
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        protected virtual void Initialize()
        {
            
        }

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        public bool Start()
        {
            Log.Info("Starting LocationService");

            var result = StartSensor();
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

            StopSensor();

            Log.Info("Stopped LocationService");
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        public virtual ILocation GetCurrentLocation()
        {
            return null;
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        protected virtual bool StartSensor()
        {
            return false;
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        protected virtual void StopSensor()
        {
        }
        #endregion
    }
}