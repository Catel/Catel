// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT

namespace Catel.Services
{
    using System;
    using Microsoft.Devices;

    public partial class VibrateService : IVibrateService
    {
        partial void StartVibration(TimeSpan duration)
        {
            VibrateController.Default.Start(duration);
        }

        partial void StopVibration()
        {
            VibrateController.Default.Stop();
        }
    }
}

#endif