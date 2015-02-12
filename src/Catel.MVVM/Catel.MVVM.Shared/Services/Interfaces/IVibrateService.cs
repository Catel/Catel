// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVibrateService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// Interface defining the functionality of the vibrate service.
    /// </summary>
    public interface IVibrateService
    {
        /// <summary>
        /// Starts vibration on the device.
        /// </summary>
        /// <param name="duration">
        /// A TimeSpan object specifying the amount of time, in seconds, for which the phone vibrates.
        /// Valid times are between 0 and 5 seconds. Values greater than 5 or less than 0 will generate an exception.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">Duration is greater than the 5 seconds or duration is negative.</exception>
        void Start(TimeSpan duration);

        /// <summary>
        /// Stops the vibration on the device.
        /// </summary>
        void Stop();
    }
}
