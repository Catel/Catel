// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if IOS

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

        partial void ShowBusyIndicator()
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