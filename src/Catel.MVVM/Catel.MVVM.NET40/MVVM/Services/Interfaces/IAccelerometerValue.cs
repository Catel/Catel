// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAccelerometerValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// Value information about the accelerometer.
    /// </summary>
    public interface IAccelerometerValue
    {
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X coordinate.</value>
        double X { get; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y coordinate.</value>
        double Y { get; }

        /// <summary>
        /// Gets the Z coordinate.
        /// </summary>
        /// <value>The Z coordinate.</value>
        double Z { get; }
    }
}
