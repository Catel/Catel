// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT

namespace Catel.Services
{
    using System;
    using Microsoft.Devices.Sensors;

    public partial class AccelerometerService
    {
        #region Fields
        private Accelerometer _sensor;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the device supports the current sensor and thus supports getting values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
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
            var currentValue = _sensor.CurrentValue;

            return new AccelerometerValue(currentValue.Timestamp, currentValue.Acceleration.X, 
                currentValue.Acceleration.Y, currentValue.Acceleration.Z);
        }

        partial void Initialize()
        {
            if (IsSupported)
            {
                _sensor = new Accelerometer();
                _sensor.CurrentValueChanged += OnSensorValueChanged;    
            }
        }

        partial void StartSensor()
        {
            _sensor.Start();
        }

        partial void StopSensor()
        {
            _sensor.Stop();
        }

        private void OnSensorValueChanged(object sender, EventArgs e)
        {
            RaiseCurrentValueChanged(new AccelerometerValueChangedEventArgs(CurrentValue));
        }
        #endregion
    }
}

#endif