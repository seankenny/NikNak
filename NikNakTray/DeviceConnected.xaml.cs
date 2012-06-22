using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace NikNakTray
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class DeviceConnected : Window
    {
        public DeviceConnected()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }
    }
}
