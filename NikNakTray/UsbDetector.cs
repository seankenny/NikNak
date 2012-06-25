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
        
        private const int WM_DEVICECHANGE = 0x0219; // attached/removed
        public const int DEVICE_ARRIVAL = 0x8000;  // device attached
        public const int DEVICE_REMOVECOMPLETE = 0x8004;  // device removed

        // Handle of the window which receives messages from Windows. This will be a form.
        private IntPtr mRecipientHandle;
        public event UsbDetectorEventHandler OnDeviceAttached;
        public event UsbDetectorEventHandler OnDeviceRemoved;

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
                    case DEVICE_ARRIVAL:
                        if (OnDeviceAttached != null)
                        {
                            var device = GetConnectedDevice();
                            OnDeviceAttached(this, new UsbDetectorEventArgs(device));
                        }
                        break;

                    case DEVICE_REMOVECOMPLETE:
                        if (OnDeviceRemoved != null)
                            OnDeviceRemoved(this, new UsbDetectorEventArgs());
                        break;
                }
            }
        }

        // function queries the system using WMI and returns the relevant device
        private ManagementBaseObject GetConnectedDevice()
        {
            //HIDDevice.interfaceDetails[] devices = HIDDevice.getConnectedDevices(); 
            ComPort cp = new ComPort();
            cp.Initialise();

            using(var searcher = new ManagementObjectSearcher("Select * from Win32_PnpEntity WHERE PNPDeviceID LIKE '%VID[_]1A61&PID[_]3420%'"))
            {
                foreach (var device in searcher.Get())
                {
                    var nameProperty = device.GetPropertyValue("Name");
                    var vendorId = device.GetPropertyValue("PNPDeviceID");
                    var deviceId = device.GetPropertyValue("DeviceID");

                    if (nameProperty == null)
                    {
                        continue;
                    }



                    return device;
                }
            }
            return null;
        }
    }
}