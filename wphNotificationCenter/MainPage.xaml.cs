using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Notification;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Threading;
namespace wphNotificationCenter
{
    public partial class MainPage : PhoneApplicationPage
    {
        public class notificationData
        {
            public String title { get; set; }
            public String subtitle { get; set; }
            public String time { get; set; }
            public String id { get; set; }

            public notificationData(String title, String subtitle, String time, String id)
            {
                this.title = title;
                this.subtitle = subtitle;
                this.time = time;
                this.id = id;
            }
        }
        public ObservableCollection<notificationData> notificationList  { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            offNotifications.Visibility = System.Windows.Visibility.Collapsed;
            noNotifications.Visibility = System.Windows.Visibility.Collapsed;

            notificationList = new ObservableCollection<notificationData>();

            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var files_alphaorder = store.GetFileNames("notification_*");
                var files = files_alphaorder.Reverse();

                foreach (var filename in files)
                {
                    System.Diagnostics.Debug.WriteLine(filename);
                    using (var file = store.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        StreamReader reader = new StreamReader(file);
                        string body = reader.ReadToEnd();

                        string id = filename.Replace(".txt", "");

                        string title = "";

                        string[] s;

                        if (body.Contains("wp:Text1>"))
                        {
                            s = body.Split(new string[] { "wp:Text1>" }, StringSplitOptions.RemoveEmptyEntries);
                            s = s[1].Split("<".ToCharArray());

                            title = s[0];
                        }
                        string line = "";

                        if (body.Contains("wp:Text2>"))
                        {

                            s = body.Split(new string[] { "wp:Text2>" }, StringSplitOptions.RemoveEmptyEntries);
                            s = s[1].Split("<".ToCharArray());
                            line = s[0];
                        }


                        if (title == "")
                        {
                            if (line.Contains("Mentioned"))
                            {
                                title = "Twitter";
                            }
                        }

                        DateTime time = store.GetCreationTime(filename).DateTime;
                        string timestr = RelativeDate(time);

                        notificationList.Add(new notificationData(title, line, timestr, id));
                    }

                    if (files.Count() == 0)
                    {
                        noNotifications.Visibility = System.Windows.Visibility.Visible;
                        lb1.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    

                    lb1.DataContext = this;

                }

            }

        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            myStoryboard.Begin();
            if (!WP7RootToolsSDK.Environment.HasRootAccess())
            {
                MessageBox.Show("Your phone needs full EXE unlock to use this application. Please enable this in the respective application, e.g. Root Tools", "Full Unlock Needed", MessageBoxButton.OK);
            }
            
            
            var items = WP7RootToolsSDK.FileSystem.GetFolder(@"\windows\startup").GetSubItems();
            bool installed = false;
            foreach (var item in items) {
                if (item.Name.Contains("wph_ncservice")) installed = true;
            }
            if (!installed)
            {
                MessageBox.Show("NC is not installed. Press the install button to begin.", "Welcome", MessageBoxButton.OK);
                NavigationService.Navigate(new Uri("/settings.xaml", UriKind.Relative));
            }

        }

        public static string RelativeDate(DateTime theDate)
        {
            Dictionary<long, string> thresholds = new Dictionary<long, string>();
            int minute = 60;
            int hour = 60 * minute;
            int day = 24 * hour;
            thresholds.Add(60, "{0} seconds ago");
            thresholds.Add(minute * 2, "a minute ago");
            thresholds.Add(45 * minute, "{0} minutes ago");
            thresholds.Add(120 * minute, "an hour ago");
            thresholds.Add(day, "{0} hours ago");
            thresholds.Add(day * 2, "yesterday");
            thresholds.Add(day * 30, "{0} days ago");
            thresholds.Add(day * 365, "{0} months ago");
            thresholds.Add(long.MaxValue, "{0} years ago");

            long since = (DateTime.Now.Ticks - theDate.Ticks) / 10000000;
            foreach (long threshold in thresholds.Keys)
            {
                if (since < threshold)
                {
                    TimeSpan t = new TimeSpan((DateTime.Now.Ticks - theDate.Ticks));
                    return string.Format(thresholds[threshold], (t.Days > 365 ? t.Days / 365 : (t.Days > 0 ? t.Days : (t.Hours > 0 ? t.Hours : (t.Minutes > 0 ? t.Minutes : (t.Seconds > 0 ? t.Seconds : 0))))).ToString());
                }
            }
            return "";
        }
        private void StackPanel_Tap_1(object sender, GestureEventArgs e)
        {
            string id = ((notificationData)lb1.SelectedItem).id;
            //TODO: Send message to the NC server to "replay" the notification
        }


        private void StackPanel_ManipulationDelta_1(object sender, ManipulationDeltaEventArgs e)
        {
            StackPanel obj = (StackPanel)sender;

            double x = obj.Margin.Left;
            x += e.DeltaManipulation.Translation.X;

            if (x > 0)
            {
                obj.Margin = new Thickness(x, 0, 0, 0);
            }
            //if it passes a certain point, it goes bye-bye
            if (x > 120)
            {
                obj.Opacity = 0.5;
            }
            else
            {
                obj.Opacity = 1;
            }

            if (x > 280)
            {
                string id = ((TextBlock)obj.Children[0]).Text;
                
                notificationList.Remove(notificationList.Where(t => t.id == id).FirstOrDefault());
                
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(id + ".txt"))
                    {
                        store.DeleteFile(id + ".txt");
                    }
                }
            }

        }

        private void StackPanel_ManipulationCompleted_1(object sender, ManipulationCompletedEventArgs e)
        {
            StackPanel obj = (StackPanel)sender;

            obj.Opacity = 1;

            double x = obj.Margin.Left;

            DispatcherTimer tim = new DispatcherTimer();
            tim.Interval = TimeSpan.FromMilliseconds(10);
            tim.Tick += (o,e2) =>
            {
                if (x > 0)
                    x -= 12;
                else
                    x = 0;

                if (x < 0)
                    x = 0;

                obj.Margin = new Thickness(x, 0, 0, 0);
                if (x == 0)
                    tim.Stop();
            };
            tim.Start();

        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/settings.xaml", UriKind.Relative));
        }

    }
}