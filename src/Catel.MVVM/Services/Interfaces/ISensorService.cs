// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISensorService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface that serves as a base interface for interfaces supported sensors (especially on Windows Phone 7).
    /// </summary>
    /// <typeparam name="TValueInterface">The type of the value interface.</typeparam>
    /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
// ReSharper disable TypeParameterCanBeVariant
    public interface ISensorService<TValueInterface, TEventArgs> 
// ReSharper restore TypeParameterCanBeVariant
        where TEventArgs : EventArgs
    {
        /// <summary>
        /// Occurs when the current sensor value has changed.
        /// </summary>
        event EventHandler<TEventArgs> CurrentValueChanged;

        /// <summary>
        /// Gets a value indicating whether the device supports the current sensor and thus supports getting values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this device supports the current sensor; otherwise, <c>false</c>.
        /// </value>
        bool IsSupported { get; }

        /// <summary>
        /// Gets or sets the preferred time between updates.
        /// </summary>
        /// <value>The preferred time between updates.</value>
        TimeSpan TimeBetweenUpdates { get; set; }

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
        TValueInterface CurrentValue { get; }

        /// <summary>
        /// Gets the current sensor value.
        /// </summary>
        /// <returns>
        /// The current sensor value. If no value is available, <c>null</c> will be returned.
        /// </returns>
        TValueInterface GetCurrentValue();

        /// <summary>
        /// Starts the sensor service so it's retrieving data.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the sensor service so it's no longer retrieving data.
        /// </summary>
        void Stop();
    }
}
