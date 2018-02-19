using Windows.UI.Xaml;
using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;

namespace IISSMVVM.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private void MyMap_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
