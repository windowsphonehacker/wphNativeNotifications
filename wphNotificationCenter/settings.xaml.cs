using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Notification;

namespace wphNotificationCenter
{
    public partial class settings : PhoneApplicationPage
    {
        public settings()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var str = WP7RootToolsSDK.Environment.ProcessConfigXml("<wap-provisioningdoc><characteristic type=\"Registry\"><nocharacteristic type=\"HKLM\\COMM\\Tcpip\\Hosts\\push.live.net\" /></characteristic></wap-provisioningdoc>");
            try
            {
                WP7RootToolsSDK.FileSystem.DeleteFile(@"\windows\startup\wph_ncservice.lnk");
                WP7RootToolsSDK.FileSystem.DeleteFile(@"\windows\startup\wph_stunnel_native.lnk");
                WP7RootToolsSDK.FileSystem.DeleteFile(@"\windows\startup\wph_stunnel_nc.lnk");
                WP7RootToolsSDK.FileSystem.DeleteFile(@"\windows\startup\wph_stunnel_proxy.lnk");
            }
            catch
            {
            }
            MessageBox.Show("Unprovisioning complete. You can now uninstall this app.");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Install.xaml", UriKind.Relative));
        }
    }
}