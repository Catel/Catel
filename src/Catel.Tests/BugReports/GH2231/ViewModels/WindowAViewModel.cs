namespace Catel.Tests.BugReports.GH2231.ViewModels
{
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Services;

    public class WindowAViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;

        public WindowAViewModel(IUIVisualizerService uiVisualizerService)
        {
            _uiVisualizerService = uiVisualizerService;
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _ = _uiVisualizerService.ShowDialogAsync<WindowBViewModel>();
        }
    }
}
