using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

namespace Shuffle_n_queue
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            UpdatePlayerButtons();
        }

        private void UpdatePlayerButtons()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                Play.Visibility = System.Windows.Visibility.Collapsed;
                Pause.Visibility = System.Windows.Visibility.Visible;
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                Play.Visibility = System.Windows.Visibility.Collapsed;
                Pause.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Play.Visibility = System.Windows.Visibility.Collapsed;
                Pause.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SongPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //song clicked
            StackPanel panel = (StackPanel)sender;
            var selectedSong = (Song)panel.Tag;

            int index = 0;
            foreach (var song in App.ViewModel.AllSongs)
            {
                if (song.Equals(selectedSong))
                    break;

                index++; 
            }

            if (index < App.ViewModel.AllSongs.Count())
            {
                FrameworkDispatcher.Update(); 
                MediaPlayer.Play(App.ViewModel.AllSongs, index);
                MediaPlayer.IsShuffled = true;
                MediaPlayer.IsRepeating = true;
                Play.Visibility = System.Windows.Visibility.Collapsed;
                Pause.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void TwitterButton_Click(object sender, EventArgs e)
        {
            var task = new WebBrowserTask
            {
                Uri = new Uri("https://twitter.com/AlexJohnMartin", UriKind.Absolute)
            };
            task.Show();
        }

        public void StoreButton_Click(object sender, EventArgs e)
        {
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var task = new WebBrowserTask
            {
                Uri = new Uri(string.Format("http://www.windowsphone.com/{0}/store/publishers?publisherId=nocturnal%2Btendencies&appId=63cb6767-4940-4fa1-be8c-a7f58e455c3b", currentCulture.Name), UriKind.Absolute)
            };
            task.Show();
        }

        public void ReviewButton_Click(object sender, EventArgs e)
        {
            //FeedbackHelper.Default.Reviewed();
            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
        }

        public void EmailButton_Click(object sender, EventArgs e)
        {
            var email = new EmailComposeTask();
            email.Subject = "Feedback for the Calendar Tile application";
            email.Show();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update(); 
            MediaPlayer.MovePrevious();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            FrameworkDispatcher.Update(); 
            MediaPlayer.MoveNext(); 
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Collapsed;
            Pause.Visibility = System.Windows.Visibility.Visible;
            FrameworkDispatcher.Update(); 
            MediaPlayer.Resume(); 
        }
 
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Visible;
            Pause.Visibility = System.Windows.Visibility.Collapsed;
            FrameworkDispatcher.Update(); 
            MediaPlayer.Pause(); 
        }
    }
}