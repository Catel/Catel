// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vector3.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   <see cref="Microsoft.Xna.Framework.Vector3"/> implementation to support sensors in MVVM.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    /// <summary>
    /// <see cref="Microsoft.Xna.Framework.Vector3"/> implementation to support sensors in MVVM.
    /// </summary>
    public class Vector3 : IVector3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class.
        /// </summary>
        /// <param name="vector">The vector.</param>
        public Vector3(Microsoft.Xna.Framework.Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

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
    }
}
