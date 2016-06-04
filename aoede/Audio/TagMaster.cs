using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aoede.Audio
{
    class TagMaster
    {
        Dictionary<string, List<Tag>> tagMap;
        public TagMaster()
        {
            tagMap = new Dictionary<string, List<Tag>>();
            
        }

        public void add(Music music, string str)
        {
            if (tagMap.ContainsKey(music.filepath))
            {
                var temp = tagMap[music.filepath];
                temp.Add(new Tag(str));
            }
            else
            {
                var temp = new List<Tag>();
                temp.Add(new Tag(str));
                tagMap.Add(music.filepath, temp);
            }
        }

        public List<Tag> get(Music music)
        {
            if (tagMap.ContainsKey(music.filepath))
                return tagMap[music.filepath];
            else
                return new List<Tag>();
        }

    }
}
