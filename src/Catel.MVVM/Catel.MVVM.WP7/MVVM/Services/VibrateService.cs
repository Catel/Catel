// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Microsoft.Devices;

    /// <summary>
    /// Windows Phone 7 implementation of the <see cref="IVibrateService"/>.
    /// </summary>
    public class VibrateService : IVibrateService
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Starts vibration on the device.
        /// </summary>
        /// <param name="duration">A TimeSpan object specifying the amount of time, in seconds, for which the phone vibrates.
        /// Valid times are between 0 and 5 seconds. Values greater than 5 or less than 0 will generate an exception.</param>
        /// <exception cref="ArgumentOutOfRangeException">Duration is greater than the 5 seconds or duration is negative.</exception>
        public void Start(TimeSpan duration)
        {
            VibrateController.Default.Start(duration);
        }

        /// <summary>
        /// Stops the vibration on the device.
        /// </summary>
        public void Stop()
        {
            VibrateController.Default.Stop();
        }
        #endregion
    }
}
