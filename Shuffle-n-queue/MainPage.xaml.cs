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
using System.Threading;

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
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            //MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        private void MediaPlayer_ActiveSongChanged(object sender, EventArgs e)
        {
            //UpdatePlayerDisplay(); 
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            //UpdatePlayerButtons();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            FrameworkDispatcher.Update();
            UpdatePlayerButtons();
            UpdatePlayerDisplay(MediaPlayer.Queue.ActiveSong); 
        }

        private void UpdatePlayerDisplay(Song song)
        {
            if (song != null)
            {
                SongText.Text = song.Name;
                AlbumText.Text = song.Album.Name;
                ArtistText.Text = song.Artist.Name;
            }
            else
            {
                SongText.Text = string.Empty;
                AlbumText.Text = string.Empty;
                ArtistText.Text = string.Empty;
            }
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
                Play.Visibility = System.Windows.Visibility.Visible;
                Pause.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SongPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //song clicked
            StackPanel panel = (StackPanel)sender;
            var selectedSong = (Song)panel.Tag;
            PlaySong(selectedSong);
        }

        private void PlaySong(Song selectedSong)
        {
            int index = 0;
            foreach (var song in App.ViewModel.AllSongs)
            {
                if (song.Equals(selectedSong))
                    break;

                index++;
            }

            if (index < App.ViewModel.AllSongs.Count())
            {
                UpdatePlayerDisplay(selectedSong);
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
            email.Subject = "Feedback for the Shuffle'n'queue application";
            email.Show();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            var index = MediaPlayer.Queue.ActiveSongIndex;
            index--;
            if (index < 0) index = MediaPlayer.Queue.Count - 1; 
            UpdatePlayerDisplay(MediaPlayer.Queue[index]); 
            FrameworkDispatcher.Update(); 
            MediaPlayer.MovePrevious();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var index = MediaPlayer.Queue.ActiveSongIndex;
            index++;
            if (index >= MediaPlayer.Queue.Count) index = 0;
            UpdatePlayerDisplay(MediaPlayer.Queue[index]); 
            FrameworkDispatcher.Update(); 
            MediaPlayer.MoveNext(); 
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Collapsed;
            Pause.Visibility = System.Windows.Visibility.Visible;
            FrameworkDispatcher.Update();
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
            else
            {
                MediaPlayer.Play(App.ViewModel.AllSongs, 0);
                MediaPlayer.IsShuffled = true;
                MediaPlayer.IsRepeating = true;
                UpdatePlayerDisplay(MediaPlayer.Queue[0]); 
            }
        }
 
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Visible;
            Pause.Visibility = System.Windows.Visibility.Collapsed;
            FrameworkDispatcher.Update(); 
            MediaPlayer.Pause(); 
        }

        public void CreditsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreditsPage.xaml", UriKind.Relative));
        }

        private void PlayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var song = (Song)item.Tag;
            PlaySong(song); 
        }

        private void QueueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("not implemented yet"); 
        }
    }
}