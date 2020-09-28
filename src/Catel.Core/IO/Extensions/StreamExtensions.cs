// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IO
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Collections;

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

            var length = (int)stream.Length;
            if (length == 0)
            {
                return ArrayShim.Empty<byte>();
            }

            var buffer = new byte[length];

            stream.Read(buffer, 0, length);

            return buffer;
        }

        /// <summary>
        /// Gets the UTF8 string from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>System.String.</returns>
        public static string GetUtf8String(this Stream stream)
        {
            return stream.GetString(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the string from the stream using the specified encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        public static string GetString(this Stream stream, Encoding encoding)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("encoding", encoding);

            stream.Position = 0;

            using var reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd();
        }
    }
}
