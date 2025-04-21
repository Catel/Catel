namespace Catel.Tests.Views
{
    using System;
    using Catel.Services;

    public partial class AutoClosingView
    {
        public AutoClosingView(IServiceProvider serviceProvider, IWrapControlService wrapControlService,
            ILanguageService languageService)
            : base(serviceProvider, wrapControlService, languageService)
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            Close();
        }
    }
}
