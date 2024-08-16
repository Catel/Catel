namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;

    public partial class WindowBView
    {
        public WindowBView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            Dispatcher.BeginInvoke(() =>
            {
                var parent = this.Owner;
                parent.Close();
            });
        }
    }
}
