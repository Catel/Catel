namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;
    using System.Threading.Tasks;

    public partial class WindowBView
    {
        public WindowBView()
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
