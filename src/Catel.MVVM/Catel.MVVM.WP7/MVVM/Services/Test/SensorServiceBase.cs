// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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
    /// Base class for implementing test versions of sensor services.
    /// </summary>
    public abstract class SensorServiceBase<TValueInterface, TEventArgs> : ViewModelServiceBase, ISensorService<TValueInterface, TEventArgs>
        where TEventArgs : EventArgs
    {
        #region Fields
        private TValueInterface _currentValue;
        private readonly DispatcherTimer _timer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorServiceBase&lt;TValueInterface, TEventArgs&gt;"/> class.
        /// </summary>
        protected SensorServiceBase()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += OnTimerTick;

            ExpectedValues = new Queue<SensorTestData<TValueInterface>>();

            IsSupported = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the device supports the current sensor and thus supports getting values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is a test implementation. Therefore, the <see cref="IsSupported"/> value can be set as well to
        /// simulate whether a sensor is available or not.
        /// </remarks>
        public bool IsSupported { get; set; }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        /// <remarks>
        /// This property is not used in the test implementation of the services.
        /// </remarks>
        public TimeSpan TimeBetweenUpdates { get; set; }

        /// <summary>
        /// Gets the current sensor value. If no value is available, <c>null</c> will be returned.
        /// </summary>
        /// <value>The current sensor value.</value>
        /// <remarks>
        /// This is convenience property that internally calls <see cref="GetCurrentValue"/>.
        /// <para />
        /// Note that the services inside Catel do not support <see cref="INotifyPropertyChanged"/>, thus you cannot 
        /// subscribe to changes of this property. Instead, subscribe to the <see cref="CurrentValueChanged"/> event.
        /// </remarks>
        public TValueInterface CurrentValue { get { return GetCurrentValue(); } }

        /// <summary>
        /// Gets the queue of expected values.
        /// </summary>
        /// <value>The expected values.</value>
        public Queue<SensorTestData<TValueInterface>> ExpectedValues { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the current sensor value has changed.
        /// </summary>
        public event EventHandler<TEventArgs> CurrentValueChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the current sensor value.
        /// </summary>
        /// <returns>
        /// The current sensor value. If no value is available, <c>null</c> will be returned.
        /// </returns>
        public TValueInterface GetCurrentValue()
        {
            return _currentValue;
        }

        /// <summary>
        /// Starts the location service so it's retrieving data.
        /// </summary>
        public void Start()
        {
            if (ExpectedValues.Count > 0)
            {
                ProceedToNextLocation();
            }
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Method to invoke the <see cref="CurrentValueChanged"/> event from derived classes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnCurrentValueChanged(object sender, TEventArgs e)
        {
            if (CurrentValueChanged != null)
            {
                CurrentValueChanged(sender, e);
            }
        }

        /// <summary>
        /// Called when the timer reaches another tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (ExpectedValues.Count > 0)
            {
                ProceedToNextLocation();
            }
        }

        /// <summary>
        /// Proceeds to next location.
        /// </summary>
        public void ProceedToNextLocation()
        {
            _timer.Stop();

            if (ExpectedValues.Count == 0)
            {
                throw new Exception(Exceptions.NoExpectedResultsInQueueForUnitTest);
            }

            var expectedValue = ExpectedValues.Dequeue();
            _currentValue = expectedValue.Value;

            OnCurrentValueChanged(this, (TEventArgs)Activator.CreateInstance(typeof (TEventArgs), new object[] { expectedValue.Value }));

            if (expectedValue.Timeout.HasValue)
            {
                _timer.Interval = expectedValue.Timeout.Value;
                _timer.Start();
            }
        }
        #endregion
    }

    /// <summary>
    /// Test data which contains the value and time-out.
    /// </summary>
    public class SensorTestData<TValueInterface>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorTestData{TValueInterface}"/> class.
        /// <para />
        /// When this constructor is used, no time-out is used and only one location will be available.
        /// </summary>
        public SensorTestData(TValueInterface value)
            : this(value, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorTestData{TValueInterface}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        public SensorTestData(TValueInterface value, TimeSpan timeSpan)
            : this(value, (TimeSpan?)timeSpan) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorTestData{TValueInterface}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timeSpan">The time span.</param>
        public SensorTestData(TValueInterface value, TimeSpan? timeSpan)
        {
            Value = value;
            Timeout = timeSpan;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public TValueInterface Value { get; private set; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan? Timeout { get; private set; }
    }
}
