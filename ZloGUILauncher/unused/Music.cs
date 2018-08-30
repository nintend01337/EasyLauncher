using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ZloGUILauncher
{
    public class Music
    {
        public enum MusicState : int
        {
            Stop    = 0,
            Play    = 1,
            Pause   = 3,
        }

        string MusicPath = AppDomain.CurrentDomain.BaseDirectory + "music.mp3";
        static Music musicplayer = null;

        private Music()
        {MediaPlayer mp = new MediaPlayer();}

        public static Music Player
        {
            get 
            {
                if(musicplayer == null)
            {
                    musicplayer = new Music();
            }
                return musicplayer;
            }
        }

        public void PlayMusic(MusicState state,MediaPlayer mp)
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(MusicPath,UriKind.Relative));
            switch (state)
            {
                case MusicState.Stop:
                    player.Stop();
                    break;

                case MusicState.Play:
                    player.Play();
                    break;
                case MusicState.Pause:
                    player.Pause();
                    break;
            }
        }

        public void PlayMusic(MusicState state,string URI)
        {
            string uri = URI;
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(uri, UriKind.Relative));
            switch (state)
            {
                case MusicState.Stop:
                    player.Stop();
                    break;

                case MusicState.Play:
                    player.Play();
                    break;
                case MusicState.Pause:
                    player.Pause();
                    break;
            }
        }
    }
}
