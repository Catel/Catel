// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompassValueChangedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation which contains a new compass value.
    /// </summary>
    public class CompassValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompassValueChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newValue">The new compass value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="newValue"/> is <c>null</c>.</exception>
        public CompassValueChangedEventArgs(ICompassValue newValue)
        {
            Argument.IsNotNull("newValue", newValue);

            Value = newValue;
        }

        /// <summary>
        /// Gets the new gyroscope value.
        /// </summary>
        /// <value>The new gyroscope value.</value>
        public ICompassValue Value { get; private set; }
    }
}
