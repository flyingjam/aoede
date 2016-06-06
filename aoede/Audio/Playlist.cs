using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace aoede.Audio
{
    enum PlaylistModes { NORMAL, RANDOM, SHUFFLE }
    class Playlist
    {

        //TODO: change
        public List<Music> music { get; private set; }
        List<int> history;
        Random rand;

        int currentIndex { get; set; }
        Playback playback;

        public Playlist()
        {
            music = new List<Music>();
            history = new List<int>();

            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Playlist(Playback play)
        {
            music = new List<Music>();
            history = new List<int>();

            rand = new Random();
            currentIndex = 0;
            playback = play;
        }

        public Playlist(params Music[] m)
        {
            music = new List<Music>();

            foreach (Music item in m)
                music.Add(item);

            history = new List<int>();

            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Playlist(List<Music> m)
        {
            music = m;
            history = new List<int>();
            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Music get()
        {
            if (music.Count == 0)
                return null;
            return music[currentIndex];
        }

        public int next()
        {
            history.Add(currentIndex);
            currentIndex = playback.next(currentIndex, music, history);
            return currentIndex;
        }

        public int previous()
        {
            currentIndex = playback.previous(currentIndex, music, history);
            return currentIndex;
        }

        public void add(Music m)
        {
            music.Add(m);
        }

        public void setPlayback(Playback play)
        {
            playback = play;
        }

        public void add(List<Music> m)
        {
            foreach (Music item in m)
            {
                music.Add(item);
            }
        }

        public void add(params Music[] m)
        {
            foreach (Music item in m)
            {
                music.Add(item);
            }
        }

        public override string ToString()
        {
            //no stringbuilder because I don't care
            string str = "Music: \n";
            foreach(Music m in music)
            {
                str += (m.filepath + "\n");
            }

            return str;
        }
    }

    abstract class Playback
    {
        abstract public int next(int current, List<Music> music, List<int> history);
        abstract public int previous(int current, List<Music> music, List<int> history);
    }

    class PlaybackNormal : Playback
    {
        public PlaybackNormal()
        {

        }

        public override int next(int current, List<Music> music, List<int> history)
        {
            return (current + 1) % music.Count;
        }

        public override int previous(int current, List<Music> music, List<int> history)
        {
            int potential = current - 1;
            if (potential < 0)
                potential = 0;
            return potential;
        }
    }
    class PlaybackShuffle : Playback
    {
        Random rand;
        List<int> shuffleMemory;
        public PlaybackShuffle()
        {
            rand = new Random();
            shuffleMemory = new List<int>();
        }
        public override int next(int current, List<Music> music, List<int> history)
        {
            if (shuffleMemory.Count == music.Count)
                shuffleMemory.Clear();

            int potential = rand.Next(0, music.Count);
            while (shuffleMemory.Contains(potential))
            {
                potential = rand.Next(0, music.Count);
            }

            shuffleMemory.Add(current);
            return potential;
        }

        public override int previous(int current, List<Music> music, List<int> history)
        {
            if (history.Count == 0)
                return 0;
            int potential = shuffleMemory[shuffleMemory.Count - 1];
            shuffleMemory.RemoveAt(shuffleMemory.Count - 1);
            return potential;
        }
    }


}
