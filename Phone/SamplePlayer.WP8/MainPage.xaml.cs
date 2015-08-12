using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace SamplePlayer.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // コンストラクター
        public MainPage()
        {
            InitializeComponent();
        }

        private void RootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RootPivot.SelectedIndex == 0) // search
            {
                SearchBox.Visibility = Visibility.Visible;
            }
            else //about
            {
                SearchBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="query"></param>
        private async void Search(string query)
        {
            searchList.Visibility = Visibility.Collapsed;
            NoItemLabel.Visibility = Visibility.Collapsed;
            progressIndicator.IsVisible = true;

            string url;
            if (query.Length == 0)
            {
                url = "http://api.ustream.tv/json/stream/popular/search/all";
            }
            else
            {
                url = "http://api.ustream.tv/json/stream/popular/search/title:like:" + HttpUtility.UrlEncode(query);
            }
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBodyAsText = await response.Content.ReadAsStringAsync();

            // refer to http://blogs.gine.jp/taka/archives/2106
            SearchResults res = new SearchResults();
            var serializer = new DataContractJsonSerializer(typeof(SearchResults));
            byte[] bytes = Encoding.UTF8.GetBytes(responseBodyAsText);
            MemoryStream ms = new MemoryStream(bytes);
            res = serializer.ReadObject(ms) as SearchResults;
            responseBodyAsText = "";
            progressIndicator.IsVisible = false;

            if (res.Results != null)
            {
                //Format item
                res.Results.RemoveAll(temp => temp.HttpLiveUrl == null);
                foreach (SearchItem item in res.Results)
                {
                    item.Title = HttpUtility.HtmlDecode(item.Title);
                }
            }
            else
            {
                NoItemLabel.Visibility = Visibility.Visible;
                return;
            }

            // Data binding
            searchList.ItemsSource = res.Results;

            if (res.Results.Count > 0)
            {
                NoItemLabel.Visibility = Visibility.Collapsed;
                searchList.Visibility = Visibility.Visible;
            }
            else
            {
                NoItemLabel.Visibility = Visibility.Visible;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search(SearchText.Text);
        }

        private void searchList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SearchItem item = (SearchItem)((sender as LongListSelector).SelectedItem);
            if (item != null)
            {
                NavigationService.Navigate(new Uri("/PlayerPage.xaml?id=" + item.Id + "&liveurl=" + item.HttpLiveUrl, UriKind.Relative));
            }
        }

        private void StoreButton_Click(object sender, RoutedEventArgs e)
        {
            // Launch review
            LaunchReview();
        }

        // Launch the URI
        private async void LaunchReview()
        {
            // Launch the URI
            string appid = "515b7fe5-9046-4adf-8be1-29f29683b63d";
            var success = await Windows.System.Launcher.LaunchUriAsync(new Uri("zune:reviewapp?appid=app" + appid));

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }


    }

    /// <summary>
    /// refer to http://chorusde.hatenablog.jp/entry/20110910/1315620186
    /// </summary>
    [DataContract]
    public class SearchResults
    {
        /// <summary>
        /// results
        /// </summary>
        [DataMember(Name = "results")]
        public List<SearchItem> Results { get; set; }
    }

    /// <summary>
    /// refer to http://chorusde.hatenablog.jp/entry/20110910/1315620186
    /// </summary>
    [DataContract]
    public class SearchItem
    {
        /// <summary>
        /// title
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// title
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// streamStartedAt
        /// </summary>
        [DataMember(Name = "streamStartedAt")]
        public string StreamStartedAt { get; set; }

        /// <summary>
        /// currentNumberOfViewers
        /// </summary>
        [DataMember(Name = "currentNumberOfViewers")]
        public string CurrentNumberOfViewers { get; set; }

        /// <summary>
        /// liveHttpUrl
        /// </summary>
        [DataMember(Name = "liveHttpUrl")]
        public string HttpLiveUrl { get; set; }

        // Pin
        public bool isPinned { get; set; }
    }

}
