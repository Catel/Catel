// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// The IExceptionHandler extensions.
    /// </summary>
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        /// Usings the tolerance.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionHandler" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is out of range.</exception>
        public static void UsingTolerance(this IExceptionHandler exceptionHandler, int numberOfTimes, TimeSpan duration)
        {
            Argument.IsNotNull(() => exceptionHandler);

            exceptionHandler.AllowedFrequency = new Frequency(numberOfTimes, duration);
        }
    }
}