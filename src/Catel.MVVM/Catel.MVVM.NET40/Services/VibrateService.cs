// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;

    /// <summary>
    /// Implementation of the <see cref="IVibrateService"/>.
    /// </summary>
    public partial class VibrateService : IVibrateService
    {
        /// <summary>
        /// Starts vibration on the device.
        /// </summary>
        /// <param name="duration">A TimeSpan object specifying the amount of time, in seconds, for which the phone vibrates.
        /// Valid times are between 0 and 5 seconds. Values greater than 5 or less than 0 will generate an exception.</param>
        /// <exception cref="ArgumentOutOfRangeException">Duration is greater than the 5 seconds or duration is negative.</exception>
        public void Start(TimeSpan duration)
        {
            StartVibration(duration);
        }

        /// <summary>
        /// Stops the vibration on the device.
        /// </summary>
        public void Stop()
        {
            StopVibration();
        }

        partial void StartVibration(TimeSpan duration);
        partial void StopVibration();
    }
}