namespace Catel.Services;

using System.Windows.Input;
using Logging;
using Microsoft.Extensions.Logging;

public partial class BusyIndicatorService
{
    private Cursor? _previousCursor;

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
        _dispatcherService.BeginInvokeIfRequiredAsync(() =>
        {
            var overrideCursor = Mouse.OverrideCursor;

            _logger.LogDebug($"Storing cursor '{overrideCursor}' overriding it to 'Wait'");

            if (_previousCursor is null)
            {
                _previousCursor = overrideCursor;
            }

            Mouse.OverrideCursor = Cursors.Wait;
        });
    }

    partial void HideBusyIndicator()
    {
        _dispatcherService.BeginInvokeIfRequiredAsync(() =>
        {
            _logger.LogDebug($"Restoring cursor '{_previousCursor}'");

            Mouse.OverrideCursor = _previousCursor;

            _previousCursor = null;
        });
    }
}
