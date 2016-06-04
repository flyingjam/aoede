using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede
{
    enum ErrorType { Init, Playback };
    class FMODInitError : Exception
    {
        public FMODInitError()
        {

        }
        public FMODInitError(string msg) : base(msg)
        {

        }
        public FMODInitError(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
    class FMODPlayError : Exception
        {
        public FMODPlayError()
        {

        }
        public FMODPlayError(string msg) : base(msg)
        {

        }
        public FMODPlayError(string msg, Exception inner) : base(msg, inner)
        {

        }
    }
}
