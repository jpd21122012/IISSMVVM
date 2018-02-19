using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Popups;
using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using IISSMVVM.Models;
using IISSMVVM.Services;
using Microsoft.WindowsAzure.MobileServices;

namespace IISSMVVM.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginViewModel ViewModel { get; } = new LoginViewModel();
        IMobileServiceTable<Login> userTableObject = App.MobileService.GetTable<Login>();
        public static readonly IMobileServiceTable<Empresas> _companyTableObject = App.MobileService.GetTable<Empresas>();
        private readonly IMobileServiceTable<Dispositivos> _devicesTable = App.MobileService1.GetTable<Dispositivos>();
        public static List<Empresas> Company;
        public static List<Dispositivos> Device;
        GetDeviceInfo info = new GetDeviceInfo();
        public LoginPage()
        {
            InitializeComponent();
            info.Info();
            Company = new List<Empresas>();
            Device = new List<Dispositivos>();
            GetAzureDataBinding();
        }
        private async void GetAzureDataBinding()
        {
            try
            {
                Device = await _devicesTable.Where(devicesTable => devicesTable.Nombre == GetDeviceInfo.DeviceName).ToListAsync();
                Company = await _companyTableObject.Where(companyTableObject => companyTableObject.IdC == Device[0].IdC).ToListAsync();
                System.Diagnostics.Debug.WriteLine(Device[0].IdC);
                System.Diagnostics.Debug.WriteLine(Company[0].FaceListId);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
        string Encrypt(string pass)
        {
            MD5 md5 = MD5.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(pass));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        private async void Query(string user, string pass)
        {
            List<Login> lista = new List<Login>();
            try
            {
                //In this part we do a linq query to get the correct information.
                lista = await userTableObject.Where(userTableObj => userTableObj.User == user && userTableObj.Password == pass).ToListAsync();
                var obj = lista[0];
                var messageDialog = new MessageDialog("Welcome back " + obj.User + " Tu id es: " + obj.IdC, " Intelligent Identificator Security System");
                messageDialog.Commands.Add(new UICommand("Continue"));
                await messageDialog.ShowAsync();
                if (lista.Count == 1)
                {
                    //Frame.Navigate(typeof(MainPageView));
                    //MainPageView.DetectionId = Convert.ToInt32(obj.IdC);
                }
            }
            catch (Exception ex)
            {
                var messageDialog = new MessageDialog("Sorry you're not registered");
                messageDialog.Commands.Add(new UICommand("Continue"));
                await messageDialog.ShowAsync();
                ClearTextBoxes();
            }
        }
        void ClearTextBoxes()
        {
            TbUser.Text = "";
            TbPass.Password = "";
        }
        private void LoginBtn_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Query(TbUser.Text, Encrypt(TbPass.Password));
        }
    }
}
