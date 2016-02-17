using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using NativeBandClientManager = Microsoft.Band.BandClientManager;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class BandClientManager
    {
        private static Lazy<BandClientManager> instance;

        static BandClientManager()
        {
            instance = new Lazy<BandClientManager>(() => new BandClientManager());
        }

        public static BandClientManager Instance
        {
            get { return instance.Value;  }
        }

        public async Task<IEnumerable<BandDeviceInfo>> GetPairedBandAsync()
        {
            return (await NativeBandClientManager.Instance.GetBandsAsync()).Select(b => new BandDeviceInfo(b));
        }

        public async Task<BandClient> ConnectAsync(BandDeviceInfo info)
        {
            var nativeClient = await NativeBandClientManager.Instance.ConnectAsync(info.Native);
            return new BandClient(nativeClient);
        }
    }
}
