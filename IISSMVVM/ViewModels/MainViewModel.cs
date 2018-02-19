using System;
using System.Windows.Input;

using IISSMVVM.EventHandlers;
using IISSMVVM.Helpers;

using Windows.UI.Xaml.Media.Imaging;
using IISSMVVM.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using IISSMVVM.Views;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace IISSMVVM.ViewModels
{
    public class MainViewModel : Observable
    {
        IMobileServiceTable<UsersUPT> userTableObject = App.MobileService.GetTable<UsersUPT>();
        IMobileServiceTable<Deteccion> detectionTable = App.MobileService.GetTable<Deteccion>();
        IMobileServiceTable<Dispositivos> devicesTable = App.MobileService1.GetTable<Dispositivos>();
        IMobileServiceTable<Empresas> companyTable = App.MobileService.GetTable<Empresas>();
        static readonly string OxfordApiKey = "491eb9cb37e64596b44461fd51abf270";
        public List<Dispositivos> device = new List<Dispositivos>();
        public List<Empresas> company = new List<Empresas>();
        FaceServiceClient faceService = new FaceServiceClient(OxfordApiKey);
        FaceServiceClient faceService1 = new FaceServiceClient(OxfordApiKey);
        public string FaceListId;


        private ICommand _photoTakenCommand;
        private BitmapImage _photo;

        public BitmapImage Photo
        {
            get { return _photo; }
            set { Set(ref _photo, value); }
        }

        public ICommand PhotoTakenCommand => _photoTakenCommand ?? (_photoTakenCommand = new RelayCommand<CameraControlEventArgs>(OnPhotoTakenAsync));
        private async void OnPhotoTakenAsync(CameraControlEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Photo))
            {
                Photo = new BitmapImage(new Uri(args.Photo));

                Image.f

                var requiredFaceAttributes = new FaceAttributeType[]
                    {FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Glasses};

                Face[] faces = await faceService.DetectAsync(stream.AsStream(), returnFaceLandmarks: true, returnFaceAttributes: requiredFaceAttributes);
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
                        /* try
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
                                         await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                         {
                                             //imgCaution.Visibility = Visibility.Visible;
                                             bgAmber.Visibility = Visibility.Collapsed;
                                             bgNormal.Visibility = Visibility.Collapsed;
                                             bgDanger.Visibility = Visibility.Visible;
                                             tbDanger.Visibility = Visibility.Visible;
                                             tbLocation.Visibility = Visibility.Visible;
                                             tbCoordinates.Visibility = Visibility.Visible;
                                             tbStreet.Visibility = Visibility.Visible;
                                             tbTown.Visibility = Visibility.Visible;
                                             tbCountry.Visibility = Visibility.Visible;
                                             tbSubjectInfo.Visibility = Visibility.Visible;
                                             tbName.Visibility = Visibility.Visible;
                                             tbAge.Visibility = Visibility.Visible;
                                             tbDescription.Visibility = Visibility.Visible;
                                             Debug.WriteLine("Usuario encontrado");
                                             MyMap.Visibility = Visibility.Visible;
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
                                         await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                         {
                                             //stackpanelAlert.Width = 550;
                                             //stackpanelAlert.Visibility = Visibility.Visible;
                                             //imgCaution.Visibility = Visibility.Collapsed;
                                             //imgClean.Visibility = Visibility.Visible;
                                             //imgGlasses.Visibility = Visibility.Collapsed;
                                             Debug.WriteLine("Usuario no identificado");
                                             sinth.StartSpeaking(media, "Usuario no identificado");
                                             //imgNoFaces.Visibility = Visibility.Collapsed;
                                         });
                                     }
                                 }
                                 catch (Exception)
                                 {
                                 }
                             }
                             else
                             {
                                 //if the user is wearing glasses, we can't continue eith the process and an alert is shown.
                                 await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                 {
                                     sinth.StartSpeaking(media, "No se puede realizar el proceso con lentes");
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
                         }*/
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
        }
    }
}
