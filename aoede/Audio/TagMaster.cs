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

        public void add(Music music, Tag tag)
        {
             if (tagMap.ContainsKey(music.filepath))
            {
                var temp = tagMap[music.filepath];
                temp.Add(tag);
            }
            else
            {
                var temp = new List<Tag>();
                temp.Add(tag);
                tagMap.Add(music.filepath, temp);
            }
        }

        public void add(Music music, string str)
        {
            add(music, new Tag(str));
        }

        public List<Tag> get(Music music)
        {
            if (tagMap.ContainsKey(music.filepath))
                return tagMap[music.filepath];
            else
                return new List<Tag>();
        }

        public Tag get(Music music, string label)
        {
            var tags = get(music);
            foreach(Tag tag in tags)
            {
                if (tag.label == label)
                {
                    Console.WriteLine("yo");
                    return tag;
                }
            }
            Console.WriteLine("Couldn't Find");
            return new Tag("");
        }

    }
}
