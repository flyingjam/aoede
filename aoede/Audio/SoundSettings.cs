using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMOD;

namespace aoede.Audio
{
    class SoundSettings
    {
        private float volume = 1;

        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value / 100;
            }
        } 
        public SoundSettings()
        {
            
        }

        public void Set(ref Channel channel)
        {
            channel.setVolume(Volume);
        }

    }
}
