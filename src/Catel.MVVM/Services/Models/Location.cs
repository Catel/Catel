// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Location.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// Class that represents a location.
    /// </summary>
    public class Location : ILocation
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        public Location (double latitude, double longitude)
            : this(latitude, longitude, 0d) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Location"/> class.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="altitude">The altitude.</param>
        public Location(double latitude, double longitude, double altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the latitude. The latitute is the angular distance of that location south or north of the equator.
        /// </summary>
        /// <value>The latitude.</value>
        public double Latitude { get; private set; }

        /// <summary>
        /// Gets the longitude. The longitude specifies the east-west position of a point on the Earth's surface.
        /// </summary>
        /// <value>The longitude.</value>
        public double Longitude { get; private set; }

        /// <summary>
        /// Gets the altitude. The altitude is the height of the location.
        /// </summary>
        /// <value>The altitude.</value>
        public double Altitude { get; private set; }
        #endregion

        #region Methods
        #endregion
    }
}
