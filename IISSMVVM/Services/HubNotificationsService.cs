using System;
using System.Threading.Tasks;

using IISSMVVM.Activation;

using Microsoft.WindowsAzure.Messaging;

using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;

namespace IISSMVVM.Services
{
    // More about adding push notifications to your Windows app at https://docs.microsoft.com/azure/app-service-mobile/app-service-mobile-windows-store-dotnet-get-started-push
    internal class HubNotificationsService : ActivationHandler<ToastNotificationActivatedEventArgs>
    {
        public async Task InitializeAsync()
        {
            // TODO WTS: Set your Hub Name
            var hubName = "IISSMVVM";

            // TODO WTS: Set your DefaultListenSharedAccessSignature
            var accessSignature = "Endpoint=sb://iissmvvm.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nh+ztV+qtQwl6iG4N1kwb+uiGrULq6NWhPyEEppYCjc=";

            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += Channel_PushNotificationReceived;
            var hub = new NotificationHub(hubName, accessSignature);
            var result = await hub.RegisterNativeAsync(channel.Uri);
            if (result.RegistrationId != null)
            {
                // Registration was successful
            }

            // You can also send push notifications from Windows Developer Center targeting your app consumers
            // More details at https://docs.microsoft.com/windows/uwp/publish/send-push-notifications-to-your-apps-customers
        }

        private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            //fragment the inner text to obtain the CompanyName and filter with that.
            //if (args.ToastNotification != null && args.ToastNotification.Content.InnerText!="jpd21122012")
            //{
            //    return ;
            //}
        }

        protected override async Task HandleInternalAsync(ToastNotificationActivatedEventArgs args)
        {
            // TODO WTS: Handle activation from toast notification,
            // Be sure to use the template 'ToastGeneric' in the toast notification configuration XML
            // to ensure OnActivated is called when launching from a Toast Notification sent from Azure
            //
            // More about handling activation at https://docs.microsoft.com/windows/uwp/design/shell/tiles-and-notifications/send-local-toast
            await Task.CompletedTask;
        }
    }
}
