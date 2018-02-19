using System;

using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;

namespace IISSMVVM.Views
{
    public sealed partial class AboutPage : Page
    {
        public AboutViewModel ViewModel { get; } = new AboutViewModel();

        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
