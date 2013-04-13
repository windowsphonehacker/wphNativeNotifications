using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace wphNotificationCenter
{
    public partial class Install : PhoneApplicationPage
    {
        public Install()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("Your phone needs to be rooted, and Notifications must have root access.");
                MessageBox.Show("In addition, you need EXE unlock.");
                return;
            }
            string apppath = @"\Applications\Install\9d85578e-4908-43f3-a9e6-8ba5d297b817\Install\";

            try
            {
                WP7RootToolsSDK.FileSystem.CreateFolder(@"\wphnc");
            }
            catch
            {
            }

            try
            {
                var folder = WP7RootToolsSDK.FileSystem.GetFolder(apppath + @"wphnc\");
                foreach (var item in folder.GetSubItems())
                {
                    if (item.IsFile)
                    {
                        WP7RootToolsSDK.FileSystem.CopyFile(item.Path, @"\wphnc\" + item.Name);
                    }
                }

                var startupfolder = WP7RootToolsSDK.FileSystem.GetFolder(apppath + @"startup\");
                foreach (var item in startupfolder.GetSubItems())
                {
                    if (item.IsFile)
                    {
                        WP7RootToolsSDK.FileSystem.CopyFile(item.Path, @"\Windows\Startup\" + item.Name);
                    }
                }
                WP7RootToolsSDK.Environment.ProcessConfigXml("<wap-provisioningdoc><characteristic type=\"Registry\"><characteristic type=\"HKLM\\COMM\\Tcpip\\Hosts\\push.live.net\"><parm name=\"ipaddr\" value=\"fwAAAQ==\" datatype=\"binary\" /><parm name=\"ExpireTime\" value=\"mZmZmQ==\" datatype=\"binary\" /></characteristic></characteristic></wap-provisioningdoc>");
                MessageBox.Show("WPH Notifications was installed.", "c:", MessageBoxButton.OK);
                MessageBox.Show("Please restart your phone.");
                
            }
            catch
            {
                MessageBox.Show("Something went wrong while copying files! Make sure your device is rooted, and that Notifications has access permissions.");
            }
        }
    }
}