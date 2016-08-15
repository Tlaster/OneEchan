using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace OneEchan.Core.Models
{
    public class DetailResult
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public List<DetailList> List { get; set; }
    }
    public class DetailList
    {
        public double Set { get; set; }
        public int ClickCount { get; set; }
        public string FileThumb { get; set; }
        public DateTime? Created_At { get; set; }
    }
    public class SetResult
    {
        public string HighQuality { get; set; }
        public string LowQuality { get; set; }
        public string MediumQuality { get; set; }
        public string OriginalQuality { get; set; }
        public Dictionary<string, string> ToDictionary()
        {
            var dic = new Dictionary<string, string>
            {
                { "Source", OriginalQuality },
            };
            if (HighQuality != null)
                dic.Add("720P", HighQuality);
            if (MediumQuality != null)
                dic.Add("480P", MediumQuality);
            if (LowQuality != null)
                dic.Add("240P", LowQuality);
            return dic;
        }
    }
}
