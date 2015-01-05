// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteArrayExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System.Text;

    /// <summary>
    /// Extensions for byte arrays.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Gets the UTF8 string from the byte array.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.String.</returns>
        public static string GetUtf8String(this byte[] data)
        {
            return data.GetString(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the string from the byte array using the specified encoding.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        public static string GetString(this byte[] data, Encoding encoding)
        {
            Argument.IsNotNull("data", data);
            Argument.IsNotNull("encoding", encoding);

            return encoding.GetString(data, 0, data.Length);
        }
    }
}