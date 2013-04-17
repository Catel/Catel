// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompassValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Microsoft.Devices.Sensors;

    /// <summary>
    /// Compass reading value.
    /// </summary>
    public class CompassValue : ICompassValue
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CompassValue"/> class.
        /// </summary>
        /// <param name="reading">The compass reading.</param>
        public CompassValue(CompassReading reading)
            : this(reading.Timestamp, reading.HeadingAccuracy, reading.MagneticHeading, 
                   new Vector3(reading.MagnetometerReading), reading.TrueHeading) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompassValue"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="headingAccuracy">The heading accuracy.</param>
        /// <param name="magneticHeading">The magnetic heading.</param>
        /// <param name="magnetometerReading">The magnetometer reading.</param>
        /// <param name="trueHeading">The true heading.</param>
        public CompassValue(DateTimeOffset timestamp, double headingAccuracy, double magneticHeading, IVector3 magnetometerReading,
            double trueHeading)
        {
            Timestamp = timestamp;
            HeadingAccuracy = headingAccuracy;
            MagneticHeading = magneticHeading;
            MagnetometerReading = magnetometerReading;
            TrueHeading = trueHeading;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTimeOffset Timestamp { get; private set; }

        /// <summary>
        /// Gets the accuracy, in degrees, of the compass heading readings.
        /// </summary>
        /// <value>The accuracy of the compass heading readings.</value>
        public double HeadingAccuracy { get; private set; }

        /// <summary>
        /// Gets the heading, in degrees, measured clockwise from the Earth’s magnetic north.
        /// </summary>
        /// <value>The heading relative to the Earth’s magnetic north.</value>
        public double MagneticHeading { get; private set; }

        /// <summary>
        /// Gets the raw magnetometer reading in microteslas.
        /// </summary>
        /// <value>The raw magnetometer reading.</value>
        public IVector3 MagnetometerReading { get; private set; }

        /// <summary>
        /// Gets the heading, in degrees, measured clockwise from the Earth’s geographic north.
        /// </summary>
        /// <value>The heading relative to the Earth’s geographic north.</value>
        public double TrueHeading { get; private set; }
        #endregion
    }
}
