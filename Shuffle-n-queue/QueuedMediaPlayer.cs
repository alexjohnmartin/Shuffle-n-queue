using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shuffle_n_queue
{
    public class QueuedMediaPlayer : IDisposable
    {
        public void Play(Song song)
        {
            _doonce = false; 
            _songs = new List<Song>(App.ViewModel.AllSongs.OrderBy(s => Guid.NewGuid()));
            _currentSong = _songs.IndexOf(song);
            MediaPlayer.Play(song);
            OnNewSongPlaying(new EventArgs()); 
        }

        internal void Pause()
        {
            _doonce = false; 
            MediaPlayer.Pause(); 
        }

        public void PlayNext()
        {
            if (_queuedSongs.Count() > 0)
            {
                var song = _queuedSongs.Dequeue();
                if (_songs.Any(s => s.Equals(song)))
                {
                    _currentSong = _songs.IndexOf(song);
                }

                _doonce = false;
                MediaPlayer.Play(song);
            }
            else
            {
                _currentSong++;
                if (_currentSong > _songs.Count() - 1)
                    _currentSong = 0;

                _doonce = false;
                MediaPlayer.Play(_songs[_currentSong]);
            }
            OnNewSongPlaying(new EventArgs()); 
        }

        public void PlayPrev()
        {
            _currentSong--;
            if (_currentSong < 0)
                _currentSong = _songs.Count() - 1;

            _doonce = false;
            MediaPlayer.Play(_songs[_currentSong]);
            OnNewSongPlaying(new EventArgs()); 
        }

        public Song GetCurrentSong()
        {
            return _songs[_currentSong];
        }

        private void UpdateEvents()
        {
            while (_running)
            {
                FrameworkDispatcher.Update();
                Thread.Sleep(500);
            }
        }

        public void Resume()
        {
            _doonce = false; 
            MediaPlayer.Resume(); 
        }

        public void Queue(Song song)
        {
            _queuedSongs.Enqueue(song);
        }

        public void Dispose()
        {
            _running = false;
            _updateThread.Join(); 
        }

        void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                if (_doonce)
                {
                    _doonce = false;
                    PlayNext();
                }
            }
            if (MediaPlayer.State == MediaState.Playing)
            {
                _doonce = true;
            }
        }

        public event EventHandler<EventArgs> NewSongPlaying;

        protected virtual void OnNewSongPlaying(EventArgs e)
        {
            EventHandler<EventArgs> handler = NewSongPlaying;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool _running = false;
        private bool _doonce = true;
        private int _currentSong = 0;
        private IList<Song> _songs;
        private Queue<Song> _queuedSongs;
        private Thread _updateThread;

        public QueuedMediaPlayer()
        {
            _songs = new List<Song>(App.ViewModel.AllSongs.OrderBy(s => Guid.NewGuid()));
            _queuedSongs = new Queue<Song>();

            FrameworkDispatcher.Update();
            MediaPlayer.IsShuffled = false;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

            _running = true;
            _updateThread = new Thread(UpdateEvents);
            _updateThread.Start();
        }
    }
}
