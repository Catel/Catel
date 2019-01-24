// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    using System;
    using global::Android.App;
    using global::Android.Content;
    using global::Android.Hardware;

    /// <summary>
    /// Sensor listener for the accelerometer.
    /// </summary>
    public class AccelerometerSensorListener : Java.Lang.Object, ISensorEventListener
    {
        /// <summary>
        /// Invoked when the sensor value has changed.
        /// </summary>
        public event EventHandler<AccelerometerValueChangedEventArgs> SensorChanged;

        /// <summary>
        /// Called when the accuracy of a sensor has changed.
        /// </summary>
        /// <param name="sensor">The sensor.</param>
        /// <param name="accuracy">The new accuracy of this sensor.</param>
        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        /// <summary>
        /// Called when sensor values have changed.
        /// </summary>
        /// <param name="e">the <c><see cref="T:Android.Hardware.SensorEvent" /></c>.</param>
        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type != SensorType.Accelerometer)
            {
                return;
            }

            var dateTimeOffset = new DateTimeOffset().AddMilliseconds(e.Timestamp);
            var value = new AccelerometerValue(dateTimeOffset, e.Values[0], e.Values[1], e.Values[2]);
            var eventArgs = new AccelerometerValueChangedEventArgs(value);
            SensorChanged?.Invoke(this, eventArgs);
        }
    }

    public partial class AccelerometerService
    {
        #region Fields
        private SensorManager _sensorManager;
        private Sensor _sensor;
        private AccelerometerSensorListener _sensorListener;

        private IAccelerometerValue _lastValue;
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
            get { return _sensorListener != null; }
        }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        public override TimeSpan TimeBetweenUpdates
        {
            get { return TimeSpan.FromMilliseconds(_sensor.MinDelay); }
            set { throw new NotSupportedException(); }
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
            return _lastValue;
        }

        partial void Initialize()
        {
            var context = Catel.Android.ContextHelper.CurrentContext;
            _sensorManager = context.GetSystemService(Context.SensorService) as SensorManager;
            _sensor = _sensorManager.GetDefaultSensor(SensorType.Accelerometer);

            _sensorListener = new AccelerometerSensorListener();
            _sensorListener.SensorChanged += OnSensorChanged;
        }

        partial void StartSensor()
        {
            _sensorManager.RegisterListener(_sensorListener, _sensor, SensorDelay.Ui);
        }

        partial void StopSensor()
        {
            _sensorManager.UnregisterListener(_sensorListener);
        }

        private void OnSensorChanged(object sender, AccelerometerValueChangedEventArgs e)
        {
            _lastValue = e.Value;

            RaiseCurrentValueChanged(e);
        }
        #endregion
    }
}

#endif
