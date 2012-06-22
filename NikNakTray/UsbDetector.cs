using System;
using System.Collections;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NikNakTray
{
    // Delegate for event handler to handle the device events 
    public delegate void UsbDetectorEventHandler(Object sender, UsbDetectorEventArgs e);

    internal class UsbDetector : IDisposable
    {
        // Win32 constants
        private const int DBT_USB_HANDLE = 7;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 
        private const int WM_DEVICECHANGE = 0x0219;

        private delegate ArrayList GetFriendlyNameListDelegate();

        // Handle of the window which receives messages from Windows. This will be a form.
        private IntPtr mRecipientHandle;
        public event UsbDetectorEventHandler DeviceAttached;
        public event UsbDetectorEventHandler DeviceRemoved;

        public UsbDetector()
        {
            var frm = new UsbDetectorForm(this);
            frm.Show(); // will be hidden immediatelly
            mRecipientHandle = frm.Handle;
        }

        public void Dispose()
        {
        }

        public void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                // WM_DEVICECHANGE can have several meanings depending on the WParam value...
                switch (m.WParam.ToInt32())
                {
                    case DBT_USB_HANDLE:
                        var device = GetConnectedDevice();
                        if (device != null && DeviceAttached != null)
                        {
                            DeviceAttached(this, new UsbDetectorEventArgs(device));
                        }
                        break;

                    case DBT_DEVICEREMOVECOMPLETE:
                        if (DeviceRemoved != null)
                            DeviceRemoved(this, new UsbDetectorEventArgs());
                        break;
                }
            }
        }

        // function queries the system using WMI and returns the relevant device
        private ManagementBaseObject GetConnectedDevice()
        {
            using(var searcher = new ManagementObjectSearcher("Select * from Win32_PnpEntity"))
            {
                foreach (var device in searcher.Get())
                {
                    var nameProperty = device.GetPropertyValue("Name");

                    if (nameProperty == null)
                    {
                        continue;
                    }

                    if (nameProperty.ToString().ToLower().Contains("usb")
                        && nameProperty.ToString().ToLower().Contains("abbott"))
                    {
                        return device;
                    }
                }
            }
            return null;
        }
    }
}