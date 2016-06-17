using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede.Audio
{
    class MusicList : List<Music>
    {
        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("Music: ");
            foreach (Music m in this)
            {
                str.AppendFormat("{0}", m.Filename);
            }

            return str.ToString();
        }
    }

    class PlaylistList : List<Playlist>
    {
        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("Playlists: ");

            foreach (Playlist p in this)
            {
                str.AppendFormat("Name: {0} | ID {1}", p.Name, p.GlobalID);
            }
            return str.ToString();
        }
    }

    class TagList : List<Tag>
    {
        public override string ToString()
        {
            var str = new StringBuilder();
            str.AppendLine("Playlists: ");

            foreach (Tag t in this)
            {
                str.AppendFormat("Label: {0} | Type: {0} | Value {0} | Numeric Value {0}", t.Label, t.Type, t.Value, t.Num);
            }
            return str.ToString();
        }
    }
}
