// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using Microsoft.Devices.Sensors;

    /// <summary>
    /// Implementation of the <see cref="IAccelerometerService"/> interface.
    /// </summary>
    public class AccelerometerService : SensorServiceBase<IAccelerometerValue, AccelerometerValueChangedEventArgs>, IAccelerometerService
    {
        #region Fields
        private readonly Accelerometer _sensor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerService"/> class.
        /// </summary>
        public AccelerometerService()
        {
            if (IsSupported)
            {
                _sensor = new Accelerometer();
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
            get { return Accelerometer.IsSupported; }
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
        public override IAccelerometerValue GetCurrentValue()
        {
            return new AccelerometerValue(_sensor.CurrentValue);
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
            var value = new AccelerometerValueChangedEventArgs(CurrentValue);

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
