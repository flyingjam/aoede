using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede.Audio
{
    public class MusicChangedArgs : EventArgs { 

        public uint Length { get; private set; }
        public string Filepath { get; private set; }

        public MusicChangedArgs(uint len, string file)
        {
            Length = len;
            Filepath = file; 
        }
    }

    class PlaylistAddedArgs : EventArgs
    {
        public Playlist Addition { get; set; }

        public PlaylistAddedArgs(Playlist playlist)
        {
            Addition = playlist;
        }

    }

}
