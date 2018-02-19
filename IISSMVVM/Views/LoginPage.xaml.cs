using System;

using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;

namespace IISSMVVM.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; } = new LoginViewModel();

        public LoginPage()
        {
            InitializeComponent();
        }
    }
}
