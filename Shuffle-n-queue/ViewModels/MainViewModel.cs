using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq; 
using Shuffle_n_queue.Resources;
using Microsoft.Xna.Framework.Media;

namespace Shuffle_n_queue.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.SongItems = new ObservableCollection<Song>();
        }

        public ObservableCollection<Song> SongItems { get; private set; }

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
                    var songs = mediaLibrary.Songs;
                    foreach (var song in songs)
                    {
                        SongItems.Add(song); 
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