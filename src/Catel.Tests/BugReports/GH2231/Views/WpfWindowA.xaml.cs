namespace Catel.Tests.BugReports.GH2231.Views
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for WpfWindowA.xaml
    /// </summary>
    public partial class WpfWindowA : Window
    {
        public WpfWindowA()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);

            var windowB = new WpfWindowB();
            windowB.ShowDialog();
        }
    }
}
