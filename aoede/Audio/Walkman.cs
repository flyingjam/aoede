using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMOD;

namespace aoede
{
    namespace Audio
    {

        enum STATUS { STOP, PLAY };

        public delegate void MusicChangedEventHandler(object obj, MusicChangedArgs args);
        delegate void PlaylistAddedHandler(object obj, PlaylistAddedArgs args);

        class Walkman
        {

            FMOD.System system;
            FMOD.RESULT result;
            FMOD.Sound current;
            FMOD.Channel channel;

            public event MusicChangedEventHandler MusicChanged;

            protected virtual void OnMusicChanged(MusicChangedArgs args)
            {
                if (MusicChanged != null)
                    MusicChanged(this, args);
            }

            public event PlaylistAddedHandler PlaylistAdded;

            protected virtual void OnPlaylistAdded(PlaylistAddedArgs args)
            {
                if (PlaylistAdded != null)
                    PlaylistAdded(this, args);
            }

            //note: change
            public TagMaster tagMaster { get; private set; }

            Playlist playlist;

            List<Playlist> globalPlaylistList;
            STATUS status;

            public SoundSettings GlobalSettings { get; set; } = new SoundSettings();

            private int nextPlaylistID = 0;

            public Walkman()
            {
                //have proper errorchecking and shit
                result = FMOD.Factory.System_Create(out system);
                CHECKERR(result, ErrorType.Init);
                result = system.init(512, FMOD.INITFLAGS.NORMAL, (IntPtr)0);
                CHECKERR(result, ErrorType.Init);
                globalPlaylistList = new List<Playlist>();
                tagMaster = new TagMaster();

                playlist = new Playlist();
                status = STATUS.STOP;
            }

            public TimeSpan GetTime()
            {
                if(status == STATUS.PLAY)
                {
                    uint position;
                    channel.getPosition(out position, TIMEUNIT.MS);
                    return TimeSpan.FromMilliseconds(position);
                }
                else
                {
                    return TimeSpan.FromMilliseconds(0);
                }
            }

            public TimeSpan GetCurrentLength()
            {
                if(current != null)
                {
                    uint length;
                    current.getLength(out length, TIMEUNIT.MS);
                    return TimeSpan.FromMilliseconds(length);
                }
                else
                {
                    return TimeSpan.FromMilliseconds(0);
                }
            }

            public Music createMusic(string path)
            {
                var m = new Music(path);
                loadMetadata(m);
                return m;
            }

            private void SetSettings()
            {
                if(status == STATUS.PLAY)
                    GlobalSettings.Set(ref channel);
            }

            public Playlist createPlaylist(string name, params string[] fileList)
            {
                var play = new Playlist();
                foreach(string item in fileList)
                {
                    var temp = createMusic(item);
                    play.add(temp);
                }
                play.Name = name;
                play.GlobalID = nextPlaylistID;
                nextPlaylistID++;

                globalPlaylistList.Add(play);
                OnPlaylistAdded(new PlaylistAddedArgs(play));
                return play;
            }

            public void loadMetadata(Music music)
            {
                TAG t;
                int numTags, numTagsUpdated;
                FMOD.Sound sound;
                system.createStream(music.Filepath, FMOD.MODE.OPENONLY, out sound);
                sound.getNumTags(out numTags, out numTagsUpdated);

                for(int i = 0; i < numTags; i++)
                {
                    sound.getTag(null, i, out t);
                    var data = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(t.data);
                    tag(music, t.name, data);
                    //Console.WriteLine("Name: " + t.name + " | Data: " + data);
                }
            }

            public void tag(Music music, Tag tag)
            {
                tagMaster.add(music, tag);
            }

            public void tag(Music music, params Tag[] tags)
            {
                foreach(Tag t in tags)
                {
                    tag(music, t);
                }
            }

            public void tag(Music music, string label)
            {
                tag(music, new Tag(label));
            }

            public void tag(Music music, string label, string value)
            {
                tagMaster.add(music, new Tag(label, value));
            }

            public void tag(Music music, params string[] labels)
            {
                foreach(string label in labels)
                {
                    tag(music, label);
                }
            }

            public void tag(Music music, string label, double num)
            {
                tagMaster.add(music, new Tag(label, num));
            }

            public void tag(Playlist play, string label)
            {
                foreach(Music m in play.music)
                {
                    tag(m, label);
                }
            }

            public void tag(Playlist play, string label, double num)
            {
                foreach(Music m in play.music)
                {
                    tag(m, label, num);
                }
            }

            public Playlist getPlaylist(int ID)
            {
                foreach(Playlist play in globalPlaylistList)
                {
                    if (play.GlobalID == ID)
                        return play;
                }
                return null;
            }

            public PlaylistList playlistSearch(params string[] name)
            {

                var temp = new PlaylistList();

                foreach(Playlist play in globalPlaylistList)
                {
                    if (name.Contains(play.Name))
                    {
                        temp.Add(play);
                    }
                }

                return temp;
            }

            public Playlist Query(Playlist play, params Tag[] tags)
            {
                var temp = play.music.Where(x => tags.Aggregate(true, (prod, next) => prod && (tagMaster.get(x).Contains(next)))).ToList<Music>();
                return new Playlist(temp);
            }

            public List<Music> Query(Playlist play, params string[] names)
            {
                var temp = new List<Music>();

                foreach(Music m in play.music)
                {
                    var tags = getTag(m);
                    var contains = tags.Aggregate(true, (prod, next) => prod && (names.Contains(next.Label)));
                    if (contains)
                        temp.Add(m);
                }
                return temp;
            }

			public List<Music> Query(Playlist play, MoonSharp.Interpreter.Closure func){
                var temp = new List<Music>();
                
                foreach(Music m in play.music)
                {
                    var tags = getTag(m);
                    var filename = m.Filename;
                    try {
                        var truth = func.Call(filename, tags).CastToBool();
                        if (truth)
                        {
                            temp.Add(m);
                        }
                    } catch(Exception e)
                    {
                        //eventually print to internal console
                    }
                }

                return temp;
			}


            public Playlist GetCurrentPlaylist()
            {
                return playlist;
            }

            public Tag getTag(Music m, string label)
            {
                return tagMaster.get(m, label);
            }

            public List<Tag> getTag(Music m)
            {
                return tagMaster.get(m);
            }


            private void playMusic(Music music)
            {
                if (status == STATUS.PLAY)
                    Stop();
                result = system.createStream(music.Filepath, FMOD.MODE.DEFAULT, out current);
                CHECKERR(result, ErrorType.Playback);
                result = system.playSound(current, null, false, out channel);
                CHECKERR(result, ErrorType.Playback);
                status = STATUS.PLAY;
                SetSettings();
                uint length;
                current.getLength(out length, TIMEUNIT.MS);
                OnMusicChanged(new MusicChangedArgs(length, music.Filepath));

            }

            public void Play()
            {
                Music music = playlist.Get();
                if (music != null)
                    playMusic(music);
            }

            public void Play(Music music)
            {
                status = STATUS.PLAY;
                var p = new Playlist(music);
                setPlaylist(p);
                Play();
            }


            public void Stop()
            {

                if (status != STATUS.STOP)
                {
                    status = STATUS.STOP;
                    result = channel.stop();
                    CHECKERR(result, ErrorType.Playback);
                    current.release();
                }
            }

            public void Pause()
            {
                if (status != STATUS.STOP)
                {
                    bool previous = false;
                    result = channel.getPaused(out previous);
                    CHECKERR(result, ErrorType.Playback);
                    result = channel.setPaused(!previous);
                    CHECKERR(result, ErrorType.Playback);
                }
            }

            public void Next()
            {
                playlist.Next();
                Play();
            }

            public void Previous()
            {
                playlist.Previous();
                Play();
            }


            public void setPlaylist(Playlist p)
            {
                playlist = p;
            }

            public void Seek(TimeSpan position)
            {
                if(status == STATUS.PLAY && channel != null)
                {
                    channel.setPosition((uint)position.TotalMilliseconds, TIMEUNIT.MS);
                }
            }

            public void Seek(int position)
            {
                if(status == STATUS.PLAY && channel != null)
                {
                    channel.setPosition((uint)position, TIMEUNIT.MS);
                }
            }
            
            public void playlistSeek(Music m)
            {
                playlist.seek(m);
            }

            public void playlistSeek(int index)
            {
                playlist.seek(index);
            }

            public void playlistSeek(Guid UUID)
            {
                playlist.seek(UUID);
            }

            public bool playlistSeekPID(int PID)
            {
                return playlist.seekPID(PID);
            }

            public void Volume(float value)
            {
                GlobalSettings.Volume = value;
                SetSettings();
                Console.WriteLine(GlobalSettings.Volume);
            }

            public void Volume(double value)
            {
                Volume((float)value);
            }

            public float GetVolume()
            {
                return GlobalSettings.Volume;
            }

            private static void CHECKERR(RESULT result, ErrorType type, string msg = "")
            {
                if (result != FMOD.RESULT.OK)
                {
                    if (type == ErrorType.Init)
                        throw new FMODInitError(msg);
                    else if (type == ErrorType.Playback)
                        throw new FMODPlayError(msg);
                }
            }

            public void release()
            {
                system.release();
            }
        }
    }

    class WalkmanEvents
    {

    }


}
