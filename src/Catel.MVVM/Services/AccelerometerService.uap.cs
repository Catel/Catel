// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if UWP

namespace Catel.Services
{
    using System;
    using global::Windows.Devices.Sensors;

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
        /// 	<c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSupported
        {
            get { return _sensor != null; }
        }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        public override TimeSpan TimeBetweenUpdates
        {
            get { return TimeSpan.FromMilliseconds(_sensor.ReportInterval); }
            set { _sensor.ReportInterval = (uint)value.TotalMilliseconds; }
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
            var currentValue = _sensor.GetCurrentReading();

            return new AccelerometerValue(currentValue.Timestamp, currentValue.AccelerationX,
                currentValue.AccelerationY, currentValue.AccelerationZ);
        }

        partial void Initialize()
        {
            _sensor = Accelerometer.GetDefault();
        }

        partial void StartSensor()
        {
            // Not required
        }

        partial void StopSensor()
        {
            // Not required
        }

        private void OnSensorValueChanged(object sender, EventArgs e)
        {
            RaiseCurrentValueChanged(new AccelerometerValueChangedEventArgs(CurrentValue));
        }
        #endregion
    }
}

#endif
