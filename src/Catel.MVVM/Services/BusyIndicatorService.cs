namespace Catel.Services
{
    using System;
    using Catel.Logging;

    /// <summary>
    /// Service to show a busy indicator.
    /// </summary>
    public partial class BusyIndicatorService : IBusyIndicatorService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILanguageService _languageService;
        private readonly IDispatcherService _dispatcherService;

        private string _lastStatus = string.Empty;

        public BusyIndicatorService(ILanguageService languageService, IDispatcherService dispatcherService)
        {
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(dispatcherService);

            _languageService = languageService;
            _dispatcherService = dispatcherService;
        }

        public int ShowCounter { get; private set; }

        partial void SetStatus(string status);
        partial void InitializeBusyIndicator();
        partial void ShowBusyIndicator(bool indeterminate);
        partial void HideBusyIndicator();

        public void Show(string status = "")
        {
            ShowCounter = 1;

            if (string.IsNullOrEmpty(status))
            {
                status = _languageService.GetString("PleaseWait") ?? string.Empty;
            }

            UpdateStatus(status);

            ShowBusyIndicator(true);
        }

        public void Show(BusyIndicatorWorkDelegate workDelegate, string status = "")
        {
            ArgumentNullException.ThrowIfNull(workDelegate);

            InitializeBusyIndicator();

            Show(status);

            workDelegate();

            Hide();
        }

        public async void Show(BusyIndicatorWorkAsyncDelegate workDelegate, string status = "")
        {
            ArgumentNullException.ThrowIfNull(workDelegate);

            InitializeBusyIndicator();

            Show(status);

            await workDelegate();

            Hide();
        }

        public void UpdateStatus(string status)
        {
            InitializeBusyIndicator();

            if (status is null)
            {
                status = string.Empty;
            }

            _lastStatus = status;

            SetStatus(status);
        }

        public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "")
        {
            InitializeBusyIndicator();

            if (currentItem > totalItems)
            {
                Hide();
                return;
            }

            UpdateStatus(string.Format(statusFormat, currentItem.ToString(), totalItems.ToString()));

            ShowBusyIndicator(false);
        }

        public void Hide()
        {
            InitializeBusyIndicator();

            HideBusyIndicator();

            ShowCounter = 0;
        }

        public void Push(string status = "")
        {
            if (ShowCounter <= 0)
            {
                Show(status);
            }
            else
            {
                ShowCounter++;
                UpdateStatus(status);
            }
        }

        public void Pop()
        {
            if (ShowCounter > 0)
            {
                ShowCounter--;
            }

            if (ShowCounter <= 0)
            {
                Hide();
            }
        }
    }
}
