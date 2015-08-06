using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace SamplePlayer.WP8
{
    public partial class PlayerPage : PhoneApplicationPage
    {
        private string id;
        private int ScreenWidth, ScreenHeight;

        public PlayerPage()
        {
            InitializeComponent();

            // from http://d.hatena.ne.jp/c-mitsuba/20110908/1315455810
            ScreenWidth = (int)Application.Current.Host.Content.ActualWidth;
            ScreenHeight = (int)Application.Current.Host.Content.ActualHeight;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                Player.Width = ScreenWidth;
                Player.Height = Player.Width * 3 / 4; // aspect ratio = 4 : 3
                SocialStream.Width = ScreenWidth;
                SocialStream.Height = ScreenHeight - Player.Height;

                string liveUrl;
                NavigationContext.QueryString.TryGetValue("id", out id);
                NavigationContext.QueryString.TryGetValue("liveurl", out liveUrl);
                Player.Source = new Uri(liveUrl, UriKind.Absolute);
                SocialStream.Source = new Uri("http://www.ustream.tv/socialstream/" + id + "?siteMode=1&activeTab=socialStream&hideVideoTab=1&colorScheme=light&v=6", UriKind.Absolute);
            }
        }

        private void SocialStream_Navigated(object sender, NavigationEventArgs e)
        {
            Uri source = (sender as WebBrowser).Source;
            // return from authentication
            if (source.AbsoluteUri.StartsWith("http://www.ustream.tv/dashboard/twitter/connect/save?opener=socialstream"))
            {
                //string query = source.AbsoluteUri.Substring(source.AbsoluteUri.IndexOf("dc="));
                //SocialStream.Source = new Uri("http://www.ustream.tv/socialstream/" + id + "?siteMode=1&activeTab=socialStream&hideVideoTab=1&colorScheme=light&v=6", UriKind.Absolute);
                SocialStream.Navigate(new Uri("http://www.ustream.tv/socialstream/" + id + "?siteMode=1&activeTab=socialStream&hideVideoTab=1&colorScheme=light&v=6", UriKind.Absolute));
                //CookieCollection ccol = (sender as WebBrowser).GetCookies();
            }
        }

        private void SocialStream_LoadCompleted(object sender, NavigationEventArgs e)
        {
            // Disable scaling by modifying viewport -> http://amci.seesaa.net/article/214969632.html
            String function = "var meta = document.createElement(\'meta\');meta.setAttribute(\'name\', \'viewport\');meta.setAttribute(\'content\', \'width=device-width, user-scalable=0\');document.getElementsByTagName(\'head\')[0].appendChild(meta);";
            SocialStream.InvokeScript("eval", function);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.Orientation.ToString().StartsWith("Landscape"))
            {
                SocialStream.Visibility = Visibility.Collapsed;

                Player.Width = ScreenHeight;
                Player.Height = ScreenWidth;
            }
            else
            {
                Player.Width = ScreenWidth;
                Player.Height = Player.Width * 3 / 4; // aspect ratio = 4 : 3
                SocialStream.Width = ScreenWidth;
                SocialStream.Height = ScreenHeight - Player.Height;

                SocialStream.Visibility = Visibility.Visible;
            }
        }
    }

}