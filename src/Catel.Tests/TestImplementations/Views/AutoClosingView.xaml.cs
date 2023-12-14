namespace Catel.Tests.Views
{
    using System;

    public partial class AutoClosingView
    {
        public AutoClosingView()
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
