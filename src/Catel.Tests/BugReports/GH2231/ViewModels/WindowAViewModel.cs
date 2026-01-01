namespace Catel.Tests.BugReports.GH2231.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Services;

    public class WindowAViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;

        public WindowAViewModel(IServiceProvider serviceProvider, IUIVisualizerService uiVisualizerService)
            : base(serviceProvider)
        {
            _uiVisualizerService = uiVisualizerService;
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await Task.Delay(1000);

            _ = _uiVisualizerService.ShowDialogAsync<WindowBViewModel>();
        }
    }
}
