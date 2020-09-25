#if ANDROID

namespace Catel.Services
{
    public partial class BusyIndicatorService
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
