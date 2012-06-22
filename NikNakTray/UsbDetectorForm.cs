using System;
using System.Drawing;
using System.Windows.Forms;

namespace NikNakTray
{
    internal class UsbDetectorForm : Form
    {
        private readonly UsbDetector _detector;
        private Label _label;

        public UsbDetectorForm(UsbDetector detector)
        {
            this._detector = detector;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.None;
            Load += Load_Form;
            Activated += Form_Activated;
        }

        private void Load_Form(object sender, EventArgs e)
        {
            // We don't really need this, just to display the label in designer ...
            InitializeComponent();

            // Create really small form, invisible anyway.
            Size = new Size(5, 5);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            Visible = false;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (_detector != null)
            {
                _detector.WndProc(ref m);
            }
        }

        private void InitializeComponent()
        {
            _label = new Label();
            SuspendLayout();
            
            _label.AutoSize = true;
            _label.Location = new Point(13, 30);
            _label.Name = string.Empty;
            _label.Size = new Size(314, 13);
            _label.TabIndex = 0;
            _label.Text = string.Empty;

            ClientSize = new Size(360, 80);
            Controls.Add(_label);
            Name = string.Empty;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}