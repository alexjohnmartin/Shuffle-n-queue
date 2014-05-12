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
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Phone.BackgroundAudio;
using System.Windows.Media;
using BugSense;
using RateMyApp.Helpers;
using System.Xml.Linq;

namespace Shuffle_n_queue
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - navigated to");
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
                AddPlaylistsToPanorama();
            }

            UpdateButtonColor();
            FrameworkDispatcher.Update();
        }

        private void AddPlaylistsToPanorama()
        {
            int index = 1;
            foreach (var playlist in App.ViewModel.Playlists)
            {
                AddPlaylistToPanorama(playlist, index);
                index++;
            }
        }

        /*
         <phone:PanoramaItem Header="Songs" Margin="0,-40,0,0">
            <phone:LongListSelector Margin="0,-38,0,2" ItemsSource="{Binding SongItems}">
                <phone:LongListSelector.ItemTemplate>
                    <DataTemplate>
                        <StackPanel Orientation="Horizontal" Margin="12,2,0,4" Height="105" Width="432" Name="SongPanel" Tap="SongPanel_Tap" Tag="{Binding}">
                            <Border BorderThickness="1" Width="10" Height="99" BorderBrush="#FFFFC700" Background="#FFFFC700"/>
                            <!--<Image Source="{Binding AlbumArt}" Width="100" Height="100"/>-->
                            <StackPanel Width="311" Margin="12,-7,0,0">
                                <TextBlock Text="{Binding Name}" Margin="0,0" Style="{StaticResource PhoneTextExtraLargeStyle}" FontSize="{StaticResource PhoneFontSizeLarge}" />
                                <TextBlock Text="{Binding Artist}" Margin="0,-2,10,0" Style="{StaticResource PhoneTextSubtleStyle}" />
                                <TextBlock Text="{Binding Album}" Margin="0,-2,10,0" Style="{StaticResource PhoneTextSubtleStyle}" />
                            </StackPanel>
                        </StackPanel>
                    </DataTemplate>
                </phone:LongListSelector.ItemTemplate>
            </phone:LongListSelector>
          </phone:PanoramaItem>
        */

        private void AddPlaylistToPanorama(Playlist playlist, int index)
        {
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - adding playlist to panorama");
            var item = new PanoramaItem { Header = playlist.Name, Margin = new Thickness(0, -40, 0, 0) };
            var scroller = new ScrollViewer { Margin = new Thickness(0, -38, 0, 2) };
            var listStackPanel = new StackPanel { Orientation = System.Windows.Controls.Orientation.Vertical };
            foreach (var song in playlist.Songs)
            { 
                var outerStackPanel = new StackPanel {
                        Orientation = System.Windows.Controls.Orientation.Horizontal, 
                        Margin = new Thickness(12,2,0,4), 
                        Height = 95, 
                        Width = 432, 
                        Tag = song, 
                        Name = playlist.Name
                };
                outerStackPanel.Tap += SongPanel_Tap;

                var rectangle = new Border { BorderThickness = new Thickness(1), Width = 10, Height = 90, BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 199, 0)), Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 199, 0)) };
                outerStackPanel.Children.Add(rectangle);

                var innerStackPanel = new StackPanel { Width = 311, Margin = new Thickness(12,-7,0,0) };
                var nameTextBlock = new TextBlock { 
                    Text = song.Name, 
                    Margin = new Thickness(0), 
                    Style = (System.Windows.Style)Resources["PhoneTextExtraLargeStyle"],
                    FontSize = (double)Resources["PhoneFontSizeLarge"],
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                innerStackPanel.Children.Add(nameTextBlock);

                var artistTextBlock = new TextBlock
                {
                    Text = song.Artist.Name,
                    Margin = new Thickness(0, -2, 10, 0),
                    Style = (System.Windows.Style)Resources["PhoneTextSubtleStyle"],
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                innerStackPanel.Children.Add(artistTextBlock);

                var albumTextBlock = new TextBlock
                {
                    Text = song.Album.Name,
                    Margin = new Thickness(0, -2, 10, 0),
                    Style = (System.Windows.Style)Resources["PhoneTextSubtleStyle"],
                    Foreground = new SolidColorBrush(Colors.Black),
                };
                innerStackPanel.Children.Add(albumTextBlock);

                outerStackPanel.Children.Add(innerStackPanel);
                listStackPanel.Children.Add(outerStackPanel);
            }
            scroller.Content = listStackPanel;
            item.Content = scroller;
            MainPanorama.Items.Insert(index, item);
        }

        private void SongPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                //song clicked
                BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - song clicked");
                FrameworkDispatcher.Update();
                StackPanel panel = (StackPanel)sender;
                var selectedSong = (Song)panel.Tag;

                if (!MediaPlayer.IsShuffled) MediaPlayer.IsShuffled = true;
                if (!MediaPlayer.IsRepeating) MediaPlayer.IsRepeating = true;

                SongCollection songCollection;
                if (panel.Name == "SongPanel")
                {
                    songCollection = App.ViewModel.AllSongs;
                }
                else
                {
                    var playlist = App.ViewModel.Playlists.First(p => p.Name.Equals(panel.Name));
                    songCollection = playlist.Songs;
                }

                var index = 0;
                foreach (var song in songCollection)
                {
                    if (song == selectedSong)
                        break;

                    index++;
                }

                if (index < songCollection.Count)
                    MediaPlayer.Play(songCollection, index);
                else
                    MessageBox.Show("Song not found", "Error", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                BugSenseHandler.Instance.LogException(ex);
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
            FeedbackHelper.Default.Reviewed();
            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
        }

        public void EmailButton_Click(object sender, EventArgs e)
        {
            var email = new EmailComposeTask();
            email.To = "alexmartin9999@hotmail.com";
            email.Subject = "Feedback for the Your Music Shuffled application";
            email.Show();
        }

        public void CreditsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CreditsPage.xaml", UriKind.Relative));
        }

        private void AppButton_Click(object sender, RoutedEventArgs e)
        {
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - other app...");
            var button = (Button)sender;
            var task = new MarketplaceDetailTask();
            task.ContentIdentifier = button.Tag.ToString();
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - ..." + button.Tag.ToString());
            task.ContentType = MarketplaceContentType.Applications;
            task.Show();
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - pin to home screen");
            var tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("MainPage.xaml"));
            if (tile == null) ShellTile.Create(new Uri("/MainPage.xaml?test=true", UriKind.Relative), GetTileData(), true);
        }

        private ShellTileData GetTileData()
        {
            BugSenseHandler.Instance.LeaveBreadCrumb("MainPage - get tile data");
            var data = new FlipTileData();
            data.BackgroundImage = new Uri(@"/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative);
            data.WideBackgroundImage = new Uri(@"/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative);
            data.Title = Shuffle_n_queue.Resources.AppResources.ApplicationTitle;
            return data;
        }

        private void UpdateButtonColor()
        {
            VersionTextBox.Text = "v" + XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            ReviewButton.Background = new SolidColorBrush((System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"]);
            EmailButton.Background = new SolidColorBrush((System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"]);
            CreditsButton.Background = new SolidColorBrush((System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"]);
            PinButton.Background = new SolidColorBrush((System.Windows.Media.Color)Application.Current.Resources["PhoneAccentColor"]);
        }
    }
}