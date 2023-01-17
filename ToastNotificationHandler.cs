using Notifications.Wpf;
using System;

namespace Delfinovin
{
    /// <summary>
    /// A class providing the functionality to send simple toast notifications.
    /// </summary>
    public static class ToastNotificationHandler
    {
        /// <summary>
        /// Send a toast notification 
        /// </summary>
        /// <param name="title">The titlebar of the Toast notification.</param>
        /// <param name="message">The message content of the Toast notification.</param>
        /// <param name="type">The type of content being sent in the notification.</param>
        public static void ShowNotification(string title = "", string message = "", NotificationType type = NotificationType.Information)
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = message,
                Type = type
            });
        }
    }
}
