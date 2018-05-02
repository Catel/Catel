// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if ANDROID

namespace Catel.Services
{
    public partial class PleaseWaitService
    {
        partial void SetStatus(string status)
        {
            throw new MustBeImplementedException();
        }

        partial void InitializeBusyIndicator()
        {
            throw new MustBeImplementedException();
        }

        partial void ShowBusyIndicator(bool indeterminate)
        {
            throw new MustBeImplementedException();
        }

        partial void HideBusyIndicator()
        {
            throw new MustBeImplementedException();
        }
    }
}

#endif