using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneEchan.Core.Common.Api.Model
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
    }
}
