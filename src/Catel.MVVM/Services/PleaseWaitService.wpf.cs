// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System.Windows.Input;
    using Catel.Windows;
    using Logging;

    public partial class PleaseWaitService
    {
        private Cursor _previousCursor;

        partial void SetStatus(string status)
        {
            // not required
        }

        partial void InitializeBusyIndicator()
        {
            // not required
        }

        partial void ShowBusyIndicator(bool indeterminate)
        {
            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                var overrideCursor = Mouse.OverrideCursor;

                Log.Debug($"Storing cursor '{overrideCursor}' overriding it to 'Wait'");

                if (_previousCursor is null)
                {
                    _previousCursor = overrideCursor;
                }

                Mouse.OverrideCursor = Cursors.Wait;
            });
        }

        partial void HideBusyIndicator()
        {
            _dispatcherService.BeginInvokeIfRequired(() =>
            {
                Log.Debug($"Restoring cursor '{_previousCursor}'");

                Mouse.OverrideCursor = _previousCursor;

                _previousCursor = null;
            });
        }
    }
}

#endif
