using Microsoft.Toolkit.Uwp.Notifications;

using Windows.UI.Notifications;

namespace IISSMVVM.Services
{
    internal partial class ToastNotificationsService
    {
        public void ShowToastNotificationSample()
        {
            // Create the toast content
            var content = new ToastContent()
            {
                // More about the Launch property at https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "Intelligent Identificator Security System"
                            },

                            new AdaptiveText()
                            {
                                 Text = @"Welcome to IISS, now you will be able to interact with the System, please be careful to read the Users Guide for better performance."
                            }
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // More about Toast Buttons at https://developer.microsoft.com/en-us/windows/uwp-community-toolkit/api/microsoft_toolkit_uwp_notifications_toastbutton
                        new ToastButton("OK", "ToastButtonActivationArguments")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        new ToastButtonDismiss("Cancel")
                    }
                }
            };

            // Add the content to the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
                // More details at https://docs.microsoft.com/en-gb/uwp/api/windows.ui.notifications.toastnotification#Windows_UI_Notifications_ToastNotification_Tag
                Tag = "jpd21122012"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
