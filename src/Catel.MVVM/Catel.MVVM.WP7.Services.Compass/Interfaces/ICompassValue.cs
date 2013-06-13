// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompassValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// Value information about the compass.
    /// </summary>
    public interface ICompassValue
    {
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the accuracy, in degrees, of the compass heading readings.
        /// </summary>
        /// <value>The accuracy of the compass heading readings.</value>
        double HeadingAccuracy { get; }

        /// <summary>
        /// Gets the heading, in degrees, measured clockwise from the Earth’s magnetic north.
        /// </summary>
        /// <value>The heading relative to the Earth’s magnetic north.</value>
        double MagneticHeading { get; }

        /// <summary>
        /// Gets the raw magnetometer reading in microteslas.
        /// </summary>
        /// <value>The raw magnetometer reading.</value>
        IVector3 MagnetometerReading { get; }

        /// <summary>
        /// Gets the heading, in degrees, measured clockwise from the Earth’s geographic north.
        /// </summary>
        /// <value>The heading relative to the Earth’s geographic north.</value>
        double TrueHeading { get; }
    }
}
