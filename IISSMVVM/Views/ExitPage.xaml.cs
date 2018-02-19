using System;

using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;

namespace IISSMVVM.Views
{
    public sealed partial class ExitPage : Page
    {
        public ExitViewModel ViewModel { get; } = new ExitViewModel();

        public ExitPage()
        {
            InitializeComponent();
        }
    }
}
