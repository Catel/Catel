// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompassService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Microsoft.Devices.Sensors;

    /// <summary>
    /// Implementation of the <see cref="ICompassService"/> interface.
    /// </summary>
    public class CompassService : SensorServiceBase<ICompassValue, CompassValueChangedEventArgs>, ICompassService
    {
        #region Fields
        private readonly Compass _sensor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CompassService"/> class.
        /// </summary>
        public CompassService()
        {
            if (IsSupported)
            {
                _sensor = new Compass();
                _sensor.CurrentValueChanged += OnSensorValueChanged;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the device supports the current sensor and thus supports getting values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSupported
        {
            get { return Compass.IsSupported; }
        }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        public override TimeSpan TimeBetweenUpdates
        {
            get { return _sensor.TimeBetweenUpdates; }
            set { _sensor.TimeBetweenUpdates = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the current sensor value.
        /// </summary>
        /// <returns>
        /// The current sensor value. If no value is available, <c>null</c> will be returned.
        /// </returns>
        public override ICompassValue GetCurrentValue()
        {
            return new CompassValue(_sensor.CurrentValue);
        }

        /// <summary>
        /// Starts the sensor service so it's retrieving data.
        /// </summary>
        public override void Start()
        {
            _sensor.Start();
        }

        /// <summary>
        /// Stops the sensor service so it's no longer retrieving data.
        /// </summary>
        public override void Stop()
        {
            _sensor.Stop();
        }

        /// <summary>
        /// Called when the current location has changed.
        /// </summary>
        private void OnSensorValueChanged(object sender, EventArgs e)
        {
            var value = new CompassValueChangedEventArgs(CurrentValue);

            // Must be thread-safe, dispatch
            if (Dispatcher != null)
            {
                Dispatcher.BeginInvoke(() => OnCurrentValueChanged(this, value));
            }
            else
            {
                // If no dispatcher is available, trust the user of this service
                OnCurrentValueChanged(this, value);
            }
        }
        #endregion
    }
}
