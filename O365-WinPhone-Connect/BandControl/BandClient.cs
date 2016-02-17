
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Tiles;
using Microsoft.Band.Notifications;
using NativeBandClient = Microsoft.Band.IBandClient;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class BandClient
    {
        private readonly Lazy<TileManager> tile;
        private readonly Lazy<NotificationManager> notification;

        private BandClient()
        {
        }

        internal NativeBandClient Native;
        internal BandClient(NativeBandClient client)
        {
            this.Native = client;
            this.tile = new Lazy<TileManager>(() => new TileManager(this, Native.TileManager));
            this.notification = new Lazy<NotificationManager>(() => new NotificationManager(this, Native.NotificationManager));
        }
        public TileManager TileManage
        {
            get
            {
                CheckDisposed();

                return tile.Value;
            }
        }
        public NotificationManager NotificationManage
        {
            get
            {
                CheckDisposed();

                return notification.Value;
            }
        }

        //Get a value indicating whether this instance is connected to a Band device
        public bool IsConnected
        {
            get
            {
                CheckDisposed();

                return true;
            }
        }

        //Disconnects from current Band device.
        public void Disconnect()
        {
            CheckDisposed();
            var nativeClient = Native;
            Native = null;
            nativeClient.Dispose();
        }
        private void CheckDisposed()
        {
            if(Native == null)
            {
                throw new ObjectDisposedException(null);
            }
        }
    }
}
