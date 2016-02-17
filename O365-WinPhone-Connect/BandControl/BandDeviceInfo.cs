using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeBandDeviceInfo = Microsoft.Band.IBandInfo;

namespace Menu_and_Topbar_app__Windows_Phone_8._1_3.BandControl
{
    public class BandDeviceInfo
    {
        private BandDeviceInfo()
        { }
        internal readonly NativeBandDeviceInfo Native;
        internal BandDeviceInfo(NativeBandDeviceInfo info)
        {
            this.Native = info;
        }

        public string Name
        {
            get
            {
                return Native.Name;
            }
        }
    }
}
