// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    using System;

    public partial class VibrateService
    {
        partial void StartVibration(TimeSpan duration)
        {
            throw new MustBeImplementedException();
        }

        partial void StopVibration()
        {
            throw new MustBeImplementedException();
        }
    }
}

#endif