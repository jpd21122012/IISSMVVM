using System;

using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IISSMVVM.Views
{
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to http://enigma-mx.azurewebsites.net/privacyPolicy
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Initialize();
        }
    }
}
