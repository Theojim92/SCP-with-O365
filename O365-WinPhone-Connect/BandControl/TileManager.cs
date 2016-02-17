using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band.Tiles.Pages;
using Microsoft.Band.Tiles;
using NativeTileManager = Microsoft.Band.Tiles.IBandTileManager;
using NativeTileButtonPressedEventArgs = Microsoft.Band.Tiles.BandTileEventArgs<Microsoft.Band.Tiles.IBandTileButtonPressedEvent>;
using NativeTileOpenedEventArgs = Microsoft.Band.Tiles.BandTileEventArgs<Microsoft.Band.Tiles.IBandTileOpenedEvent>;
using NativeTileClosedEventArgs = Microsoft.Band.Tiles.BandTileEventArgs<Microsoft.Band.Tiles.IBandTileClosedEvent>;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class TileManager
    {
        private readonly BandClient client;
        private readonly object subscribedLock = new object();
        internal readonly NativeTileManager Native;

        internal TileManager(BandClient client, NativeTileManager tileManager)
        {
            this.Native = tileManager;
            this.client = client;
           // this.Native.TileButtonPressed += OnNativeTileButtonPressed;
           // this.Native.TileOpened += OnNativeTileOpened;
           // this.Native.TileClosed += OnNativeTileClosed;
        }

        public async Task<bool> AddTileAsync(BandTile tile)
        {
            bool result = false;
            result = await Native.AddTileAsync(tile);

            return result;
        } 
        public async Task RemoveTileAsync(Guid tileId)
        {
            await Native.RemoveTileAsync(tileId);
        }
        public async Task RemoveTilePagesAsync(Guid tileId)
        {
            await Native.RemovePagesAsync(tileId);
        }
       // public Task SetTilePageDataAsync(Guid tileId, params PageData[] pageData){   }

        public async Task StartEventListenersAsync()
        {
            await Native.StartReadingsAsync();
        }
        public async Task StopEventListenersAsync()
        {
            await Native.StopReadingsAsync();
        }
    }
}
