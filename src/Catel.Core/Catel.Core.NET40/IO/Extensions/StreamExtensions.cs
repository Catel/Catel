// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Extensions for the <see cref="Stream"/> class.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Converters the stream to a byte array.
        /// </summary>
        /// <param name="stream">The stream to convert to a byte array.</param>
        /// <returns>The byte array representing the stream.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        public static byte[] ToByteArray(this Stream stream)
        {
            Argument.IsNotNull("stream", stream);

            stream.Position = 0L;

            int length = (int) stream.Length;
            var buffer = new byte[length];

            stream.Read(buffer, 0, length);

            return buffer;
        }
    }
}