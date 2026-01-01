namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;
    using Catel.Services;

    public partial class WindowAView
    {
        public WindowAView(IServiceProvider serviceProvider, IWrapControlService wrapControlService,
            ILanguageService languageService)
            : base(serviceProvider, wrapControlService, languageService)
        {
            InitializeComponent();
        }
    }
}
