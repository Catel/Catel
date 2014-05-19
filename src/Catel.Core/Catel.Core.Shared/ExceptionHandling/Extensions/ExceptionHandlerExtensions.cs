// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// The exception handler extensions.
    /// </summary>
    public static class ExceptionHandlerExtensions
    {
        /// <summary>
        /// Should buffer the exceptions using the specified policy tolerance.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="numberOfTimes">The number of times before handling the exception.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionHandler" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is out of range.</exception>
        public static void UsingTolerance(this IExceptionHandler exceptionHandler, int numberOfTimes, TimeSpan interval)
        {
            Argument.IsNotNull("exceptionHandler", exceptionHandler);

            exceptionHandler.BufferPolicy = new BufferPolicy(numberOfTimes, interval);
        }

        /// <summary>
        /// Should retry the action on error using the provided policy.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="numberOfTimes">The number of times to attempts.</param>
        /// <param name="interval">The interval between two attempts.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="exceptionHandler"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is larger than <c>1</c>.</exception>
        public static void OnErrorRetry(this IExceptionHandler exceptionHandler, int numberOfTimes, TimeSpan interval)
        {
            Argument.IsNotNull("exceptionHandler", exceptionHandler);
            
            exceptionHandler.RetryPolicy = new RetryPolicy(numberOfTimes, interval);
        }

        /// <summary>
        /// Should retry action on error immediately.
        /// </summary>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="exceptionHandler"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is larger than <c>1</c>.</exception>
        public static void OnErrorRetryImmediately(this IExceptionHandler exceptionHandler, int numberOfTimes = int.MaxValue)
        {
            Argument.IsNotNull("exceptionHandler", exceptionHandler);

            exceptionHandler.OnErrorRetry(numberOfTimes, TimeSpan.FromMilliseconds(1));
        }
    }
}