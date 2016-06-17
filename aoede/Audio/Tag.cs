using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede.Audio
{
    enum TAGTYPE { LABEL, NUMERIC, GENERIC, PAIR, COMBINED };

    class Tag
    {
        public string Label { get; private set; }
        public string Value { get; private set; }
        public TAGTYPE Type { get; private set; }
        public double Num { get; private set; }

        public Tag(string l)
        {
            Label = l;
            Value = "";
            Num = 0;
            Type = TAGTYPE.LABEL;
        }
        public Tag(string l, double n)
        {
            Label = l;
            Value = "";
            Num = n;
            Type = TAGTYPE.NUMERIC;
        }

        public Tag(string l, string v)
        {
            Label = l;
            Value = v;
            Num = 0;
            Type = TAGTYPE.PAIR;
        }

        public Tag(string l, double n, string v)
        {
            Label = l;
            Value = v;
            Num = 0;
            Type = TAGTYPE.COMBINED;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var t = (Tag)obj;
            return (Label == t.Label) && (Num == t.Num);
        }

		public override int GetHashCode ()
		{
			return Label.GetHashCode() + Value.GetHashCode() + (int)Num;
		}

    }
}
