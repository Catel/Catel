// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Services
{
    using Catel.Windows;

    public partial class PleaseWaitService
    {
        partial void SetStatus(string status)
        {
            PleaseWaitHelper.UpdateStatus(status);
        }

        partial void InitializeBusyIndicator()
        {
            // not required
        }

        partial void ShowBusyIndicator()
        {
            PleaseWaitHelper.Show(_lastStatus);
        }

        partial void HideBusyIndicator()
        {
            PleaseWaitHelper.Hide();
        }
    }
}

#endif