using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Models
{
    public class HomeViewModel
    {
        public List<Video> HotVideo { get; internal set; }
        public List<Article> HotArticle { get; internal set; }
        public List<Video> NewVideo { get; internal set; }
        public List<Article> NewArticle { get; internal set; }
    }
}
