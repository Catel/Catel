// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class BufferedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedEventArgs"/> class.
        /// </summary>
        /// <param name="bufferedException">The buffered exception.</param>
        /// <param name="dateTime">the date time that indicates when the buffering was invoked.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="bufferedException"/> is <c>null</c>.</exception>
        public BufferedEventArgs(Exception bufferedException, DateTime dateTime)
        {
            Argument.IsNotNull(() => bufferedException);

            BufferedException = bufferedException;

            DateTime = dateTime;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the buffered exception.
        /// </summary>
        public Exception BufferedException { get; private set; }

        /// <summary>
        /// Gets the date time that indicates when the buffering was invoked.
        /// </summary>
        public DateTime DateTime { get; private set; }
        #endregion
    }
}