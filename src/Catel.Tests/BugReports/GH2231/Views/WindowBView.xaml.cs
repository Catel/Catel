namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;
    using System.Threading.Tasks;
    using Catel.Services;

    public partial class WindowBView
    {
        public WindowBView(IServiceProvider serviceProvider, IWrapControlService wrapControlService, 
            ILanguageService languageService)
            : base(serviceProvider, wrapControlService, languageService)
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            Dispatcher.BeginInvoke(async () =>
            {
                await Task.Delay(5000);

                var parent = Owner;
                parent.Close();
            });
        }
    }
}
