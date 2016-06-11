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

        class Walkman
        {

            FMOD.System system;
            FMOD.RESULT result;
            FMOD.Sound current;
            FMOD.Channel channel;
            //note: change
            public TagMaster tagMaster { get; private set; }

            Playlist playlist;

            List<Playlist> globalPlaylistList;
            STATUS status;

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

            public Music createMusic(string path)
            {
                var m = new Music(path);
                loadMetadata(m);
                return m;
            }

            public Playlist createPlaylist(params string[] fileList)
            {
                var play = new Playlist();
                foreach(string item in fileList)
                {
                    var temp = createMusic(item);
                    play.add(temp);
                }

                globalPlaylistList.Add(play);
                return play;

            }

            public void loadMetadata(Music music)
            {
                TAG t;
                int numTags, numTagsUpdated;
                FMOD.Sound sound;
                system.createStream(music.filepath, FMOD.MODE.OPENONLY, out sound);
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


            public Playlist query(Playlist play, params Tag[] tags)
            {
                var temp = play.music.Where(x => tags.Aggregate(true, (prod, next) => prod && (tagMaster.get(x).Contains(next)))).ToList<Music>();
                return new Playlist(temp);
            }

			public Playlist query(Playlist play, Func<string, double, string, bool> fun){
                return null;
			}

			public bool query(MoonSharp.Interpreter.Closure func){
				Func<string, bool> f = (x => func.Call (x).CastToBool ());

				return f ("hi");
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
                    stop();

                result = system.createStream(music.filepath, FMOD.MODE.DEFAULT, out current);
                CHECKERR(result, ErrorType.Playback);
                result = system.playSound(current, null, false, out channel);
                CHECKERR(result, ErrorType.Playback);
                status = STATUS.PLAY;
            }

            public void play()
            {
                Music music = playlist.get();
                if (music != null)
                    playMusic(music);
            }

            public void play(Music music)
            {
                status = STATUS.PLAY;
                var p = new Playlist(music);
                setPlaylist(p);
                play();
            }


            public void setPlaylist(Playlist p)
            {
                playlist = p;
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


            public void stop()
            {

                if (status != STATUS.STOP)
                {
                    status = STATUS.STOP;
                    result = channel.stop();
                    CHECKERR(result, ErrorType.Playback);
                    current.release();
                }
            }

            public void pause()
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

            public void next()
            {
                playlist.next();
                Console.WriteLine(playlist.get().filepath);
                play();
            }

            public void previous()
            {
                playlist.previous();
                play();
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
}
