using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using NikNakTray.Properties;

namespace NikNakTray
{
    internal class UsbContext : ApplicationContext
    {
        private IContainer components;
        private NotifyIcon notifyIcon;
        private Window aboutForm;
        private Window syncForm;
        private Window deviceConnectedForm;
        private readonly UsbDetector usbDetector;

        public UsbContext()
        {
            this.InitializeContext();
            
            this.usbDetector = new UsbDetector();
            this.usbDetector.DeviceAttached += this.OnUsbDeviceAttached;
            this.usbDetector.DeviceRemoved += this.OnUsbDeviceRemoved;
        }

        private void OnUsbDeviceAttached(object sender, UsbDetectorEventArgs e)
        {
            if (deviceConnectedForm == null)
            {
                deviceConnectedForm = new DeviceConnected();
                deviceConnectedForm.Closed += (a, b) => { deviceConnectedForm = null; };
                deviceConnectedForm.Show();
            }
            else
            {
                deviceConnectedForm.Activate();
            }
        }

        private void OnUsbDeviceRemoved(object sender, UsbDetectorEventArgs e)
        {
            if (deviceConnectedForm != null)
            {
                deviceConnectedForm.Close();
            }
        }

        private void InitializeContext()
        {
            components = new Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon("Resources/route.ico"),
                Text = Resources.NikNak,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.MouseUp += NotifyIcon_MouseUp;
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            notifyIcon.ContextMenuStrip.Items.Clear();
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Sync Data", Resources.signal_red , this.SyncData_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&About", Resources.signal_red, this.ShowAboutItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Exit", Resources.signal_red, this.ExitItem_Click));
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        private void SyncData_Click(object sender, EventArgs e)
        {
            if (syncForm == null)
            {
                syncForm = new Sync();
                syncForm.Closed += (a, b) => { syncForm = null; };
                syncForm.Show();
            }
            else
            {
                syncForm.Activate();
            }
        }

        private void ShowAboutItem_Click(object sender, EventArgs e)
        {
            if (aboutForm == null)
            {
                aboutForm = new About();
                aboutForm.Closed += (a, b) => { aboutForm = null; };
                aboutForm.Show();
            }
            else
            {
                aboutForm.Activate();
            }
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }
        
        private void NotifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        protected override void ExitThreadCore()
        {
            if (aboutForm != null) { aboutForm.Close(); }

            notifyIcon.Visible = false;
            base.ExitThreadCore();
        }

    }
}