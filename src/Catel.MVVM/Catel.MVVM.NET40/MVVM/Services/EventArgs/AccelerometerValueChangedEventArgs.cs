// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccelerometerValueChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation which contains a new accelerometer value.
    /// </summary>
    public class AccelerometerValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newValue">The new accelerometer value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="newValue"/> is <c>null</c>.</exception>
        public AccelerometerValueChangedEventArgs(IAccelerometerValue newValue)
        {
            Argument.IsNotNull("newValue", newValue);

            Value = newValue;
        }

        /// <summary>
        /// Gets the new accelerometer value.
        /// </summary>
        /// <value>The new accelerometer value.</value>
        public IAccelerometerValue Value { get; private set; }
    }
}
