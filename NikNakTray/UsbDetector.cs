using System;
using System.Collections;
using System.Management;
using System.Windows.Forms;

namespace NikNakTray
{
    // Delegate for event handler to handle the device events 
    public delegate void DriveDetectorEventHandler(Object sender, DriveDetectorEventArgs e);

    internal class UsbDetector : IDisposable
    {
        // Win32 constants
        private const int DBT_IPHONE_HANDLE = 7;
        private const int WM_DEVICECHANGE = 0x0219;
        //private GetFriendlyNameListDelegate _mDeleg;
        private delegate ArrayList GetFriendlyNameListDelegate();

        // Handle of the window which receives messages from Windows. This will be a form.
        private IntPtr mRecipientHandle;
        private GetFriendlyNameListDelegate _nameDelegate;
        public event DriveDetectorEventHandler DeviceAttached;

        public UsbDetector()
        {
            var frm = new DetectorForm(this);
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
                    case DBT_IPHONE_HANDLE:
                         
                        //_nameDelegate = new GetFriendlyNameListDelegate(GetFriendlyNameListBegin);
                        //AsyncCallback callback = GetFriendlyNameListEnd;

                        // invoke the thread that will handle getting the friendly names  
                        //_nameDelegate.BeginInvoke(callback, new object());
                        var properties = GetFriendlyNameListBegin();
                        if(properties.Count > 0)
                        {
                            DeviceAttached(this, new DriveDetectorEventArgs());
                        }
                        break;
                }
            }
        }

        //public void MyTaskAsync(string[] files)
        //{
        //    MyTaskWorkerDelegate worker = new MyTaskWorkerDelegate(MyTaskWorker);
        //    AsyncCallback completedCallback = new AsyncCallback(MyTaskCompletedCallback);

        //    lock (_sync)
        //    {
        //        if (_myTaskIsRunning)
        //            throw new InvalidOperationException("The control is currently busy.");

        //        AsyncOperation async = AsyncOperationManager.CreateOperation(null);
        //        worker.BeginInvoke(files, completedCallback, async);
        //        _myTaskIsRunning = true;
        //    }
        //}

        //private readonly object _sync = new object();

        // function queries the system using WMI and gets an arraylist of all com port devices     
        private ArrayList GetFriendlyNameListBegin()
        {
            var deviceList = new ArrayList();

            var searcher = new ManagementObjectSearcher("Select * from Win32_PnpEntity");

            foreach (var devices in searcher.Get())
            {
                var nameProperty = devices.GetPropertyValue("Name");

                if (nameProperty == null)
                {
                    continue;
                }

                if (nameProperty.ToString().ToLower().Contains("usb") &&
                    nameProperty.ToString().ToLower().Contains("abbott"))
                {
                    deviceList.Add(devices);
                }
            }

            searcher.Dispose();
            return deviceList;
        }

        private void GetFriendlyNameListEnd(IAsyncResult ar)
        {
            // got the returned arrayList, now we can do whatever with it  
            var result = _nameDelegate.EndInvoke(ar);

            if (result.Count > 0)
            {
                DeviceAttached(this, new DriveDetectorEventArgs());
            }
        }
    }
}