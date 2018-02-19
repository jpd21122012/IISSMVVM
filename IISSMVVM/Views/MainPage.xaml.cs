using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using IISSMVVM.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using IISSMVVM.Models;
using IISSMVVM.Services;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace IISSMVVM.Views
{
    public sealed partial class MainPage : Page
    {
        IMobileServiceTable<UsersUPT> userTableObject = App.MobileService.GetTable<UsersUPT>();
        IMobileServiceTable<Deteccion> detectionTable = App.MobileService.GetTable<Deteccion>();
        IMobileServiceTable<Dispositivos> devicesTable = App.MobileService1.GetTable<Dispositivos>();
        IMobileServiceTable<Empresas> companyTable = App.MobileService.GetTable<Empresas>();
        static readonly string OxfordApiKey = "491eb9cb37e64596b44461fd51abf270";
        public List<Dispositivos> device = new List<Dispositivos>();
        public List<Empresas> company = new List<Empresas>();
        public static FaceServiceClient faceService = new FaceServiceClient(OxfordApiKey);
        public static FaceServiceClient faceService1 = new FaceServiceClient(OxfordApiKey);
        public string FaceListId;
        public static Face[] faces;
        public static char[] ch;
        public static Stream st;
        public static StorageFile file;
        public static int DetectionId;
        public static string nombreguardia;
        public static string sectorguardia;
        private string imageName;
        public MainViewModel ViewModel { get; } = new MainViewModel();
        private TextBlock _tbName;

        public TextBlock TbName
        {
            get => _tbName;
            set => _tbName = value;
        }
        public MainPage()
        {
            InitializeComponent();
        }
        public MainPage(bool x)
        {
            if (x)
            {
                ProcessAll();
            }
        }
        public async void ProcessAll()
        {
            imageName = Guid.NewGuid() + ".jpg";
            using (FileStream stream = File.Open(file.Path, FileMode.Open))
            {
                var requiredFaceAttributes = new[]
{FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Glasses};
                try
                {
                    faces = await faceService.DetectAsync(stream, returnFaceLandmarks: true, returnFaceAttributes: requiredFaceAttributes);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }
            //Now we will be able to use the API clients to detect the face.
            try
            {
                //if a face exists we can proceed with the comparison into the database.
                if (faces.Length >= 1)
                {
                    Debug.WriteLine("ID de rostro: " + faces[0].FaceId);
                    Guid idGuid = Guid.Parse(faces[0].FaceId.ToString());
                    //Here we get the range of simility respect our data on the Azure Cloud.
                    SimilarPersistedFace[] facescomp = await faceService1.FindSimilarAsync(idGuid, LoginPage.Company[0].FaceListId, 1);
                    double confidence = Double.Parse(facescomp[0].Confidence.ToString());
                    string persistentID = facescomp[0].PersistedFaceId.ToString();
                    Debug.WriteLine("PID: " + facescomp[0].PersistedFaceId);
                    Debug.WriteLine("conf: " + facescomp[0].Confidence);
                    string lentes = faces[0].FaceAttributes.Glasses.ToString();
                    try
                    {
                        //once we have the face data, if the user in not wearing glasses we can continue.
                        if (lentes == "NoGlasses")
                        {
                            try
                            {
                                //on the range obtained we define a limit of positive or negative.
                                if (confidence >= .67)
                                {
                                    //Here the user is a criminal and we obtain the location and the alert will appear.
                                    //await StartTracking();
                                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                    {
                                        //imgCaution.Visibility = Visibility.Visible;
                                        //bgAmber.Visibility = Visibility.Collapsed;
                                        //bgNormal.Visibility = Visibility.Collapsed;
                                        //bgDanger.Visibility = Visibility.Visible;
                                        //tbDanger.Visibility = Visibility.Visible;
                                        //tbLocation.Visibility = Visibility.Visible;
                                        //tbCoordinates.Visibility = Visibility.Visible;
                                        //tbStreet.Visibility = Visibility.Visible;
                                        //tbTown.Visibility = Visibility.Visible;
                                        //tbCountry.Visibility = Visibility.Visible;
                                        //tbSubjectInfo.Visibility = Visibility.Visible;
                                        //tbName.Visibility = Visibility.Visible;
                                        //tbAge.Visibility = Visibility.Visible;
                                        //tbDescription.Visibility = Visibility.Visible;
                                        Debug.WriteLine("Usuario encontrado");
                                        //MyMap.Visibility = Visibility.Visible;
                                        //we do a query to search all the information about the criminal that finally is shown on the UI.
                                        Query(facescomp[0].PersistedFaceId.ToString());
                                    });
                                    //sinth.StartSpeaking(media, "Nombre:,   ,   ,   ,   ," +(tbName.Text) + "Edad:,   ,   ,   ,   ," + (tbName.Text) +
                                    //"Descripcion:,   ,   ,   ,   ," +tbDescription.Text);
                                    Debug.WriteLine(tbName.Text + "\n" + tbName.Text + "\n" + tbDescription.Text);
                                    //Video Stream
                                    //await StartListener();
                                    //await BeginRecording();
                                    //Mapping();
                                }
                                else
                                {
                                    //if the user is not into our range, it's a negative, so it's clean.
                                    //await new CoreDispatcher(null).RunAsync(CoreDispatcherPriority.Normal, () =>
                                    //{
                                    //stackpanelAlert.Width = 550;
                                    //stackpanelAlert.Visibility = Visibility.Visible;
                                    //imgCaution.Visibility = Visibility.Collapsed;
                                    //imgClean.Visibility = Visibility.Visible;
                                    //imgGlasses.Visibility = Visibility.Collapsed;
                                    Debug.WriteLine("Usuario no identificado");
                                    //sinth.StartSpeaking(media, "Usuario no identificado");
                                    //imgNoFaces.Visibility = Visibility.Collapsed;
                                    //});
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            //if the user is wearing glasses, we can't continue eith the process and an alert is shown.
                            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                //sinth.StartSpeaking(media, "No se puede realizar el proceso con lentes");
                                //stackpanelAlert.Width = 616;
                                //stackpanelAlert.Visibility = Visibility.Visible;
                                //imgCaution.Visibility = Visibility.Collapsed;
                                //imgClean.Visibility = Visibility.Collapsed;
                                //imgGlasses.Visibility = Visibility.Visible;
                                //imgNoFaces.Visibility = Visibility.Collapsed;
                            });
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            catch (Exception eex)
            {
                //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                //{
                //    //stackpanelAlert.Width = 550;
                //    //stackpanelAlert.Visibility = Visibility.Visible;
                //    //imgCaution.Visibility = Visibility.Collapsed;
                //    //imgClean.Visibility = Visibility.Visible;
                //    //imgGlasses.Visibility = Visibility.Collapsed;
                //    Debug.WriteLine("Usuario no identificado");
                //    sinth.StartSpeaking(media, "Usuario no identificado");
                //    //imgNoFaces.Visibility = Visibility.Collapsed;
                //});
            }
        }
        public static void idDataReadyM1()
        {
            new MainPage().ProcessAll();
        }
        private async void Query(string searchId)
        {
            List<UsersUPT> lista = new List<UsersUPT>();
            Debug.WriteLine("El id a buscar es: " + searchId);
            try
            {
                //In this part we do a linq query to get the correct information.
                lista = await userTableObject.Where(userTableObj => userTableObj.PID == searchId).ToListAsync();
                var obj = lista.First();
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _tbName = new TextBlock();
                    //tbName.Text = $"Name: {obj.nombre}";
                    _tbName.Text = $"Name: {obj.nombre}";
                    //tbAge.Text = $"Age: {obj.edad}";
                    //tbDescription.Text = $"Description: {obj.descripcion}";
                    //tbDanger.Text = $"Priority: {obj.priority}";
                    //if (obj.priority == "Low")
                    //{
                    //    imgDanger1.Visibility = Visibility.Visible;
                    //}
                    //else if ((obj.priority == "Medium"))
                    //{
                    //    imgDanger2.Visibility = Visibility.Visible;

                    //}
                    //else if (obj.priority == "High")
                    //{
                    //    imgDanger3.Visibility = Visibility.Visible;

                    //}
                });
                //        sinth.StartSpeaking(media, "Nombre: ,    ,    ,    ,    ,    ,    , " + obj.nombre + ",   ,   ,   " +
                //",   ,   ,Edad:   ,   ,   ,   ,   ," + obj.edad + " años,   ,   ,   ,   ,Descripcion:,   ,   ,   ,   ,   ," + obj.descripcion);
                //        device = await devicesTable.Where(devicesTable => devicesTable.Nombre == GetDeviceInfo.DeviceName).ToListAsync();
                //        var deviceObj = device.First();
                UploadBlob(imageName);
                //enviar datos a la bd
                try
                {
                    await InsertTodoItem(new Deteccion
                    {
                        IdC = DetectionId,
                        Hora = DateTime.Now.ToString(),
                        Imagen = imageName,
                        //Latitud = latitude.ToString(),
                        //Longitud = longitude.ToString(),
                        NombreCriminal = obj.nombre,
                        EdadCriminal = obj.edad,
                        DescripcionCriminal = obj.descripcion,
                        NombreGuardia = LoginPage.Device[0].Guardia,
                        Sectorguardia = LoginPage.Device[0].Sector,
                        Dispositivo = GetDeviceInfo.DeviceName,
                        IdDisp = LoginPage.Device[0].IdDisp,
                        Status = "Pending"
                    });
                }
                catch (Exception InsertDetection)
                {
                    Debug.WriteLine(InsertDetection.Message.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.Write("Error: " + ex);
            }
        }
        public void UploadBlob(string imageName)
        {
            string accountname = "enigmaimages";
            string accesskey = "k1Kd0FOWR0N2ALo4Be/eo9VOz1dcoU0+XICAGBsrh3Owblsa9/anXqo3V1ahRzD+tpoFzSxTqwn+cyalFHGq0Q==";
            try
            {
                StorageCredentials creden = new StorageCredentials(accountname, accesskey);
                CloudStorageAccount acc = new CloudStorageAccount(creden, useHttps: true);
                CloudBlobClient client = acc.CreateCloudBlobClient();
                CloudBlobContainer cont = client.GetContainerReference("testblob");
                cont.CreateIfNotExistsAsync();
                cont.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                CloudBlockBlob cblob = cont.GetBlockBlobReference(imageName);
                //cblob.UploadFromStreamAsync(face);
                cblob.UploadFromFileAsync(file);
                //using (Stream file = File.OpenRead(path))
                //{
                //    cblob.UploadFromStreamAsync(file);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR UploadBlob():" + ex.ToString());
            }
        }
        private async Task InsertTodoItem(Deteccion todoItem)
        {
            // This code inserts a new TodoItem into the database. After the operation completes
            // and the mobile app backend has assigned an id, the item is added to the CollectionView.
            try
            {
                await detectionTable.InsertAsync(todoItem);
            }
            catch (Exception)
            {
            }
        }
        private void MyMap_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
