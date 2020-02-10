// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Text
{
    using System.Text;

    /// <summary>
    /// Extensions for the <see cref="StringBuilder"/> class.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a new line with formatting options to the string builder.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/>.</param>
        /// <param name="format">The message format.</param>
        /// <param name="arg0">A formatting argument.</param>
        public static void AppendLine(this StringBuilder sb, string format, object arg0)
        {
            sb.AppendLine(string.Format(format, arg0));
        }

        /// <summary>
        /// Appends a new line with formatting options to the string builder.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/>.</param>
        /// <param name="format">The message format.</param>
        /// <param name="arg0">A formatting argument.</param>
        /// <param name="arg1">A formatting argument.</param>
        public static void AppendLine(this StringBuilder sb, string format, object arg0, object arg1)
        {
            sb.AppendLine(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Appends a new line with formatting options to the string builder.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/>.</param>
        /// <param name="format">The message format.</param>
        /// <param name="arg0">A formatting argument.</param>
        /// <param name="arg1">A formatting argument.</param>
        /// <param name="arg2">A formatting argument.</param>
        public static void AppendLine(this StringBuilder sb, string format, object arg0, object arg1, object arg2)
        {
            sb.AppendLine(string.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Appends a new line with formatting options to the string builder.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/>.</param>
        /// <param name="format">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        public static void AppendLine(this StringBuilder sb, string format, params object[] args)
        {
            sb.AppendLine(string.Format(format, args));
        }
    }
}
