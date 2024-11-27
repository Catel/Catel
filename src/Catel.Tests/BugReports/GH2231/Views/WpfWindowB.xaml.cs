namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Catel.Windows;

    /// <summary>
    /// Interaction logic for WpfWindowB.xaml
    /// </summary>
    public partial class WpfWindowB : System.Windows.Window
    {
        public WpfWindowB()
        {
            InitializeComponent();

            this.SetOwnerWindow();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(5000);

            var parent = Owner;
            parent.Close();
        }
    }
}
