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
             if (tagMap.ContainsKey(music.Filepath))
            {
                var temp = tagMap[music.Filepath];
                temp.Add(tag);
            }
            else
            {
                var temp = new List<Tag>();
                temp.Add(tag);
                tagMap.Add(music.Filepath, temp);
            }
        }

        public void add(Music music, string str)
        {
            add(music, new Tag(str));
        }

        public List<Tag> get(Music music)
        {
            if (tagMap.ContainsKey(music.Filepath))
                return tagMap[music.Filepath];
            else
                return new List<Tag>();
        }

        public Tag get(Music music, string label)
        {
            var tags = get(music);
            foreach(Tag tag in tags)
            {
                if (tag.Label.ToLower() == label.ToLower())
                {
                    return tag;
                }
            }
            return new Tag("");
        }

    }
}
