// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VibrateService.ios.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if IOS

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