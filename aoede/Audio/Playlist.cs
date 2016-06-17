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
        public List<Music> music { get; set; }
        public string Name { get; set; }
        List<int> history;
        Random rand;

        public int GlobalID { get; set; } = -1;

        int currentIndex { get; set; }
        int nextId = 0;
        Playback playback;

        public Playlist()
        {
            music = new List<Music>();
            history = new List<int>();

            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Playlist(Playback play, string name = "")
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
                add(item);

            history = new List<int>();

            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Playlist(List<Music> m, string name = "")
        {
            music = m;
            //still have to tag music
            foreach(Music item in music)
            {
                item.PlaylistID = nextId;
                nextId++;
            }

            history = new List<int>();
            rand = new Random();
            currentIndex = 0;
            playback = new PlaybackNormal();
        }

        public Music Get()
        {
            if (music.Count == 0)
                return null;
            return music[currentIndex];
        }
        
        public Music GetAt(int PID)
        {
            foreach(Music m in music)
            {
                if (m.PlaylistID == PID)
                    return m;
            }
            return null;
        }

        public int Next()
        {
            history.Add(currentIndex);
            currentIndex = playback.next(currentIndex, music, history);
            return currentIndex;
        }

        public int Previous()
        {
            currentIndex = playback.previous(currentIndex, music, history);
            return currentIndex;
        }

        public void Move(int index, int position)
        {
            var temp = music[index];
            music.RemoveAt(index);
            music.Insert(position, temp);
        }

        public void MoveAfter(int index, int position)
        {
            var temp = music[index];
            music.RemoveAt(index);
            if (position + 1 > music.Count)
                music.Add(temp);
            else
                music.Insert(position + 1, temp);
        }

        public void moveBeforePID(int PID, int PIDPosition)
        {
            var index = indexOf(PID);
            var temp = music[index];
            music.RemoveAt(index);

            var position = indexOf(PIDPosition);
            //if (position > index) position--;
            if (position == -1)
                Console.WriteLine("WTF");
            music.Insert(position, temp);
            Console.WriteLine("STATUS: ");
            foreach(Music m in music)
            {
                Console.WriteLine(m.Filepath);
            }
            Console.WriteLine("END");
        }

        public void moveAfterPID(int PID, int PIDPosition)
        {
            var index = indexOf(PID);
            var temp = music[index];
            music.RemoveAt(index);

            var position = indexOf(PIDPosition);
            if(position + 1 > music.Count)
            {
                music.Add(temp);
            }
            else
            {
                music.Insert(position + 1, temp);
            }

        }

        public void moveBefore(int index, int position)
        {
            Console.WriteLine("Moving {0} to {1}", music[index].Filepath, position);
            var temp = music[index];
            music.RemoveAt(index);
            if (position > index) position--;
            music.Insert(position, temp);
        }

        public int indexOf(Music m)
        {
            return music.IndexOf(m);
        }
        
        public int indexOf(Guid UUID)
        {
            for(int i = 0; i < music.Count; i++)
            {
                if (music[i].UUID == UUID)
                    return i;
            }
            return -1;
        }

        public int indexOf(int PID)
        {
            for(int i = 0; i < music.Count; i++)
            {
                if (music[i].PlaylistID == PID)
                    return i;
            }
            return -1;
        }

        public void seek(Music m)
        {
            var index = music.IndexOf(m);
            if (index != -1)
            {
                history.Add(currentIndex);
                currentIndex = index;
            }
        }

        public bool seek(int index)
        {
            if(index >= 0 && index < music.Count)
            {
                history.Add(currentIndex);
                currentIndex = index;
                return true;
            }
            return false;
        }


        public bool seek(string file)
        {
            for(int i = 0; i < music.Count; i++)
            {
                if(music[i].Filepath == file)
                {
                    return seek(i);
                }
            }
            return false;
        }

        public bool seek(Guid UUID)
        {
            for(int i = 0; i < music.Count; i++)
            {
                if (music[i].UUID == UUID)
                {
                    return seek(i);
                }
            }
            return false;
        }

        public bool seekPID(int PID)
        {
            for(int i = 0; i < music.Count; i++)
            {
                if (music[i].PlaylistID == PID)
                {
                    return seek(i);
                }
            }
            return false;
        }

        public void add(Music m)
        {
            m.PlaylistID = nextId;
            nextId++;
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
                add(item);
            }
        }

        public void add(params Music[] m)
        {
            foreach (Music item in m)
            {
                add(item);
            }
        }

        public override string ToString()
        {
            //no stringbuilder because I don't care
            string str = "Music: \n";
            foreach(Music m in music)
            {
                str += (m.Filepath + "\n");
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
