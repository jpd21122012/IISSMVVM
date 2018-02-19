using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Core;
using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;
using IISSMVVM.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IISSMVVM.Views
{
    public sealed partial class AccountPage : Page
    {
        public List<Bills> Bill;
        public ObservableCollection<Bills> ObjBills;
        private readonly IMobileServiceTable<Address> _addressTableObject = App.MobileService.GetTable<Address>();
        private readonly IMobileServiceTable<Bills> _billsTableObject = App.MobileService.GetTable<Bills>();
        private readonly IMobileServiceTable<CardPayment> _cardTableObject = App.MobileService.GetTable<CardPayment>();
        private readonly ObservableCollection<CardPayment> _cardPaymentObservableCollection;
        private readonly ObservableCollection<Payment> _paymentObservableCollection;
        public ObservableCollection<Bills> SubscriptionObservableCollection;
        public ObservableCollection<Bills> SubscriptionObservableCollection1 = new ObservableCollection<Bills>()
        {
            new Bills()
            {
                Date = DateTimeOffset.Now,
                ExpirationDate = DateTime.Today,
                Name = "P1",
                IdC = 19,
                PaymentType = "card",
                ProductName = "pn1",
                Quantity = 1,
                RFC = "PEDJ"
            }
        };
        public List<Bills> FullList = new List<Bills>();
        public List<Bills> DesktopList = new List<Bills>();
        public List<Bills> MobileList = new List<Bills>();
        public List<Bills> HololensList = new List<Bills>();
        public ObservableCollection<Bills> FullSubscriptionObservableCollection;
        public ObservableCollection<Bills> DesktopSubscriptionObservableCollection;
        public ObservableCollection<Bills> MobileSubscriptionObservableCollection;
        public ObservableCollection<Bills> HololensSubscriptionObservableCollection;
        private ObservableCollection<Bills> _fullCollection;
        private ObservableCollection<Bills> _desktopCollection;
        private ObservableCollection<Bills> _mobileCollection;
        private ObservableCollection<Bills> _hololensCollection;
        private BasicGeoposition _geoPosition;
        public double Latitude;
        public double Longitude;
        private Geolocator _geolocator = null;
        public bool IsRefreshed;
        public static List<Address> Address;
        public static List<CardPayment> CardPayment;
        public AccountViewModel ViewModel { get; } = new AccountViewModel();

        public AccountPage()
        {
            InitializeComponent();
            ObjBills = new ObservableCollection<Bills>();
            _paymentObservableCollection = new ObservableCollection<Payment>();
            _cardPaymentObservableCollection = new ObservableCollection<CardPayment>();
            Bill = new List<Bills>();
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    GetAzureDataBinding();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                try
                {
                    Address = new List<Address>();
                    CardPayment = new List<CardPayment>();
                    if (LoginPage.Company[0].Image != null)
                    {
                        ImgCompany.ImageSource = new BitmapImage(new Uri(LoginPage.Company[0].Image));
                        btnImageButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        CompanyEllipse.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    tbAccountCompanyName.Text = LoginPage.Company[0].Nombre;
                    tbAccountCompanyEmail.Text = LoginPage.Company[0].Correo;
                    tbAccountCompanyNumber.Text = LoginPage.Company[0].Telefono;
                    tbAccountCompanyState.Text = LoginPage.Company[0].Estado;
                    StateAccountViewFontIcon.Foreground = LoginPage.Company[0].Estado == "Active" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });
        }
        private async Task GetAzureDataBinding()
        {
            await StartTracking();
            try
            {
                Bill = await _billsTableObject.Where(billsTableObject =>
                                 billsTableObject.IdC == LoginPage.Company[0].IdC).OrderByDescending(
                    billsTableObject => billsTableObject.Date).ToListAsync();
                foreach (var item in Bill)
                {
                    ObjBills.Add(item);
                }
                int countTransfer = 0, countCard = 0;
                foreach (var item in Bill)
                {
                    if (item.PaymentType == "Transfer")
                    {
                        countTransfer++;
                    }
                    else
                    {
                        countCard++;
                    }
                }
                tbAccountCompanyPayment.Text = countTransfer > countCard ? "Transfer" : "Credit Card";
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            try
            {
                Address = await _addressTableObject.Where(addressTableObject =>
                    addressTableObject.IdC.ToString() == Bill[0].IdC.ToString()).ToListAsync();

                CardPayment = await _cardTableObject.Where(cardTableObject =>
                    cardTableObject.IdC.ToString() == Bill[0].IdC.ToString()).ToListAsync();

                foreach (var item in CardPayment)
                {
                    _cardPaymentObservableCollection.Add(item);
                }

                foreach (var item1 in CardPayment)
                {
                    foreach (var item2 in Address)
                    {
                        if (item2.ASaleNumber == item1.CPSaleNumber && item1.BankName == "Banamex" || item1.BankName == "Banco Azteca" || item1.BankName == "Broxel"
                            || item1.BankName == "Afirme" || item1.BankName == "HSBC" || item1.BankName == "Santander" || item1.BankName == "Scotianbank")
                        {
                            _paymentObservableCollection.Add(new Payment()
                            {
                                id = item1.id,
                                CPSaleNumber = item1.CPSaleNumber,
                                BankName = item1.BankName,
                                CardNumber = item1.CardNumber,
                                ExpirationDate = item1.ExpirationDate,
                                CardHolderName = item1.CardHolderName,
                                ASaleNumber = item2.ASaleNumber,
                                StreetName = item2.StreetName,
                                HouseNumber = item2.HouseNumber,
                                Country = item2.Country,
                                State = item2.State,
                                City = item2.City,
                                ZipCode = item2.ZipCode,
                                Image = "ms-appx:///Assets/MasterCard.png"
                            });
                        }
                        else if (item2.ASaleNumber == item1.CPSaleNumber)
                        {
                            _paymentObservableCollection.Add(new Payment()
                            {
                                id = item1.id,
                                CPSaleNumber = item1.CPSaleNumber,
                                BankName = item1.BankName,
                                CardNumber = item1.CardNumber,
                                ExpirationDate = item1.ExpirationDate,
                                CardHolderName = item1.CardHolderName,
                                ASaleNumber = item2.ASaleNumber,
                                StreetName = item2.StreetName,
                                HouseNumber = item2.HouseNumber,
                                Country = item2.Country,
                                State = item2.State,
                                City = item2.City,
                                ZipCode = item2.ZipCode,
                                Image = "ms-appx:///Assets/Visa.png"
                            });
                        }
                    }
                }

                FullSubscriptionObservableCollection = new ObservableCollection<Bills>();
                DesktopSubscriptionObservableCollection = new ObservableCollection<Bills>();
                MobileSubscriptionObservableCollection = new ObservableCollection<Bills>();
                HololensSubscriptionObservableCollection = new ObservableCollection<Bills>();

                foreach (var item in ObjBills.Where(p => p.Type == "Full"))
                {
                    FullSubscriptionObservableCollection.Add(item);
                    FullList.Add(item);
                }
                foreach (var item in ObjBills.Where(p => p.Type == "Desktop"))
                {
                    DesktopSubscriptionObservableCollection.Add(item);
                    DesktopList.Add(item);

                }
                foreach (var item in ObjBills.Where(p => p.Type == "Mobile"))
                {
                    MobileSubscriptionObservableCollection.Add(item);
                    MobileList.Add(item);

                }
                foreach (var item in ObjBills.Where(p => p.Type == "HoloLens"))
                {
                    HololensSubscriptionObservableCollection.Add(item);
                    HololensList.Add(item);
                }

                if (FullSubscriptionObservableCollection.Count == 0)
                {
                    FullButton.IsEnabled = false;
                }
                if (DesktopSubscriptionObservableCollection.Count == 0)
                {
                    DesktopButton.IsEnabled = false;
                }
                if (MobileSubscriptionObservableCollection.Count == 0)
                {
                    MobileButton.IsEnabled = false;
                }
                if (HololensSubscriptionObservableCollection.Count == 0)
                {
                    HololensButton.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateLocationData(e.Position);
            });
        }

        private async Task UpdateLocationData(Geoposition position)
        {
            if (position != null)
            {
                //Sets the latitude and longitude from the position param
                this.Latitude = position.Coordinate.Point.Position.Latitude;
                this.Longitude = position.Coordinate.Point.Position.Longitude;
                await GetTown(position.Coordinate.Point.Position.Latitude, position.Coordinate.Point.Position.Longitude);
            }
        }

        private async Task StartTracking()
        {
            //this var gets the server request from the Geolocator object
            var accessStatus = await Geolocator.RequestAccessAsync();
            //this switch evaluates the diferent returns of the geolocator status
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    _geolocator = new Geolocator { ReportInterval = 1000 };
                    _geolocator.PositionChanged += OnPositionChanged;
                    break;
            }
        }

        private async Task GetTown(double latitude, double longitude)
        {
            Debug.WriteLine("Entraste al metodo GetTown {0},{1}\n", latitude, longitude);
            try
            {
                BasicGeoposition location = new BasicGeoposition();
                location.Latitude = latitude;
                location.Longitude = longitude;
                //MyMap.Navigate(new Uri("https://www.google.com.mx/maps/@" + location.Longitude + "," + location.Latitude + ",16z", UriKind.Absolute));
                MyMap.NavigateToString("<html><body>" +
                                       "<iframe src = \"https://www.google.com/maps/embed/v1/place?key=AIzaSyA-JYPnXwRQwaF9v6qJNy6czLdfBtnhNOw&q=" + location.Latitude + "," + location.Longitude + "&zoom=18 \" frameborder = \"0\" height = \"400\" style = \"border: 0\" width = \"100%\" iframe = \" \"/>"
                                + "</iframe></body >" +
                                       "</ html>");
                Geopoint pointToReverseGeocode = new Geopoint(location);
                //here we find the map location and the state if the server returns something.
                MapLocationFinderResult result =
                    await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
                if (result.Status == MapLocationFinderStatus.Success)
                {
                    //at this point the complete address is shown on the respective labels.
                    //tbStreet.Text = $"St: {result.Locations[0].Address.Street}";
                    //tbTown.Text = $"Town: {result.Locations[0].Address.Town}";
                    //tbCountry.Text = $"Country: {result.Locations[0].Address.Country}";
                    //Debug.WriteLine("Town: " + result.Locations[0].Address.Town);
                    //Debug.WriteLine("district: " + result.Locations[0].Address.District);
                    //Debug.WriteLine("Country: " + result.Locations[0].Address.Country);
                    //Debug.WriteLine("Street: " + result.Locations[0].Address.Street);
                    //Mapping(latitude, longitude);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static async void UploadBlob(string name, StorageFile file)
        {
            const string accountname = "iiss";
            const string accesskey = "PP26VIXc8E07vdMU3de91dBBtA5YmprSrUxG9QrJzaBBGnhBrg/vtj6uod4kIb2hTvzAy9i3Z/NQGHDf/4CMlQ==";
            try
            {
                //Add NuGet WindowsAzure.Storage
                var creden = new StorageCredentials(accountname, accesskey);
                var acc = new CloudStorageAccount(creden, true);
                var client = acc.CreateCloudBlobClient();
                var cont = client.GetContainerReference("iissimages");
                cont.CreateIfNotExistsAsync();
                cont.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
                //decompose array
                var cblob = cont.GetBlockBlobReference(name);
                var randomAccessStream = await file.OpenReadAsync();
                var stream = randomAccessStream.AsStreamForRead();

                cblob.UploadFromStreamAsync(stream);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR:" + ex);
            }
        }

        private async void BtnImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                UploadBlob(LoginPage.Company[0].IdC + file.FileType, file);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    btnImageButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    CompanyEllipse.Visibility = Windows.UI.Xaml.Visibility.Visible;
                });
                LoginPage.Company[0].Image = "https://iiss.blob.core.windows.net/iissimages/" + LoginPage.Company[0].IdC + file.FileType;
                await LoginPage._companyTableObject.UpdateAsync(LoginPage.Company[0]);
                ImgCompany.ImageSource = new BitmapImage(new Uri(LoginPage.Company[0].Image));
            }
        }

        private void FullButton_OnClick(object sender, RoutedEventArgs e)
        {
            FullButton.Background = new SolidColorBrush(Colors.LightGray);
            DesktopButton.Background = new SolidColorBrush(Colors.Transparent);
            MobileButton.Background = new SolidColorBrush(Colors.Transparent);
            HololensButton.Background = new SolidColorBrush(Colors.Transparent);
            FullViewBox.Visibility = Visibility.Visible;
            DesktopViewBox.Visibility = Visibility.Collapsed;
            MobileViewBox.Visibility = Visibility.Collapsed;
            HololensViewBox.Visibility = Visibility.Collapsed;
        }

        private void DesktopButton_OnClick(object sender, RoutedEventArgs e)
        {
            FullButton.Background = new SolidColorBrush(Colors.Transparent);
            DesktopButton.Background = new SolidColorBrush(Colors.LightGray);
            MobileButton.Background = new SolidColorBrush(Colors.Transparent);
            HololensButton.Background = new SolidColorBrush(Colors.Transparent);
            DesktopViewBox.Visibility = Visibility.Visible;
            FullViewBox.Visibility = Visibility.Collapsed;
            HololensViewBox.Visibility = Visibility.Collapsed;
            MobileViewBox.Visibility = Visibility.Collapsed;
        }

        private void MobileButton_OnClick(object sender, RoutedEventArgs e)
        {
            FullButton.Background = new SolidColorBrush(Colors.Transparent);
            DesktopButton.Background = new SolidColorBrush(Colors.Transparent);
            MobileButton.Background = new SolidColorBrush(Colors.LightGray);
            HololensButton.Background = new SolidColorBrush(Colors.Transparent);
            MobileViewBox.Visibility = Visibility.Visible;
            FullViewBox.Visibility = Visibility.Collapsed;
            HololensViewBox.Visibility = Visibility.Collapsed;
            DesktopViewBox.Visibility = Visibility.Collapsed;
        }

        private void HololensButton_OnClick(object sender, RoutedEventArgs e)
        {
            FullButton.Background = new SolidColorBrush(Colors.Transparent);
            DesktopButton.Background = new SolidColorBrush(Colors.Transparent);
            MobileButton.Background = new SolidColorBrush(Colors.Transparent);
            HololensButton.Background = new SolidColorBrush(Colors.LightGray);
            HololensViewBox.Visibility = Visibility.Visible;
            DesktopViewBox.Visibility = Visibility.Collapsed;
            MobileViewBox.Visibility = Visibility.Collapsed;
            FullViewBox.Visibility = Visibility.Collapsed;
        }

        private void FullGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var full = (GridView)e.OriginalSource;
            var fullItems = (List<Bills>)full.ItemsSource;
            if (fullItems != null)
            {
                var differenceTimeSpan = fullItems.First().ExpirationDate - DateTimeOffset.Now;
                var daysLeft = Convert.ToInt32(differenceTimeSpan.TotalDays);
                ExpirationTextBlock.Text = daysLeft + " days left on this plan, consider next payment.";
            }
        }

        private void FullGridView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void DesktopGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var full = (GridView)e.OriginalSource;
            var desk = (List<Bills>)full.ItemsSource;
            if (desk != null)
            {
                var differenceTimeSpan = desk.First().ExpirationDate - DateTimeOffset.Now;
                var daysLeft = Convert.ToInt32(differenceTimeSpan.TotalDays);
                ExpirationTextBlock.Text = daysLeft + " days left on this plan, consider next payment.";
            }
        }

        private void DesktopGridView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MobileGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var full = (GridView)e.OriginalSource;
            var mobi = (List<Bills>)full.ItemsSource;
            if (mobi != null)
            {
                var differenceTimeSpan = mobi.First().ExpirationDate - DateTimeOffset.Now;
                var daysLeft = Convert.ToInt32(differenceTimeSpan.TotalDays);
                ExpirationTextBlock.Text = daysLeft + " days left on this plan, consider next payment.";
            }
        }

        private void MobileGridView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void HololensGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var full = (GridView)e.OriginalSource;
            var holo = (List<Bills>)full.ItemsSource;
            if (holo != null)
            {
                var differenceTimeSpan = holo.First().ExpirationDate - DateTimeOffset.Now;
                var daysLeft = Convert.ToInt32(differenceTimeSpan.TotalDays);
                ExpirationTextBlock.Text = daysLeft + " days left on this plan, consider next payment.";
            }
        }

        private void HololensGridView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
