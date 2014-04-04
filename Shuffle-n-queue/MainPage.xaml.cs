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
using System.Windows.Media.Imaging;
using Nokia.Graphics.Imaging;

namespace Shuffle_n_queue
{
    public partial class MainPage : PhoneApplicationPage
    {
        private QueuedMediaPlayer _queuedMediaPlayer; 

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            //MediaPlayer.ActiveSongChanged += MediaPlayer_ActiveSongChanged;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
                _queuedMediaPlayer = new QueuedMediaPlayer();
                _queuedMediaPlayer.NewSongPlaying += _queuedMediaPlayer_NewSongPlaying;
            }

            FrameworkDispatcher.Update();
            UpdatePlayerButtons();
            UpdatePlayerDisplay(); 
        }

        void _queuedMediaPlayer_NewSongPlaying(object sender, EventArgs e)
        {
            UpdatePlayerDisplay(); 
        }

        private async void UpdatePlayerDisplay()
        {
            var song = _queuedMediaPlayer.GetCurrentSong();
            if (song != null)
            {
                SongText.Text = song.Name;
                AlbumText.Text = song.Album.Name;
                ArtistText.Text = song.Artist.Name;

                var client = new Nokia.Music.MusicClient("6944ff89bdf75c7b74a3f24f28e3fe26");
                try
                {
                    var searchTerm = song.Name;
                    var stream = song.Album.GetAlbumArt();
                    var bitmap = new WriteableBitmap(360, 360);
                    bitmap.SetSource(stream);
                    NowPlayingImage.Source = bitmap;
                    NowPlayingImage.Visibility = System.Windows.Visibility.Visible;
                }
                catch
                {
                    NowPlayingImage.Visibility = System.Windows.Visibility.Collapsed;
                }
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
            _queuedMediaPlayer.Play(selectedSong); 
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
            _queuedMediaPlayer.PlayPrev();
            UpdatePlayerDisplay(); 
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //var index = MediaPlayer.Queue.ActiveSongIndex;
            //index++;
            //if (index >= MediaPlayer.Queue.Count) index = 0;
            //UpdatePlayerDisplay(MediaPlayer.Queue[index]); 
            //FrameworkDispatcher.Update(); 
            //MediaPlayer.MoveNext(); 

            _queuedMediaPlayer.PlayNext();
            UpdatePlayerDisplay(); 
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Collapsed;
            Pause.Visibility = System.Windows.Visibility.Visible;
            _queuedMediaPlayer.Resume();
            UpdatePlayerDisplay(); 
        }
 
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Play.Visibility = System.Windows.Visibility.Visible;
            Pause.Visibility = System.Windows.Visibility.Collapsed;
            _queuedMediaPlayer.Pause(); 
        }

        public void CreditsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreditsPage.xaml", UriKind.Relative));
        }

        private void PlayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var song = (Song)item.Tag;
            _queuedMediaPlayer.Play(song); 
        }

        private void QueueMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (MenuItem)sender;
            var song = (Song)item.Tag;
            _queuedMediaPlayer.Queue(song); 
            //var indexOfNextSong = MediaPlayer.Queue.ActiveSongIndex;
            //indexOfNextSong++;
            //if (indexOfNextSong >= MediaPlayer.Queue.Count) indexOfNextSong = 0;
            //MediaPlayer.Queue[indexOfNextSong] = song; 

            //http://xboxforums.create.msdn.com/forums/p/23427/125889.aspx
            //TODO:player will need to keep it's own playlist, listen for media player events and manually fire the next song
            //not sure how well this is going to work if app is in background
            //http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj681691%28v=vs.105%29.aspx
            //http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj662935%28v=vs.105%29.aspx
            //even if this works, how well will this work with the normal phone next/prev buttons shown on the lock screen?
        }
    }
}