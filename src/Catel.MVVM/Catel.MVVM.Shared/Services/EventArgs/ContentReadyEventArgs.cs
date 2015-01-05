// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentReadyEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.IO;

    /// <summary>
    /// <see cref="EventArgs"/> implementation for camera content ready operations.
    /// </summary>
    public class ContentReadyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentReadyEventArgs"/> class.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="imageStream"/> is <c>null</c>.</exception>
        public ContentReadyEventArgs(Stream imageStream)
        {
            Argument.IsNotNull("imageStream", imageStream);

            ImageStream = imageStream;
        }

        /// <summary>
        /// Gets the image stream of the image.
        /// </summary>
        /// <value>The image stream.</value>
        public Stream ImageStream { get; private set; }
    }
}
