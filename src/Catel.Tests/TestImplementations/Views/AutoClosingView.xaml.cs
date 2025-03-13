namespace Catel.Tests.Views
{
    using System;

    public partial class AutoClosingView
    {
        public AutoClosingView()
            : base(IServiceProvider serviceProvider)
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
