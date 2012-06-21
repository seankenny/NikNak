using System;

namespace NikNakTray
{
    public class DriveDetectorEventArgs : EventArgs
    {
        public bool Cancel;
        public string Drive;
        public bool HookQueryRemove;

        public DriveDetectorEventArgs()
        {
            Cancel = false;
            Drive = string.Empty;
            HookQueryRemove = false;
        }
    }
}