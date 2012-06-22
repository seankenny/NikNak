using System;
using System.Management;

namespace NikNakTray
{
    public class UsbDetectorEventArgs : EventArgs
    {
        public ManagementBaseObject device;

        public UsbDetectorEventArgs()
        {
            this.device = null;
        }

        public UsbDetectorEventArgs(ManagementBaseObject device)
        {
            this.device = device;
        }
    }
}