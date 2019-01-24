// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SensorServiceBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Base class for implementing sensor services.
    /// </summary>
    public abstract class SensorServiceBase<TValueInterface, TEventArgs> : ViewModelServiceBase, ISensorService<TValueInterface, TEventArgs>
        where TEventArgs : EventArgs 
    {
        private readonly IDispatcherService _dispatcherService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SensorServiceBase{TValueInterface, TEventArgs}"/> class.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        protected SensorServiceBase(IDispatcherService dispatcherService)
        {
            Argument.IsNotNull("dispatcherService", dispatcherService);

            _dispatcherService = dispatcherService;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the device supports the current sensor and thus supports getting values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSupported { get { return false; } }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        public virtual TimeSpan TimeBetweenUpdates { get; set; }

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
        public virtual TValueInterface GetCurrentValue()
        {
            return default(TValueInterface);
        }

        /// <summary>
        /// Starts the sensor service so it's retrieving data.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops the sensor service so it's no longer retrieving data.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Raises the <see cref="CurrentValueChanged"/> event.
        /// </summary>
        protected void RaiseCurrentValueChanged(TEventArgs e)
        {
            _dispatcherService.BeginInvoke(() => OnCurrentValueChanged(this, e));
        }

        /// <summary>
        /// Method to invoke the <see cref="CurrentValueChanged"/> event from derived classes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void OnCurrentValueChanged(object sender, TEventArgs e)
        {
            CurrentValueChanged?.Invoke(sender, e);
        }
        #endregion
    }
}
