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
        public string label { get; private set; }
        public string value { get; private set; }
        public TAGTYPE type { get; private set; }
        public double num { get; private set; }

        public Tag(string l)
        {
            label = l;
            value = "";
            num = 0;
            type = TAGTYPE.LABEL;
        }
        public Tag(string l, double n)
        {
            label = l;
            value = "";
            num = n;
            type = TAGTYPE.NUMERIC;
        }

        public Tag(string l, string v)
        {
            label = l;
            value = v;
            num = 0;
            type = TAGTYPE.PAIR;
        }

        public Tag(string l, double n, string v)
        {
            label = l;
            value = v;
            num = 0;
            type = TAGTYPE.COMBINED;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var t = (Tag)obj;
            return (label == t.label) && (num == t.num);
        }

		public override int GetHashCode ()
		{
			return label.GetHashCode() + value.GetHashCode() + (int)num;
		}

    }
}
