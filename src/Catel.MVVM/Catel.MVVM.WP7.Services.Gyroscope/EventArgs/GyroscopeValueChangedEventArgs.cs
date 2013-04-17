// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GyroscopeValueChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation which contains a new gyroscope value.
    /// </summary>
    public class GyroscopeValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GyroscopeValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newValue">The new gyroscope value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="newValue"/> is <c>null</c>.</exception>
        public GyroscopeValueChangedEventArgs(IGyroscopeValue newValue)
        {
            Argument.IsNotNull("newValue", newValue);

            Value = newValue;
        }

        /// <summary>
        /// Gets the new gyroscope value.
        /// </summary>
        /// <value>The new gyroscope value.</value>
        public IGyroscopeValue Value { get; private set; }
    }
}
