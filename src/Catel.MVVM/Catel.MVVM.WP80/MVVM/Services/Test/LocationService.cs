// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services.Test
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Properties;

    /// <summary>
    /// Test implementation of the location service.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// 
    /// Test.LocationService service = (Test.LocationService)GetService<ILocationService>();
    /// 
    /// // Queue the next location (and then wait 5 seconds)
    /// var locationTestData = new LocationTestData(new Location(100d, 100d), new TimeSpan(0, 0, 0, 5)));
    /// service.ExpectedLocations.Add(locationTestData);
    /// 
    /// // Go to the next location manually
    /// service.ProceedToNextLocation();
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class LocationService : ViewModelServiceBase, ILocationService
    {
        #region Fields
        private ILocation _currentLocation;
        private readonly DispatcherTimer _timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationService"/> class.
        /// </summary>
        public LocationService()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            ExpectedLocations = new Queue<LocationTestData>();
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

        /// <summary>
        /// Gets the queue of expected locations.
        /// </summary>
        /// <value>The expected locations.</value>
        public Queue<LocationTestData> ExpectedLocations { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the current location has changed.
        /// </summary>
        public event EventHandler<LocationChangedEventArgs> LocationChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Called when the timer reaches another tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (ExpectedLocations.Count > 0)
            {
                ProceedToNextLocation();
            }
        }

        /// <summary>
        /// Called when the current location has changed.
        /// </summary>
        private void OnLocationChanged()
        {
            if (LocationChanged != null)
            {
                ILocation currentLocation = GetCurrentLocation();
                LocationChanged(this, new LocationChangedEventArgs(currentLocation));
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
            return _currentLocation;
        }

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        public bool Start()
        {
            if (ExpectedLocations.Count > 0)
            {
                ProceedToNextLocation();
            }

            return true;
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Proceeds to next location.
        /// </summary>
        public void ProceedToNextLocation()
        {
            _timer.Stop();

            if (ExpectedLocations.Count == 0)
            {
                throw new Exception(Exceptions.NoExpectedResultsInQueueForUnitTest);
            }

            var locationTestData = ExpectedLocations.Dequeue();

            _currentLocation = locationTestData.Location;
            OnLocationChanged();

            if (locationTestData.Timeout.HasValue)
            {
                _timer.Interval = locationTestData.Timeout.Value;
                _timer.Start();
            }
        }
        #endregion
    }

    /// <summary>
    /// Test data which contains the location and time-out.
    /// </summary>
    public class LocationTestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocationTestData"/> class.
        /// <para />
        /// When this constructor is used, no time-out is used and only one location will be available.
        /// </summary>
        public LocationTestData(ILocation location)
            : this(location, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationTestData"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="timeSpan">The time span.</param>
        public LocationTestData(ILocation location, TimeSpan timeSpan)
            : this(location, (TimeSpan?)timeSpan) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocationTestData"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="timeSpan">The time span.</param>
        public LocationTestData(ILocation location, TimeSpan? timeSpan)
        {
            Location = location;
            Timeout = timeSpan;
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public ILocation Location { get; private set; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan? Timeout { get; private set; }
    }
}
