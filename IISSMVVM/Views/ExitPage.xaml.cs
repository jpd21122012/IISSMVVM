using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;
using IISSMVVM.Models;
using IISSMVVM.Services;
using Microsoft.WindowsAzure.MobileServices;

namespace IISSMVVM.Views
{
    public sealed partial class ExitPage : Page
    {
        private readonly IMobileServiceTable<Dispositivos> _devicesTable = App.MobileService1.GetTable<Dispositivos>();
        public static Dispositivos deviceObj;
        public ExitViewModel ViewModel { get; } = new ExitViewModel();

        public ExitPage()
        {
            InitializeComponent();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Offline();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        public async Task Offline()
        {
            try
            {
                var x = await _devicesTable.Where(devicesTable => devicesTable.Nombre == GetDeviceInfo.DeviceName).ToListAsync();
                deviceObj = x.First();
                deviceObj.isActive = "0";
                deviceObj.Checksum = 48;
                await _devicesTable.UpdateAsync(deviceObj);
                Debug.WriteLine("Name: " + deviceObj.Nombre);
                Application.Current.Exit();
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }
}
