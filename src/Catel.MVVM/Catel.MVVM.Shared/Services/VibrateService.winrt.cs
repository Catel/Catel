// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Services
{
    using System;

    public partial class VibrateService
    {
        partial void StartVibration(TimeSpan duration)
        {
            // Not supported?
        }

        partial void StopVibration()
        {
            // Not supported?
        }
    }
}

#endif