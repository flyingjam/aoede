using System;
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
            public string filepath { get; private set; }
            public Guid UUID { get; private set; }
            public Music(string file = "")
            {
                filepath = file;
                UUID = Guid.NewGuid();
            }
        }
    }
}
