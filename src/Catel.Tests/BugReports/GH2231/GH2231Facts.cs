namespace Catel.Tests.BugReports.GH2231
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Tests.BugReports.GH2231.ViewModels;
    using Catel.Tests.BugReports.GH2231.Views;
    using NUnit.Framework;

    [TestFixture, Explicit("UI Tests")]
    public class GH2231Facts
    {
        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public async Task Closes_Windows_When_Child_Window_Closes_Parent_Window_First_Async()
        {
            var application = Application.Current;
            if (application is null)
            {
                application = new Application();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }

            var viewLocator = new ViewLocator();
            var dispatcherProviderService = new DispatcherProviderService();
            var dispatcherService = new DispatcherService(dispatcherProviderService);

            var uiVisualizerService = new UIVisualizerService(viewLocator, dispatcherService);

            await uiVisualizerService.ShowDialogAsync<WindowAViewModel>(new UIVisualizerContext
            {
                IsModal = true
            });

            Debug.WriteLine("Success");
        }

        [Test, Apartment(System.Threading.ApartmentState.STA)]
        public async Task Wpf_Default_Behavior_Async()
        {
            var application = Application.Current;
            if (application is null)
            {
                application = new Application();
                application.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }

            var windowA = new WpfWindowA();
            windowA.ShowDialog();

            Debug.WriteLine("Success");
        }
    }
}
