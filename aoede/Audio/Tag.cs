using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede.Audio
{
    enum TAGTYPE { LABEL, NUMERIC, GENERIC };

    class Tag
    {
        string label;
        TAGTYPE type;
        double num;
        Object data;

        public Tag(string str)
        {
            label = str;
            num = -1;
            type = TAGTYPE.LABEL;
            data = str;
        }

        public Tag(string str, double value)
        {
            label = str;
            num = value;
            type = TAGTYPE.NUMERIC;
            data = value;
        }

        public Tag(string str, Object obj)
        {
            label = str;
            data = obj;
        }

        public string getString()
        {
            return (string)data;
        }

        public double getNumeric()
        {
            return num;
        }

        public TAGTYPE getType()
        {
            return type;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var t = (Tag)obj;
            return (label == t.getString()) && (num == t.getNumeric());
        }
    }
}
