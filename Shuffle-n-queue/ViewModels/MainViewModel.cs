using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq; 
using Shuffle_n_queue.Resources;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;

namespace Shuffle_n_queue.ViewModels
{
    public class SongItem
    {
        public string Name { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public WriteableBitmap AlbumArt { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.SongItems = new ObservableCollection<SongItem>();
        }

        public ObservableCollection<SongItem> SongItems { get; private set; }
        public SongCollection AllSongs { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            foreach (MediaSource source in MediaSource.GetAvailableMediaSources())
            {
                if (source.MediaSourceType == MediaSourceType.LocalDevice)
                {
                    var mediaLibrary = new MediaLibrary(source);
                    AllSongs = mediaLibrary.Songs;
                    foreach (var song in AllSongs)
                    {
                        //try
                        //{
                        //    var stream = song.Album.GetAlbumArt();
                        //    var bitmap = new WriteableBitmap(100, 100);
                        //    bitmap.SetSource(stream); 
                        //    SongItems.Add(new SongItem
                        //    {
                        //        Name = song.Name,
                        //        Artist = song.Artist.Name,
                        //        Album = song.Album.Name,
                        //        AlbumArt = bitmap
                        //    });
                        //}
                        //catch
                        //{
                        //    SongItems.Add(new SongItem
                        //    {
                        //        Name = song.Name,
                        //        Artist = song.Artist.Name,
                        //        Album = song.Album.Name,
                        //        AlbumArt = new WriteableBitmap(100, 100)
                        //    });
                        //}   
                        SongItems.Add(new SongItem
                            {
                                Name = song.Name,
                                Artist = song.Artist.Name,
                                Album = song.Album.Name,
                                AlbumArt = new WriteableBitmap(100, 100)
                            });
                    }
                }
            }
            //SongItems.OrderBy(s => s.Artist + s.Name); 

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}