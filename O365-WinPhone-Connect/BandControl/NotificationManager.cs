using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band.Notifications;
using NativeNotificationManager = Microsoft.Band.Notifications.IBandNotificationManager;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class NotificationManager
    {
        private readonly BandClient client;
        
        internal readonly NativeNotificationManager Native;
        internal NotificationManager(BandClient client, NativeNotificationManager notificationManager)
        {
            this.Native = notificationManager;
            this.client = client;
        }
        public async Task SendMessageAsync(Guid tileId, string title, string body, DateTime timestamp, bool showDialog)
        {
            await SendMessageAsync(tileId, title, body, timestamp, showDialog);
        }
        public async Task ShowDialogAsync(Guid tileId, string title, string body)
        {
            await Native.ShowDialogAsync(tileId, title, body);
        }
        public async Task VibrateAsync(VibrationType vibrationType)
        {
            await Native.VibrateAsync(vibrationType);
        }
    }
}
