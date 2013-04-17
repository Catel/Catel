// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GyroscopeValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Microsoft.Devices.Sensors;

    /// <summary>
    /// Gyroscope reading value.
    /// </summary>
    public class GyroscopeValue : IGyroscopeValue
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GyroscopeValue"/> class.
        /// </summary>
        /// <param name="reading">The gyroscope reading.</param>
        public GyroscopeValue(GyroscopeReading reading)
            : this(reading.Timestamp, reading.RotationRate.X, reading.RotationRate.Y, reading.RotationRate.Z) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GyroscopeValue"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public GyroscopeValue(DateTimeOffset timestamp, double x, double y, double z)
        {
            Timestamp = timestamp;
            X = x;
            Y = y;
            Z = z;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTimeOffset Timestamp { get; private set; }

        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X coordinate.</value>
        public double X { get; private set; }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y coordinate.</value>
        public double Y { get; private set; }

        /// <summary>
        /// Gets the Z coordinate.
        /// </summary>
        /// <value>The Z coordinate.</value>
        public double Z { get; private set; }
        #endregion

    }
}
