using System;

using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;

namespace IISSMVVM.Views
{
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel ViewModel { get; } = new AccountViewModel();

        public AccountPage()
        {
            InitializeComponent();
        }
    }
}
