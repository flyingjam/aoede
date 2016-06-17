using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede
{
    namespace Audio
    {
        class Music
        {
            public string Filepath { get; private set; }
            public string Filename { get; private set; }
            public Guid UUID { get; private set; }
            public int PlaylistID { get; set; }
            public Music(string file = "")
            {
                Filepath = file;
                Filename = Path.GetFileName(Filepath);
                UUID = Guid.NewGuid();
                PlaylistID = -1;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;
                return (((Music)obj).UUID == UUID);
            }
        }
    }
}
